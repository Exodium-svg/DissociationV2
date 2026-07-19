namespace Application.Module.DiscordRequests;

/// <summary>
/// Represents a Discord API request that can be queued with a scheduling priority.
/// </summary>
/// <typeparam name="T">The result type returned by the request.</typeparam>
public sealed class DiscordRequest<T>(Func<Task<T>> request, Priority priority = Priority.Normal)
{
    /// <summary>
    /// Gets the priority used when ordering this request in the Discord request queue.
    /// </summary>
    public Priority Priority { get; } = priority;

    /// <summary>
    /// Executes the wrapped Discord API request.
    /// </summary>
    /// <returns>A task that completes with the Discord API request result.</returns>
    public Task<T> DiscordAPIRequest()
    {
        return request();
    }
}

/// <summary>
/// Defines the order in which queued Discord requests should be processed.
/// </summary>
public enum Priority
{
    /// <summary>
    /// Highest priority request, intended for urgent moderation or safety actions.
    /// </summary>
    Critical = 0,

    /// <summary>
    /// High priority request that should run before normal user actions.
    /// </summary>
    High = 1,

    /// <summary>
    /// Default request priority.
    /// </summary>
    Normal = 2,

    /// <summary>
    /// Lower priority request that can wait behind normal work.
    /// </summary>
    Low = 3, 

    /// <summary>
    /// Lowest priority request, intended for background maintenance work.
    /// </summary>
    Background = 4
}
