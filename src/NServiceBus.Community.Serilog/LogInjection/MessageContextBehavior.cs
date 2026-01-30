class MessageContextBehavior :
    Behavior<IIncomingLogicalMessageContext>
{
    public class Registration :
        RegisterStep
    {
        public Registration() :
            base(
                stepId: $"Serilog{nameof(MessageContextBehavior)}",
                behavior: typeof(MessageContextBehavior),
                description: nameof(MessageContextBehavior)) =>
            InsertBefore(LogIncomingBehavior.Name);
    }

    public override Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        var bag = context.Extensions;
        var state = bag.Get<ExceptionLogState>();
        state.IncomingMessage = context.Message.Instance;
        return next();
    }
}