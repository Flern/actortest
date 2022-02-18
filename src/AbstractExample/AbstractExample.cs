
namespace AbstractExample;

public static class AbstractExample
{
    public static AbstractExampleState.Result Create(CreateAbstractExample cmd,
                                                   AbstractExampleState state )
    {
        return
            state.Apply(
                new AbstractExampleCreated(
                    state.Id,
                    cmd.SourceBranchId,
                    cmd.TargetBranchId,
                    cmd.CorrelationIdList)
            );
    }

    public static AbstractExampleState.Result Apply(ApplyAbstractExample cmd,
                                                  AbstractExampleState state )
    {
        return
            state.Apply(new AbstractExampleApplied(
                  state.Id,
                  state.Version)
              );
    }

    public static AbstractExampleState.Result Approve(ApproveAbstractExample cmd,
                                                    AbstractExampleState state )
    {
        return
            state.Apply(new AbstractExampleApproved(
                  state.Id,
                  state.Version)
            );
    }

    public static AbstractExampleState.Result Reject(RejectAbstractExample cmd,
                                                   AbstractExampleState state )
    {
        return
            state.Apply(new AbstractExampleRejected(
                  state.Id,
                  state.Version)
            );
    }

    public static AbstractExampleState.Result Update(UpdateAbstractExample cmd,
                                                   AbstractExampleState state )
    {
        return
            state.Apply(new AbstractExampleUpdated(
                  state.Id,
                  state.Version,
                  cmd.CorrelationIdList)
            );
    }
}
