public class TheSaga(ManualResetEvent resetEvent) :
    Saga<TheSaga.TheSagaData>,
    IAmStartedByMessages<StartSaga>,
    IAmStartedByMessages<BackIntoSaga>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TheSagaData> mapper) =>
        mapper.MapSaga(saga => saga.Property)
            .ToMessage<StartSaga>(_ => _.Property)
            .ToMessage<BackIntoSaga>(_ => _.Property);

    public Task Handle(StartSaga message, HandlerContext context)
    {
        context.LogInformation("Hello from {@Saga}. Message: {@Message}", nameof(TheSaga), message);
        var backIntoSaga = new BackIntoSaga
        {
            Property = message.Property
        };
        return context.SendLocal(backIntoSaga);
    }

    public Task Handle(BackIntoSaga message, HandlerContext context)
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