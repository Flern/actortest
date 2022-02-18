
namespace AbstractExample;

public interface IEventStore
{
    Task<bool> CommitEventsAsync(IEnumerable<object> events);
}

public class EventStore : IEventStore
{
    Task<bool> IEventStore.CommitEventsAsync(IEnumerable<object> events)
    {
        return Task.FromResult(true);
    }
}
