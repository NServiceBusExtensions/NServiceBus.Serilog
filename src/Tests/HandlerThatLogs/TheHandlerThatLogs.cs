public class TheHandlerThatLogs :
    IHandleMessages<StartHandlerThatLogs>
{
    ManualResetEvent resetEvent;

    public TheHandlerThatLogs(ManualResetEvent resetEvent) =>
        this.resetEvent = resetEvent;

    public Task Handle(StartHandlerThatLogs message, HandlerContext context)
    {
        var logger = LogManager.GetLogger<TheHandlerThatThrows>();
        logger.Error("The message", new());
        resetEvent.Set();
        return Task.CompletedTask;
    }
}