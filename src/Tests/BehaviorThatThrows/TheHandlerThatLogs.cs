public class HandlerBehaviorThatThrows :
    IHandleMessages<StartBehaviorThatThrows>
{
    public Task Handle(StartBehaviorThatThrows message, HandlerContext context) =>
        Task.Delay(1100, context.CancellationToken);
}