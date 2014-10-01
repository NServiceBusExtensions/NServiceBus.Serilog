using NServiceBus.Logging;
using NServiceBus.Saga;

public class CreateUserSaga : Saga<MySagaData>, IAmStartedByMessages<CreateUser>
{
    static ILog logger = LogManager.GetLogger(typeof (CreateUserSaga));

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MySagaData> mapper)
    {
        mapper.ConfigureMapping<CreateUser>(m => m.UserName)
            .ToSaga(s=>s.UserName);

    }

    public void Handle(CreateUser message)
    {
        Data.UserName = message.UserName;
        logger.Info("User created");
        MarkAsComplete();
        Bus.SendLocal(new UserCreated { UserName = message.UserName });
    }

}

