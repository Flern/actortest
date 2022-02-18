using System.Runtime.Serialization;

namespace AbstractExample;

public class Event
{
    public Guid Id { get; init; }

    public Guid AggregateRootId { get; init; }

    public long AggregateRootVersion { get; init; }

    public DateTime CreatedAt { get; init; }

    public Event() { }

    // Parameterized constructor
    public Event(Guid aggregateRootId, long aggregateRootVersion)
    {
        Id = Guid.NewGuid();
        AggregateRootId = aggregateRootId;
        AggregateRootVersion = aggregateRootVersion;
        CreatedAt = DateTime.UtcNow;
    }
}
public class AbstractExampleCreated : Event
{

    public Guid SourceBranchId { get; init; }
    public Guid TargetBranchId { get; init; }
    public List<Guid> CorrelationIdList { get; init; }

    public AbstractExampleCreated(Guid aggregateRootId, Guid sourceBranchId, Guid targetBranchId, List<Guid> correlationIdList)
        : base(aggregateRootId, 0)
    {
        SourceBranchId = sourceBranchId;
        TargetBranchId = targetBranchId;
        CorrelationIdList = correlationIdList;
    }
}

[DataContract]
public class AbstractExampleApplied : Event
{
    public AbstractExampleApplied(Guid aggregateRootId, long aggregateRootVersion)
        : base(aggregateRootId, aggregateRootVersion)
    {
    }
}

[DataContract]
public class AbstractExampleApproved : Event
{
    public AbstractExampleApproved(Guid aggregateRootId, long aggregateRootVersion)
        : base(aggregateRootId, aggregateRootVersion)
    { }
}

[DataContract]
public class AbstractExampleRejected : Event
{
    public AbstractExampleRejected(Guid aggregateRootId, long aggregateRootVersion)
        : base(aggregateRootId, aggregateRootVersion)
    { }
}

[DataContract]
public class AbstractExampleUpdated : Event
{

    public List<Guid> CorrelationIdList { get; init; }

    public AbstractExampleUpdated(Guid aggregateRootId, long aggregateRootVersion, List<Guid> correlationIdList)
        : base(aggregateRootId, aggregateRootVersion)
    {
        CorrelationIdList = correlationIdList;
    }
}

