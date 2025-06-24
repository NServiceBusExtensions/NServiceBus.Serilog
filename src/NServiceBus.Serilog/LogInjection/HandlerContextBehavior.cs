class HandlerContextBehavior :
    Behavior<IInvokeHandlerContext>
{
    public class Registration() :
        RegisterStep(
            stepId: $"Serilog{nameof(HandlerContextBehavior)}",
            behavior: typeof(HandlerContextBehavior),
            description: nameof(HandlerContextBehavior));

    public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        var handler = context.HandlerType();
        using (LogContext.PushProperty("Handler", handler))
        {
            await next();
        }
    }
}