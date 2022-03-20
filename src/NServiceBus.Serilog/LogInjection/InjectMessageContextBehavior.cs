class InjectMessageContextBehavior :
    Behavior<IIncomingLogicalMessageContext>
{
    public class Registration :
        RegisterStep
    {
        public Registration() :
            base(
                stepId: $"Serilog{nameof(InjectMessageContextBehavior)}",
                behavior: typeof(InjectMessageContextBehavior),
                description: "Injects the message into the ExceptionLogState")
        {
        }
    }

    public override Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        var bag = context.Extensions;
        var state = bag.Get<ExceptionLogState>();
        state.IncomingMessage = context.Message.Instance;
        return next();
    }
}