using Dapr.Actors.Client;
using HealthChecks.UI.Client;
using AbstractExample;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

// Get some ID info
string? AppName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

var builder = WebApplication.CreateBuilder(args);

// Add health checks & actors
builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
builder.Services.AddActors(options => options.Actors.RegisterActor<AbstractExampleActor>());

// Set up dependencies
builder.Services.AddSingleton<IEventStore>(new EventStore());

// Finalize app build
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    //endpoints.MapSubscribeHandler();
    endpoints.MapActorsHandlers();
    endpoints.MapHealthChecks("/hc", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
    endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
    {
        Predicate = r => r.Name.Contains("self")
    });
    endpoints.MapGet( "/{debugConfig}", (bool debugConfig) =>
    {
        Guid absId = Guid.NewGuid();
        Guid cr1Id = Guid.NewGuid();
        Guid cr2Id = Guid.NewGuid();
        Guid cr3Id = Guid.NewGuid();
        Guid srcId = Guid.NewGuid();
        Guid tgtId = Guid.NewGuid();

        var cmd = new CreateAbstractExample( srcId, tgtId, new List<Guid> { cr1Id, cr2Id }, absId );

        // Flag to set to debug config or live actor
        //bool debugConfig = true;
        IAggregateActor proxy;

        // Setup for test config
        if (debugConfig)
        {
            var testOptions = new Dapr.Actors.Runtime.ActorTestOptions
            {
                ActorId = new Dapr.Actors.ActorId( absId.ToString() )
            };
            var actorService = Dapr.Actors.Runtime.ActorHost.CreateForTest<AbstractExampleActor>( testOptions );
            proxy = new AbstractExampleActor( actorService, new EventStore() );
        }
        else
        {
            proxy = ActorProxy.Create<IAggregateActor>(new Dapr.Actors.ActorId(absId.ToString()), "AbstractExampleActor");
        }

        proxy.HandleAsync( cmd ).Wait();

        proxy.HandleAsync( new ApproveAbstractExample( absId ) ).Wait();
        proxy.HandleAsync( new ApplyAbstractExample( absId ) ).Wait();
        proxy.HandleAsync( new RejectAbstractExample( absId ) ).Wait();
        proxy.HandleAsync( new UpdateAbstractExample( new List<Guid> { cr3Id }, absId ) ).Wait();
    } );
});

app.Run();
