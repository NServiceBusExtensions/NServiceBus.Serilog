public class GenericHandler :
    IHandleMessages<StartGenericHandler<string>>
{
    ManualResetEvent resetEvent;

    public GenericHandler(ManualResetEvent resetEvent) =>
        this.resetEvent = resetEvent;

    public Task Handle(StartGenericHandler<string> message, IMessageHandlerContext context)
    {
        context.LogInformation("Hello from {@Handler}.");
        resetEvent.Set();
        return Task.CompletedTask;
    }
}