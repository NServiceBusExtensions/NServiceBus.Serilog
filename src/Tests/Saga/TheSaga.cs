public class TheSaga :
    Saga<TheSaga.TheSagaData>,
    IAmStartedByMessages<StartSaga>,
    IAmStartedByMessages<BackIntoSaga>
{
    ManualResetEvent resetEvent;

    public TheSaga(ManualResetEvent resetEvent) =>
        this.resetEvent = resetEvent;

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TheSagaData> mapper)
    {
        mapper.ConfigureMapping<StartSaga>(m => m.Property)
            .ToSaga(s => s.Property);
        mapper.ConfigureMapping<BackIntoSaga>(m => m.Property)
            .ToSaga(s => s.Property);
    }

    public Task Handle(StartSaga message, IMessageHandlerContext context)
    {
        context.LogInformation("Hello from {@Saga}. Message: {@Message}", nameof(TheSaga), message);
        var backIntoSaga = new BackIntoSaga
        {
            Property = message.Property
        };
        return context.SendLocal(backIntoSaga);
    }

    public Task Handle(BackIntoSaga message, IMessageHandlerContext context)
    {
        context.LogInformation("Hello from {@Saga}. Message: {@Message}", nameof(TheSaga), message);
        MarkAsComplete();
        resetEvent.Set();
        return Task.CompletedTask;
    }

    public class TheSagaData :
        ContainSagaData
    {
        public string? Property { get; set; }
    }
}