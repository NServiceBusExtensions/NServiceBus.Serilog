using System.Threading.Tasks;
using NServiceBus;

public class TheSaga : Saga<TheSaga.TheSagaData>,
    IAmStartedByMessages<StartSaga>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TheSagaData> mapper)
    {
        mapper.ConfigureMapping<StartSaga>(m => m.Property)
            .ToSaga(s => s.Property);
    }

    public Task Handle(StartSaga message, IMessageHandlerContext context)
    {
        context.Logger().Information("Hello from {@Saga}. Message: {@Message}", nameof(TheSaga), message);
        IntegrationTests.resetEvent.Set();
        return Task.CompletedTask;
    }

    public class TheSagaData : ContainSagaData
    {
        public string Property { get; set; }
    }
}