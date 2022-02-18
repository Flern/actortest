namespace AbstractExample;

public class AbstractExampleState : AggregateState<AbstractExampleState>
{
    public Guid SourceBranchId { get; set; }
    public Guid TargetBranchId { get; set; }
    public List<Guid> CorrelationIdList { get; } = new();
    public AbstractExampleStatus AbstractExampleStatus { get; set; }

    public override AbstractExampleState When(AbstractExampleState state, object @event)
    {
        return @event switch
        {
            AbstractExampleCreated e => With(state, x =>
           {
               x.Version = 0;
               x.SourceBranchId = e.SourceBranchId;
               x.TargetBranchId = e.TargetBranchId;
               x.CorrelationIdList.Clear();
               x.CorrelationIdList.AddRange(e.CorrelationIdList);
               x.AbstractExampleStatus = AbstractExampleStatus.Created;
           }),
            AbstractExampleApplied e => With(state, x =>
           {
               x.Version++;
               x.AbstractExampleStatus = AbstractExampleStatus.Applied;
           }),
            AbstractExampleApproved e => With(state, x =>
           {
               x.Version++;
               x.AbstractExampleStatus = AbstractExampleStatus.Approved;
           }),
            AbstractExampleRejected e => With(state, x =>
           {
               x.Version++;
               x.AbstractExampleStatus = AbstractExampleStatus.Rejected;
           }),
            AbstractExampleUpdated e => With(state, x =>
           {
               x.Version++;
               x.CorrelationIdList.Clear();
               x.CorrelationIdList.AddRange(e.CorrelationIdList);
           }),
            _ => state
        };
    }

    protected override bool EnsureValidState(AbstractExampleState newState)
        => newState switch
        {
            { } state when state.SourceBranchId == Guid.Empty ||
                           state.TargetBranchId == Guid.Empty => false,
            _ => true
        };
}

public enum AbstractExampleStatus
{
    Created,
    Approved,
    Rejected,
    Applied
}
