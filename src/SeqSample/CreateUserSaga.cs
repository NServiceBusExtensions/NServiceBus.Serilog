﻿public class CreateUserSaga :
    Saga<CreateUserSagaData>,
    IAmStartedByMessages<CreateUser>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<CreateUserSagaData> mapper) =>
        mapper
            .ConfigureMapping<CreateUser>(message => message.UserName)
            .ToSaga(sagaData => sagaData.UserName);

    public Task Handle(CreateUser message, HandlerContext context)
    {
        Data.UserName = message.UserName;
        Log.Information("User Created {@Message}", message);
        var userCreated = new UserCreated
        {
            UserName = message.UserName
        };
        MarkAsComplete();
        return context.SendLocal(userCreated);
    }
}