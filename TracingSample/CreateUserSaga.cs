using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

public class CreateUserSaga : Saga<MySagaData>, 
    IAmStartedByMessages<CreateUser>
{
    static ILog logger = LogManager.GetLogger(typeof (CreateUserSaga));

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MySagaData> mapper)
    {
        mapper.ConfigureMapping<CreateUser>(m => m.UserName)
            .ToSaga(s=>s.UserName);
    }

    public async Task Handle(CreateUser message, IMessageHandlerContext context)
    {
        Data.UserName = message.UserName;
        logger.Info("User created");
        var userCreated = new UserCreated
        {
            UserName = message.UserName
        };
        await context.SendLocal(userCreated);
        MarkAsComplete();
    }
}

