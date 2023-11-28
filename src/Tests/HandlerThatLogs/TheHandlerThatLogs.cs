public class TheHandlerThatLogs(ManualResetEvent @event) :
    IHandleMessages<StartHandlerThatLogs>
{
    public Task Handle(StartHandlerThatLogs message, HandlerContext context)
    {
        var logger = LogManager.GetLogger<TheHandlerThatThrows>();
        logger.Error("The message", new());
        @event.Set();
        return Task.CompletedTask;
    }
}