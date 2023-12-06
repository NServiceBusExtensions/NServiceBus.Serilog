public class NotFoundSaga :
    Saga<NotFoundSaga.TheSagaData>,
    IAmStartedByMessages<NotFoundSagaMessage>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TheSagaData> mapper) =>
        mapper
            .ConfigureMapping<NotFoundSagaMessage>(_ => _.Property)
            .ToSaga(s => s.Property);

    public class TheSagaData :
        ContainSagaData
    {
        public string? Property { get; set; }
    }

    public Task Handle(NotFoundSagaMessage message, HandlerContext context) =>
        Task.CompletedTask;
}