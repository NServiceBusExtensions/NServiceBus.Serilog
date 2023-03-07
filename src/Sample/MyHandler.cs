public class MyHandler :
    IHandleMessages<MyMessage>
{
    static ILogger log = Log.ForContext<MyHandler>();

    public Task Handle(MyMessage message, HandlerContext context)
    {
        log.Information("Hello from {@Handler}", nameof(MyHandler));
        return Task.CompletedTask;
    }
}