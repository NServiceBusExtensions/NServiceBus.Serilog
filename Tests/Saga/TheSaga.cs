using System.Threading.Tasks;
using NServiceBus;
using Serilog;

public class TheSaga : Saga<TheSaga.TheSagaData>,
    IAmStartedByMessages<StartSaga>
{
    static ILogger log = Log.ForContext<TheSaga>();

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TheSagaData> mapper)
    {
        mapper.ConfigureMapping<StartSaga>(m => m.Property)
            .ToSaga(s => s.Property);
    }

    public Task Handle(StartSaga message, IMessageHandlerContext context)
    {
        log.Information("Hello from {@Saga}. Message: {@Message}", nameof(TheSaga), message);
        IntegrationTests.resetEvent.Set();
        return Task.CompletedTask;
    }

    public class TheSagaData : ContainSagaData
    {
        public string Property { get; set; }
    }
}