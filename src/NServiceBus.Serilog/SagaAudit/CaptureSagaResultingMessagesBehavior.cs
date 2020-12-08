using System.Threading.Tasks;
using System;
using NServiceBus.Pipeline;
using NServiceBus.Serilog;

class CaptureSagaResultingMessagesBehavior :
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
        if (logicalMessage == null)
        {
            //this can happen on control messages
            return;
        }

        SagaChangeOutput sagaResultingMessage = new
        (
            resultingMessageId: context.MessageId,
            messageType: logicalMessage.MessageType.ToString(),
            destination: context.GetDestinationForUnicastMessages(),
            messageIntent: context.MessageIntent()
        );
        sagaUpdatedMessage.ResultingMessages.Add(sagaResultingMessage);
    }

    public class Registration :
        RegisterStep
    {
        public Registration() :
            base(
                stepId: $"Serilog{nameof(CaptureSagaResultingMessagesBehavior)}",
                behavior: typeof(CaptureSagaResultingMessagesBehavior),
                description: "Reports messages outgoing from a saga to Serilog")
        {
        }
    }
}