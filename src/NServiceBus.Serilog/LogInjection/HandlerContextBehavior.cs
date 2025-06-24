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
        var bag = context.Extensions;
        var forContext = context
            .Logger()
            .ForContext("Handler", handler);
        try
        {
            bag.Set("SerilogHandlerLogger", forContext);
            await next();
        }
        finally
        {
            bag.Remove("SerilogHandlerLogger");
        }
    }
}