class IncomingLogicalBehavior :
    Behavior<IIncomingLogicalMessageContext>
{
    public class Registration() :
        RegisterStep(
            stepId: Name,
            behavior: typeof(IncomingLogicalBehavior),
            description:  nameof(IncomingLogicalBehavior),
            factoryMethod: _ => new IncomingLogicalBehavior());

    public static string Name = $"Serilog{nameof(IncomingLogicalBehavior)}";

    public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        var type = context.Message.MessageType;
        var typeName = TypeNameConverter.GetName(type);
        var properties = new List<PropertyEnricher>
        {
            typeName.IncomingMessageType,
            typeName.IncomingMessageTypeLong
        };

        using (LogContext.Push(properties))
        {
            await next();
        }
    }
}