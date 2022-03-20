public class NotFoundSaga :
    Saga<NotFoundSaga.TheSagaData>,
    IAmStartedByMessages<NotFoundSagaMessage>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TheSagaData> mapper) =>
        mapper.ConfigureMapping<NotFoundSagaMessage>(m => m.Property)
            .ToSaga(s => s.Property);

    public class TheSagaData :
        ContainSagaData
    {
        public string? Property { get; set; }
    }

    public Task Handle(NotFoundSagaMessage message, IMessageHandlerContext context) =>
        Task.CompletedTask;
}