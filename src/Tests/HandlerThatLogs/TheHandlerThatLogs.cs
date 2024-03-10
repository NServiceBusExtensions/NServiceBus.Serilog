public class TheHandlerThatLogs(ManualResetEvent @event) :
    IHandleMessages<StartHandlerThatLogs>
{
    public async Task Handle(StartHandlerThatLogs message, HandlerContext context)
    {
        await Task.Delay(1100, context.CancellationToken);
        var logger = LogManager.GetLogger<TheHandlerThatThrows>();
        logger.Error("The message", new());
        @event.Set();
    }
}