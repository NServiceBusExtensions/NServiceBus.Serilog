using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;

class InjectInvokeHandlerContextBehavior :
    Behavior<IInvokeHandlerContext>
{
    public class Registration :
        RegisterStep
    {
        public Registration() :
            base(
                stepId: $"Serilog{nameof(InjectInvokeHandlerContextBehavior)}",
                behavior: typeof(InjectInvokeHandlerContextBehavior),
                description: "Injects logger into the handler context")
        {
        }
    }

    public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        var handler = context.HandlerType();
        var bag = context.Extensions;
        var exceptionLogState = bag.Get<ExceptionLogState>();
        exceptionLogState.IncomingMessage = context.MessageBeingHandled;
        var forContext = context.Logger().ForContext("Handler", handler);
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