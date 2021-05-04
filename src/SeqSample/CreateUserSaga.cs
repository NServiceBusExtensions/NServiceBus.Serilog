using System.Threading.Tasks;
using NServiceBus;

public class CreateUserSaga :
    Saga<CreateUserSagaData>,
    IAmStartedByMessages<CreateUser>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<CreateUserSagaData> mapper)
    {
        mapper.ConfigureMapping<CreateUser>(message => message.UserName)
            .ToSaga(sagaData => sagaData.UserName);
    }

    public Task Handle(CreateUser message, IMessageHandlerContext context)
    {
        Data.UserName = message.UserName;
        context.LogInformation("User Created {@Message}", message);
        UserCreated userCreated = new()
        {
            UserName = message.UserName
        };
        MarkAsComplete();
        return context.SendLocal(userCreated);
    }
}