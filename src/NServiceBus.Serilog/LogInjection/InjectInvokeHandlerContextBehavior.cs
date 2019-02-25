using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using NServiceBus.Serilog;

class InjectInvokeHandlerContextBehavior : Behavior<IInvokeHandlerContext>
{
    public class Registration : RegisterStep
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
        var handlerName = context.HandlerName();
        var exceptionLogState = context.Extensions.Get<ExceptionLogState>();
        exceptionLogState.HandlerName = handlerName;
        exceptionLogState.Message = context.MessageBeingHandled;
        var forContext = context.Logger().ForContext("Handler", handlerName);
        try
        {
            context.Extensions.Set("SerilogHandlerLogger", forContext);
            await next().ConfigureAwait(false);
        }
        finally
        {
            context.Extensions.Remove("SerilogHandlerLogger");
        }
    }
}