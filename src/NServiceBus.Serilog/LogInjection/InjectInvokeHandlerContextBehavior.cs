using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using NServiceBus.Serilog;

class InjectInvokeHandlerContextBehavior :
    Behavior<IInvokeHandlerContext>
{
    public class Registration :
        RegisterStep
    {
        public Registration()
            : base(
                stepId: $"Serilog{nameof(InjectInvokeHandlerContextBehavior)}",
                behavior: typeof(InjectInvokeHandlerContextBehavior),
                description: "Injects handler type into the logger")
        {
        }
    }

    public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        var handler = context.HandlerType();
        var bag = context.Extensions;
        var exceptionLogState = bag.Get<ExceptionLogState>();
        exceptionLogState.HandlerType = handler;
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