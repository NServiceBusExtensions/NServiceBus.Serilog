public class HandlerUsingContextLogger :
    IHandleMessages<StartHandlerUsingContextLogger>
{
    public Task Handle(StartHandlerUsingContextLogger message, HandlerContext context)
    {
        context.LogError("The message", new Exception());
        return Task.CompletedTask;
    }
}