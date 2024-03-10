public class TheHandlerThatThrows :
    IHandleMessages<StartHandlerThatThrows>
{
    public async Task Handle(StartHandlerThatThrows message, HandlerContext context)
    {
        await Task.Delay(1100, context.CancellationToken);
        throw new();
    }
}