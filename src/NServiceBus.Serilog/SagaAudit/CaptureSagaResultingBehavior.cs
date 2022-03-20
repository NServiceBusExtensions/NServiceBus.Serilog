class CaptureSagaResultingBehavior :
    Behavior<IOutgoingLogicalMessageContext>
{
    public override Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        AppendMessageToState(context);
        return next();
    }

    static void AppendMessageToState(IOutgoingLogicalMessageContext context)
    {
        if (!context.Extensions.TryGet(out SagaUpdatedMessage sagaUpdatedMessage))
        {
            return;
        }

        var logicalMessage = context.Message;
        if (logicalMessage is null)
        {
            //this can happen on control messages
            return;
        }

        var messageType = logicalMessage.MessageType.Name;

        var sagaResultingMessage = new Dictionary<string, string>
        {
            {"Id", context.MessageId},
            {"Type", messageType},
            {"Intent", context.MessageIntent()}
        };
        var destination = context.GetDestinationForUnicastMessages();
        if (destination is not null)
        {
            sagaResultingMessage.Add("Destination", destination);
        }

        sagaUpdatedMessage.ResultingMessages.Add(sagaResultingMessage);
    }

    public class Registration :
        RegisterStep
    {
        public Registration() :
            base(
                stepId: $"Serilog{nameof(CaptureSagaResultingBehavior)}",
                behavior: typeof(CaptureSagaResultingBehavior),
                description: "Reports messages outgoing from a saga to Serilog",
                factoryMethod: _ => new CaptureSagaResultingBehavior())
        {
        }
    }
}