using System.Diagnostics;
using Application.Module.DiscordRequests;
using Application.Utils;

namespace Application.Tests;

/// <summary>
/// Tests Discord request wrappers and queue behavior with mock request delegates.
/// </summary>
[TestClass]
[DoNotParallelize]
public sealed class DiscordRequestsTests
{
    /// <summary>
    /// Verifies that a wrapped Discord request returns the mock delegate result.
    /// </summary>
    [TestMethod]
    public async Task RequestReturnsMockResult()
    {
        var request = new DiscordRequest<string>(() => Task.FromResult("mock response"));

        string result = await request.DiscordAPIRequest();

        Assert.AreEqual("mock response", result);
    }

    /// <summary>
    /// Verifies that a request uses normal priority when no priority is supplied.
    /// </summary>
    [TestMethod]
    public void DefaultsToNormalPriority()
    {
        var request = new DiscordRequest<int>(() => Task.FromResult(1));

        Assert.AreEqual(Priority.Normal, request.Priority);
    }

    /// <summary>
    /// Verifies that a request stores the priority supplied by the caller.
    /// </summary>
    [TestMethod]
    public void StoresPriority()
    {
        var request = new DiscordRequest<int>(() => Task.FromResult(1), Priority.Critical);

        Assert.AreEqual(Priority.Critical, request.Priority);
    }

    /// <summary>
    /// Verifies that the request service executes a queued mock request and returns its result.
    /// </summary>
    [TestMethod]
    public async Task EnqueueReturnsMockResult()
    {
        DiscordRequestService service = CreateService();

        string result = await service.Enqueue(() => Task.FromResult("mock queued response"));

        Assert.AreEqual("mock queued response", result);
    }

    /// <summary>
    /// Verifies that asynchronous failures from queued mock requests are propagated to callers.
    /// </summary>
    [TestMethod]
    public async Task EnqueuePropagatesFailure()
    {
        DiscordRequestService service = CreateService();
        var expected = new InvalidOperationException("mock failure");

        Task<string> result = service.Enqueue<string>(() => Task.FromException<string>(expected));

        try
        {
            await result;
            Assert.Fail("Expected the queued request to throw.");
        }
        catch (InvalidOperationException actual)
        {
            Assert.AreSame(expected, actual);
        }
    }

    /// <summary>
    /// Verifies that enqueueing a null request object throws an argument exception.
    /// </summary>
    [TestMethod]
    public void EnqueueRejectsNullRequest()
    {
        DiscordRequestService service = CreateService();
        DiscordRequest<string>? request = null;

        Assert.ThrowsExactly<ArgumentNullException>(() => service.Enqueue(request!));
    }

    /// <summary>
    /// Verifies that enqueueing a null request delegate throws an argument exception.
    /// </summary>
    [TestMethod]
    public void EnqueueRejectsNullDelegate()
    {
        DiscordRequestService service = CreateService();
        Func<Task<string>>? request = null;

        Assert.ThrowsExactly<ArgumentNullException>(() => service.Enqueue(request!));
    }

    /// <summary>
    /// Verifies that synchronous delegate failures are propagated through the queued task.
    /// </summary>
    [TestMethod]
    public async Task EnqueuePropagatesSyncThrow()
    {
        DiscordRequestService service = CreateService();
        var expected = new InvalidOperationException("sync failure");

        Task<string> result = service.Enqueue<string>(() => throw expected);

        try
        {
            await result;
            Assert.Fail("Expected the queued request to throw.");
        }
        catch (InvalidOperationException actual)
        {
            Assert.AreSame(expected, actual);
        }
    }

    /// <summary>
    /// Verifies that a higher priority request runs before a lower priority request waiting in the queue.
    /// </summary>
    [TestMethod]
    public async Task HigherPriorityRunsFirst()
    {
        DiscordRequestService service = CreateService();
        var firstRequestStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseFirstRequest = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var executionOrder = new List<string>();

        Task<string> firstRequest = service.Enqueue(async () =>
        {
            executionOrder.Add("first");
            firstRequestStarted.SetResult();
            await releaseFirstRequest.Task;
            return "first";
        }, Priority.Normal);

        await firstRequestStarted.Task.WaitAsync(TimeSpan.FromSeconds(1), TestContext.CancellationToken);

        Task<string> lowPriorityRequest = service.Enqueue(() =>
        {
            executionOrder.Add("low");
            return Task.FromResult("low");
        }, Priority.Low);

        Task<string> highPriorityRequest = service.Enqueue(() =>
        {
            executionOrder.Add("high");
            return Task.FromResult("high");
        }, Priority.High);

        releaseFirstRequest.SetResult();

        await Task.WhenAll(firstRequest, lowPriorityRequest, highPriorityRequest).WaitAsync(TimeSpan.FromSeconds(2), TestContext.CancellationToken);

        CollectionAssert.AreEqual(new[] { "first", "high", "low" }, executionOrder);
    }

