public class GenericHandler(ManualResetEvent @event) :
    IHandleMessages<StartGenericHandler<string>>
{
    public Task Handle(StartGenericHandler<string> message, HandlerContext context)
    {
        context.LogInformation("Hello from {@Handler}.");
        @event.Set();
        return Task.CompletedTask;
    }
}