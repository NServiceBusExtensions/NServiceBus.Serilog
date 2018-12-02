using System.Threading.Tasks;
using NServiceBus;

public class TheSaga :
    Saga<TheSaga.TheSagaData>,
    IAmStartedByMessages<StartSaga>,
    IAmStartedByMessages<BackIntoSaga>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TheSagaData> mapper)
    {
        mapper.ConfigureMapping<StartSaga>(m => m.Property)
            .ToSaga(s => s.Property);
        mapper.ConfigureMapping<BackIntoSaga>(m => m.Property)
            .ToSaga(s => s.Property);
    }

    public Task Handle(StartSaga message, IMessageHandlerContext context)
    {
        var logger = context.Logger();
        logger.Information("Hello from {@Saga}. Message: {@Message}", nameof(TheSaga), message);
        var backIntoSaga = new BackIntoSaga
        {
            Property = message.Property
        };
        return context.SendLocal(backIntoSaga);
    }

    public Task Handle(BackIntoSaga message, IMessageHandlerContext context)
    {
        var logger = context.Logger();
        logger.Information("Hello from {@Saga}. Message: {@Message}", nameof(TheSaga), message);
        MarkAsComplete();
        IntegrationTests.resetEvent.Set();
        return Task.CompletedTask;
    }

    public class TheSagaData : ContainSagaData
    {
        public string Property { get; set; }
    }
}