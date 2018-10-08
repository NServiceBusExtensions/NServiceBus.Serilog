using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;

class InjectInvokeHandlerContextBehavior : Behavior<IInvokeHandlerContext>
{
    public class Registration : RegisterStep
    {
        public Registration()
            : base(
                stepId: $"Serilog{nameof(InjectInvokeHandlerContextBehavior)}",
                behavior: typeof(InjectInvokeHandlerContextBehavior),
                description: "Injects handler type into the logger",
                factoryMethod: builder => new InjectInvokeHandlerContextBehavior()
                )
        {
        }
    }

    public override Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        var forContext = context.Logger().ForContext("Handler", context.MessageHandler.HandlerType.FullName);
        try
        {
            context.Extensions.Set("SerilogHandlerLogger", forContext);
            return next();
        }
        finally
        {
            context.Extensions.Remove("SerilogHandlerLogger");
        }
    }
}