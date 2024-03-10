public class TheSaga(ManualResetEvent resetEvent) :
    Saga<TheSaga.TheSagaData>,
    IAmStartedByMessages<StartSaga>,
    IAmStartedByMessages<BackIntoSaga>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TheSagaData> mapper) =>
        mapper.MapSaga(saga => saga.Property)
            .ToMessage<StartSaga>(_ => _.Property)
            .ToMessage<BackIntoSaga>(_ => _.Property);

    public async Task Handle(StartSaga message, HandlerContext context)
    {
        await Task.Delay(1100, context.CancellationToken);
        context.LogInformation("Hello from {@Saga}. Message: {@Message}", nameof(TheSaga), message);
        var backIntoSaga = new BackIntoSaga
        {
            Property = message.Property
        };
        await context.SendLocal(backIntoSaga);
    }

    public async Task Handle(BackIntoSaga message, HandlerContext context)
    {
        await Task.Delay(1100, context.CancellationToken);
        context.LogInformation("Hello from {@Saga}. Message: {@Message}", nameof(TheSaga), message);
        MarkAsComplete();
        resetEvent.Set();
    }

    public class TheSagaData :
        ContainSagaData
    {
        public string? Property { get; set; }
    }
}