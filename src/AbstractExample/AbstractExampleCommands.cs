using System.Runtime.Serialization;

namespace AbstractExample;

[DataContract]
[KnownType(typeof(CreateAbstractExample))]
[KnownType(typeof(ApplyAbstractExample))]
[KnownType(typeof(UpdateAbstractExample))]
[KnownType(typeof(RejectAbstractExample))]
[KnownType(typeof(ApproveAbstractExample))]
public class ApplicationCommand
{
    public Guid Id { get; init; }

    public DateTime DateRequestedUtc { get; init; }

    public CommandStateCode CurrentState { get; set; }

    public string? FailureException { get; set; }

    // Default constructor for derived classes
    protected ApplicationCommand()
    {
        DateRequestedUtc = DateTime.UtcNow;
        Id = Guid.NewGuid();
        CurrentState = CommandStateCode.Processing;
    }
}

public enum CommandStateCode
{
    Accepted,
    Rejected,
    Processing,
    Success,
    Failure
}

[DataContract]
public abstract class AbstractExampleCommand : ApplicationCommand
{
    [DataMember]
    public Guid AbstractExampleId { get; init; }

    protected AbstractExampleCommand(Guid abstractExampleId)
    {
        AbstractExampleId = abstractExampleId;
    }
}

[DataContract]
public class CreateAbstractExample : AbstractExampleCommand
{
    [DataMember]
    public Guid SourceBranchId { get; init; }

    [DataMember]
    public Guid TargetBranchId { get; init; }

    [DataMember]
    public List<Guid> CorrelationIdList { get; init; }

    public CreateAbstractExample(Guid sourceBranchId, Guid targetBranchId, List<Guid> correlationIdList, Guid abstractExampleId )
        : base(abstractExampleId)
    {
        SourceBranchId = sourceBranchId;
        TargetBranchId = targetBranchId;
        CorrelationIdList = correlationIdList;
    }
}

[DataContract]
public class ApplyAbstractExample : AbstractExampleCommand
{
    public ApplyAbstractExample(Guid abstractExampleId)
        : base(abstractExampleId)
    {
    }
}

[DataContract]
public class ApproveAbstractExample : AbstractExampleCommand
{
    public ApproveAbstractExample(Guid abstractExampleId)
        : base(abstractExampleId)
    {
    }
}

[DataContract]
public class RejectAbstractExample : AbstractExampleCommand
{
    public RejectAbstractExample(Guid abstractExampleId)
        : base(abstractExampleId)
    {
    }
}

[DataContract]
public class UpdateAbstractExample : AbstractExampleCommand
{
    [DataMember]
    public List<Guid> CorrelationIdList { get; init; }

    public UpdateAbstractExample(List<Guid> correlationIdList, Guid abstractExampleId)
        : base(abstractExampleId)
    {
        CorrelationIdList = correlationIdList;
    }
}
