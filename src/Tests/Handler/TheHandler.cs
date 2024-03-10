public class TheHandler :
    IHandleMessages<StartHandler>
{
    ManualResetEvent resetEvent;

    public TheHandler(ManualResetEvent resetEvent) =>
        this.resetEvent = resetEvent;

    public async Task Handle(StartHandler message, HandlerContext context)
    {
        await Task.Delay(1100, context.CancellationToken);
        context.LogInformation("Hello from {@Handler}.");
        resetEvent.Set();
    }
}