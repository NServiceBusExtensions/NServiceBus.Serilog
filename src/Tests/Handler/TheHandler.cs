public class TheHandler :
    IHandleMessages<StartHandler>
{
    ManualResetEvent resetEvent;

    public TheHandler(ManualResetEvent resetEvent) =>
        this.resetEvent = resetEvent;

    public Task Handle(StartHandler message, HandlerContext context)
    {
        context.LogInformation("Hello from {@Handler}.");
        resetEvent.Set();
        return Task.CompletedTask;
    }
}