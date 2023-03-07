public class HandlerBehaviorThatThrows :
    IHandleMessages<StartBehaviorThatThrows>
{
    public Task Handle(StartBehaviorThatThrows message, HandlerContext context) =>
        Task.CompletedTask;
}