public class TheHandler(ManualResetEvent resetEvent) :
    IHandleMessages<StartHandler>
{
    public async Task Handle(StartHandler message, HandlerContext context)
    {
        await Task.Delay(1100, context.CancellationToken);
        context.LogInformation("Hello from {@Handler}.");
        resetEvent.Set();
    }
}