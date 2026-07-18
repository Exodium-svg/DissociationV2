using Application.Utils;
using static Application.Utils.LoggingMessages.Debug;
using static Application.Utils.LoggingMessages.Error;

namespace Application.Module.DiscordRequests;

/// <summary>
/// Queues Discord API requests and processes them through a priority-aware rate limiter.
/// </summary>
public class DiscordRequestService
{
    /// <summary>
    /// The maximum number of Discord requests allowed per second.
    /// </summary>
    private const int DISCORD_SECOND_REQUEST_LIMIT = 50;

    /// <summary>
    /// Delay between processed requests to stay within the per-second request limit.
    /// </summary>
    private const int DELAY_BETWEEN_REQUESTS_MS = 1000 / DISCORD_SECOND_REQUEST_LIMIT;

    /// <summary>
    /// Maximum amount of time a request may run before the queue treats it as timed out.
    /// </summary>
    private static readonly TimeSpan REQUEST_TIMEOUT = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Stores pending requests so higher priority work, such as moderation actions, runs first.
    /// </summary>
    private readonly PriorityQueue<EnqueuedRequest, Priority> requestQueue = new();

    /// <summary>
    /// Signals the background processor when a new request is available.
    /// </summary>
    private readonly SemaphoreSlim queueSignal = new(0);

    /// <summary>
    /// Synchronizes access to the priority queue.
    /// </summary>
    private readonly Lock QueueLock = new();

    /// <summary>
    /// Logger used to record request queue activity and failures.
    /// </summary>
    private readonly Logger logger;

    /// <summary>
    /// Wraps a queued request with the metadata and completion handling needed by the processor.
    /// </summary>
    /// <param name="name">Name used in queue log messages.</param>
    /// <param name="executeAsync">Delegate that executes the queued request.</param>
    /// <param name="setException">Delegate that completes the original request with an exception.</param>
    private sealed class EnqueuedRequest(string name, Func<Task> executeAsync, Action<Exception> setException)
    {
        /// <summary>
        /// Gets the request name used in log messages.
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Executes the queued request delegate.
        /// </summary>
        /// <returns>A task that represents the request execution.</returns>
        public Task ExecuteAsync() => executeAsync();

        /// <summary>
        /// Completes the original request with the supplied exception.
        /// </summary>
        /// <param name="exception">The exception to report to the original request caller.</param>
        public void SetException(Exception exception) => setException(exception);
    }

    /// <summary>
    /// Initializes a Discord request queue and starts the background processor.
    /// </summary>
    /// <param name="logger">Logger used for queue diagnostics.</param>
    public DiscordRequestService(Logger logger)
    {
        this.logger = logger;
        _ = ProcessQueueAsync();
    }

    /// <summary>
    /// Adds a Discord request to the priority queue.
    /// </summary>
    /// <typeparam name="T">The result type returned by the request.</typeparam>
    /// <param name="request">The request to enqueue.</param>
    /// <returns>A task that completes when the queued request finishes.</returns>
    public Task<T> Enqueue<T>(DiscordRequest<T> request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        var enqueuedRequest = new EnqueuedRequest( request.GetType().Name, async () => {
                T result = await request.DiscordAPIRequest();
                completion.TrySetResult(result);
        }, exception => completion.TrySetException(exception));

        // Lock the critical section
        lock (QueueLock)
        {
            requestQueue.Enqueue(enqueuedRequest, request.Priority);
            logger.Log(string.Format(DISCORD_REQUEST_ENQUEUED, enqueuedRequest.Name, request.Priority, requestQueue.Count), LogLevel.Debug);
        }

        // Tell the background processor that work is available.
        queueSignal.Release();

        // Return a task that will eventually contain the result.
        return completion.Task;
    }

    /// <summary>
    /// Wraps and adds a Discord request delegate to the priority queue.
    /// </summary>
    /// <typeparam name="T">The result type returned by the request.</typeparam>
    /// <param name="request">The request delegate to enqueue.</param>
    /// <param name="priority">Priority used to order the request.</param>
    /// <returns>A task that completes when the queued request finishes.</returns>
    public Task<T> Enqueue<T>(Func<Task<T>> request, Priority priority = Priority.Normal)
    {
        ArgumentNullException.ThrowIfNull(request);
        return Enqueue(new DiscordRequest<T>(request, priority));
    }

    /// <summary>
    /// Continuously processes queued Discord requests as they become available.
    /// </summary>
    /// <returns>A task that represents the lifetime of the background queue processor.</returns>
    private async Task ProcessQueueAsync()
    {
        while (true)
        {
            // Wait until something is added to the queue.
            await queueSignal.WaitAsync();

            EnqueuedRequest enqueuedRequest;

            // Again lock our critical section.. 
            lock (QueueLock)
            {
                enqueuedRequest = requestQueue.Dequeue();
                logger.Log(string.Format(DISCORD_REQUEST_DEQUEUED, enqueuedRequest.Name, requestQueue.Count), LogLevel.Debug);
            }


            await TryExecuteRequest(enqueuedRequest);

            // Basic 50-requests-per-second limiter.
            await Task.Delay(DELAY_BETWEEN_REQUESTS_MS);
        }
    }

    /// <summary>
    /// Executes a queued request and records timeout or execution failures on the original task.
    /// </summary>
    /// <param name="enqueuedRequest">The queued request to execute.</param>
    private async Task TryExecuteRequest(EnqueuedRequest enqueuedRequest)
    {
        // Lets see 
        try
        {
            logger.Log(string.Format(DISCORD_REQUEST_EXECUTING, enqueuedRequest.Name), LogLevel.Debug);
            // Run the task
            Task requestTask = enqueuedRequest.ExecuteAsync();

            // Something went horribly wrong.. 
            try
            {
                await requestTask.WaitAsync(REQUEST_TIMEOUT);
                logger.Log(string.Format(DISCORD_REQUEST_COMPLETED, enqueuedRequest.Name), LogLevel.Debug);
            }
            catch (TimeoutException exception)
            {
                enqueuedRequest.SetException(exception);
                ObserveTimedOutRequest(enqueuedRequest, requestTask);
                logger.Log(string.Format(DISCORD_REQUEST_TIMED_OUT, enqueuedRequest.Name, REQUEST_TIMEOUT.TotalSeconds), LogLevel.Warning);
            }
        }
         // Something went even more horribly wrong.. 
        catch (Exception exception)
        {
            enqueuedRequest.SetException(exception);

            logger.Log($"{REQUEST_EXCEPTION}, {exception.Message}", LogLevel.Error);
        }
    }
    
    /// <summary>
    /// Logs any later fault from a request that already timed out.
    /// </summary>
    /// <param name="enqueuedRequest">The timed-out queued request.</param>
    /// <param name="requestTask">The underlying task that may fault after timing out.</param>
    private void ObserveTimedOutRequest(EnqueuedRequest enqueuedRequest, Task requestTask) =>
        _ = requestTask.ContinueWith(task => 
                logger.Log(
                    string.Format(TIMED_OUT_DISCORD_REQUEST_FAILED, enqueuedRequest.Name, task.Exception?.GetBaseException().Message),LogLevel.Warning),
                TaskContinuationOptions.OnlyOnFaulted);
}
