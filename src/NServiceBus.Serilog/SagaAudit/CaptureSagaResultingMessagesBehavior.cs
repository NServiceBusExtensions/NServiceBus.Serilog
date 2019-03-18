using System.Threading.Tasks;
using System;
using NServiceBus.Pipeline;
using NServiceBus.Serilog;

class CaptureSagaResultingMessagesBehavior :
    Behavior<IOutgoingLogicalMessageContext>
{
    SagaUpdatedMessage sagaUpdatedMessage;

    public override Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        AppendMessageToState(context);
        return next();
    }

    void AppendMessageToState(IOutgoingLogicalMessageContext context)
    {
        if (!context.Extensions.TryGet(out sagaUpdatedMessage))
        {
            return;
        }

        var logicalMessage = context.Message;
        if (logicalMessage == null)
        {
            //this can happen on control messages
            return;
        }

        var sagaResultingMessage = new SagaChangeOutput
        {
            ResultingMessageId = context.MessageId,
            MessageType = logicalMessage.MessageType.ToString(),
            Destination = context.GetDestinationForUnicastMessages(),
            MessageIntent = context.MessageIntent()
        };
        sagaUpdatedMessage.ResultingMessages.Add(sagaResultingMessage);
    }

    public class Registration :
        RegisterStep
    {
        public Registration()
            : base(
                stepId: $"Serilog{nameof(CaptureSagaResultingMessagesBehavior)}",
                behavior: typeof(CaptureSagaResultingMessagesBehavior),
                description: "Reports messages outgoing from a saga to Serilog")
        {
        }
    }
}