    /// <summary>
    /// Verifies that critical priority work runs before background work waiting in the queue.
    /// </summary>
    [TestMethod]
    public async Task CriticalRunsBeforeBackground()
    {
        DiscordRequestService service = CreateService();
        var firstRequestStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseFirstRequest = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var executionOrder = new List<string>();

        Task firstRequest = service.Enqueue(async () =>
        {
            firstRequestStarted.SetResult();
            await releaseFirstRequest.Task;
            return true;
        });

        await firstRequestStarted.Task.WaitAsync(TimeSpan.FromSeconds(1));

        Task backgroundRequest = service.Enqueue(() =>
        {
            executionOrder.Add("background");
            return Task.FromResult(true);
        }, Priority.Background);

        Task criticalRequest = service.Enqueue(() =>
        {
            executionOrder.Add("critical");
            return Task.FromResult(true);
        }, Priority.Critical);

        releaseFirstRequest.SetResult();

        await Task.WhenAll(firstRequest, backgroundRequest, criticalRequest).WaitAsync(TimeSpan.FromSeconds(2), TestContext.CancellationToken);

        CollectionAssert.AreEqual(new[] { "critical", "background" }, executionOrder);
    }

    /// <summary>
    /// Verifies that the queue waits between requests to enforce the request rate limiter.
    /// </summary>
    [TestMethod]
    public async Task WaitsBetweenRequests()
    {
        DiscordRequestService service = CreateService();
        var firstRequestStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseFirstRequest = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var secondRequestStarted = new TaskCompletionSource<DateTimeOffset>(TaskCreationOptions.RunContinuationsAsynchronously);

        Task firstRequest = service.Enqueue(async () =>
        {
            firstRequestStarted.SetResult();
            await releaseFirstRequest.Task;
            return true;
        });

        await firstRequestStarted.Task.WaitAsync(TimeSpan.FromSeconds(1));

        Task secondRequest = service.Enqueue(() =>
        {
            secondRequestStarted.SetResult(DateTimeOffset.UtcNow);
            return Task.FromResult(true);
        });

        DateTimeOffset firstRequestReleasedAt = DateTimeOffset.UtcNow;
        releaseFirstRequest.SetResult();

        await Task.WhenAll(firstRequest, secondRequest).WaitAsync(TimeSpan.FromSeconds(2), TestContext.CancellationToken);

        TimeSpan delay = await secondRequestStarted.Task - firstRequestReleasedAt;
        Assert.IsTrue(delay >= TimeSpan.FromMilliseconds(15), $"Expected at least a 15 ms delay, but got {delay.TotalMilliseconds} ms.");
    }

    /// <summary>
    /// Verifies that a large burst of mock requests can be enqueued quickly.
    /// </summary>
    [TestMethod]
    [TestCategory("Performance")]
    public async Task QueuesBurstQuickly()
    {
        const int requestCount = 1_000;
        DiscordRequestService service = CreateService();

        var stopwatch = Stopwatch.StartNew();
        Task<int>[] requests = [.. Enumerable.Range(0, requestCount).Select(value => service.Enqueue(() => Task.FromResult(value), Priority.Background))];
        stopwatch.Stop();

        Assert.IsTrue(
            stopwatch.Elapsed < TimeSpan.FromMilliseconds(500),
            $"Expected enqueueing {requestCount} mock requests to take less than 500 ms, but took {stopwatch.Elapsed.TotalMilliseconds} ms.");

        int firstResult = await requests[0].WaitAsync(TimeSpan.FromSeconds(2));

        Assert.AreEqual(0, firstResult);
    }

    /// <summary>
    /// Verifies that mock request processing remains close to the configured rate limit.
    /// </summary>
    [TestMethod]
    [TestCategory("Performance")]
    public async Task ProcessesNearRateLimit()
    {
        const int requestCount = 10;
        DiscordRequestService service = CreateService();
        var completionTimes = new List<TimeSpan>(requestCount);
        var stopwatch = Stopwatch.StartNew();

        Task[] requests = [.. Enumerable.Range(0, requestCount)
            .Select(_ => service.Enqueue(() =>
            {
                completionTimes.Add(stopwatch.Elapsed);
                return Task.FromResult(true);
            }))
            .Cast<Task>()];

        await Task.WhenAll(requests).WaitAsync(TimeSpan.FromSeconds(2), TestContext.CancellationToken);

        TimeSpan totalDuration = completionTimes[^1] - completionTimes[0];
        TimeSpan expectedMinimum = TimeSpan.FromMilliseconds(20 * (requestCount - 1));

        Assert.IsTrue(
            totalDuration >= expectedMinimum - TimeSpan.FromMilliseconds(20),
            $"Expected processing to respect the 50 requests/sec limiter, but {requestCount} requests completed in {totalDuration.TotalMilliseconds} ms.");
    }

    /// <summary>
    /// Creates a Discord request service with an in-memory logger for isolated tests.
    /// </summary>
    /// <returns>A request service configured with mock logging.</returns>
    private static DiscordRequestService CreateService()
    {
        var logger = new Logger([new TestLoggerService()]);
        return new DiscordRequestService(logger);
    }

    public TestContext TestContext { get; set; }

}
