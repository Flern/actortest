using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace AbstractExample;

public interface IAggregateActor : IActor
{
    // Command handler entry point
    Task<Tuple<bool, string>> HandleAsync(ApplicationCommand command);
}

public abstract class AggregateActor<T> : Actor, IAggregateActor where T : AggregateState<T>, new()
{
    // Services used by the aggregate root
    private readonly IEventStore _eventStore;

    // Handler registry
    private readonly Dictionary<Type, Func<ApplicationCommand, Task<bool>>> _handlers = new();

    // State management
    public T State { get; set; }

    protected AggregateActor(ActorHost actorService, IEventStore eventStore)
         : base(actorService)
    {
        _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        State = new T();
    }

    // Command handling hooks
    protected void When<TCommand>(Func<TCommand, AggregateState<T>.Result> handler)
        where TCommand : class
        => _handlers.Add(typeof(TCommand), cmd => Commit(handler(cmd as TCommand)));

    // Command handler entry point
    public async Task<Tuple<bool, string>> HandleAsync(ApplicationCommand command)
    {
        // Invoke handler and check results
        if (_handlers.TryGetValue(command.GetType(), out var handler) &&
             !await handler(command))
        {
            const string msg = "Failed to commit events to Event Store.";
            return Tuple.Create(false, new InvalidOperationException(msg).ToString());
        }

        // Successful execution or no handler registered
        return Tuple.Create<bool, string>(true, null!);
    }

    // Event persistence
    private async Task<bool> Commit(AggregateState<T>.Result result)
    {
        if (!await _eventStore.CommitEventsAsync(result.Events)) return false;
        State = result.State;
        return true;
    }
}
