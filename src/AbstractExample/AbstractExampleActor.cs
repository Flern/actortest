using Dapr.Actors.Runtime;

namespace AbstractExample;

public class AbstractExampleActor : AggregateActor<AbstractExampleState>
{
    public AbstractExampleActor(ActorHost actorService, IEventStore eventStore)
        : base(actorService, eventStore)
    {
        When<CreateAbstractExample>(cmd => AbstractExample.Create(cmd, State ));

        When<ApplyAbstractExample>(cmd => AbstractExample.Apply(cmd, State));

        When<ApproveAbstractExample>(cmd => AbstractExample.Approve(cmd, State));

        When<RejectAbstractExample>(cmd => AbstractExample.Reject(cmd, State));

        When<UpdateAbstractExample>(cmd => AbstractExample.Update(cmd, State));
    }
}