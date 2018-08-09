using System;
using System.Threading.Tasks;
using NServiceBus;
using Serilog;

public class CreateUserSaga : Saga<MySagaData>,
    IAmStartedByMessages<CreateUser>,
    IHandleTimeouts<SagaTimeout>
{
    static ILogger log = Log.ForContext<CreateUserSaga>();

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MySagaData> mapper)
    {
        mapper.ConfigureMapping<CreateUser>(m => m.UserName)
            .ToSaga(s => s.UserName);
    }

    public Task Handle(CreateUser message, IMessageHandlerContext context)
    {
        Data.UserName = message.UserName;
        log.Information("User created. Message: {@Message}", message);
        var userCreated = new UserCreated
        {
            UserName = message.UserName
        };
        MarkAsComplete();
        return Task.WhenAll(
            RequestTimeout<SagaTimeout>(context, TimeSpan.FromSeconds(10)),
            context.SendLocal(userCreated));
    }

    public Task Timeout(SagaTimeout state, IMessageHandlerContext context)
    {
        log.Information("Timeout received");
        return Task.CompletedTask;
    }
}