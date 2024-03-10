public class GenericHandler(ManualResetEvent @event) :
    IHandleMessages<StartGenericHandler<string>>
{
    public async Task Handle(StartGenericHandler<string> message, HandlerContext context)
    {
        await Task.Delay(1100, context.CancellationToken);
        context.LogInformation("Hello from {@Handler}.");
        @event.Set();
    }
}