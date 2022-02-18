namespace AbstractExample;

public interface IAggregateState<T>
{
    T When(T state, object @event);

    Guid Id { get; }

    bool SplitActorId { get; }

    long Version { get; }
}

public abstract class AggregateState<T> : IAggregateState<T>
    where T : class, new()
{
    public abstract T When(T state, object @event);

    public Guid Id { get; set; }

    public bool SplitActorId { get; set; } = true;

    public long Version { get; protected set; } = -1;

    protected T With(T state, Action<T> update)
    {
        update(state);
        return state;
    }

    protected abstract bool EnsureValidState(T newState);

    public Result EmptyResult() => new(this as T, new List<object>());

    T Apply(T state, object @event)
    {
        var newState = When(state, @event);

        if (!EnsureValidState(newState))
        {
            throw new InvalidEntityState(this, "Post-checks failed");
        }

        return newState;
    }

    public Result Apply(params object[] events)
    {
        var stateCopy = (T)this.DeepCopy();
        var newState = events.Aggregate(stateCopy, Apply);
        return new Result(newState, events);
    }

    public Result Apply(Result result, params object[] events)
    {
        var newState = result.State;
        newState = events.Aggregate(newState, Apply);
        return new Result(newState, result.Events.ToList().Concat(events));
    }

    class InvalidEntityState : Exception
    {
        public InvalidEntityState(object entity, string message)
            : base($"Entity {entity.GetType().Name} state change rejected, {message}")
        { }
    }

    public class Result
    {
        public T State { get; }
        public IEnumerable<object> Events { get; }

        public Result(T state, IEnumerable<object> events)
        {
            State = state;
            Events = events;
        }
    }
}