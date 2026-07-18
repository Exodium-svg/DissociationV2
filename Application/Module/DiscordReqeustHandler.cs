namespace Application.Module;

// I'll ask about if we should even do this.. 
public interface IDiscordRequest<T>
{
    int Priority { get; }
    Task<T> DiscordAPIRequest();
}

// We can make this way faster in the future. 
public class DiscordRequestHandler<T>
{
    private const int DISCORD_SECOND_REQUEST_LIMIT = 50;
    private readonly PriorityQueue<EnqueuedRequest, ulong> requestQueue = new();
    internal sealed class EnqueuedRequest
    {

    };

}
