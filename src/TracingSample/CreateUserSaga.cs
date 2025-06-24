public class CreateUserSaga :
    Saga<MySagaData>,
    IAmStartedByMessages<CreateUser>,
    IHandleTimeouts<SagaTimeout>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MySagaData> mapper) =>
        mapper
            .ConfigureMapping<CreateUser>(_ => _.UserName)
            .ToSaga(s => s.UserName);

    public Task Handle(CreateUser message, HandlerContext context)
    {
        Data.UserName = message.UserName;
        Log.Information("User created. Message: {@Message}", message);
        var userCreated = new UserCreated
        {
            UserName = message.UserName
        };
        MarkAsComplete();
        return Task.WhenAll(
            RequestTimeout<SagaTimeout>(context, TimeSpan.FromSeconds(10)),
            context.SendLocal(userCreated));
    }

    public Task Timeout(SagaTimeout state, HandlerContext context)
    {
        Log.Information("Timeout received");
        return Task.CompletedTask;
    }
}