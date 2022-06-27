public class HandlerBehaviorThatThrows :
    IHandleMessages<StartBehaviorThatThrows>
{
    public Task Handle(StartBehaviorThatThrows message, IMessageHandlerContext context) =>
        Task.CompletedTask;
}