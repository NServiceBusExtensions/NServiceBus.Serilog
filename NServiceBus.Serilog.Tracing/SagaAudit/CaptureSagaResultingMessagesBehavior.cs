namespace NServiceBus.Serilog.Tracing
{
    using System;
    using NServiceBus.Unicast;
    using Pipeline;
    using Pipeline.Contexts;

    class CaptureSagaResultingMessagesBehavior : IBehavior<OutgoingContext>
    {
        SagaUpdatedMessage sagaUpdatedMessage;

        public void Invoke(OutgoingContext context, Action next)
        {
            AppendMessageToState(context);
            next();
        }

        void AppendMessageToState(OutgoingContext context)
        {
            if (!context.TryGet(out sagaUpdatedMessage))
            {
                return;
            }

            var logicalMessage = context.OutgoingLogicalMessage;
            if (logicalMessage == null)
            {
                //this can happen on control messages
                return;
            }
            
            var sendOptions = context.DeliveryOptions as SendOptions;
            if (sendOptions != null)
            {
                var sagaResultingMessage = new SagaChangeOutput
                {
                    ResultingMessageId = context.OutgoingMessage.Id,
                    MessageType = logicalMessage.MessageType.ToString(),
                    Destination = sendOptions.Destination.ToString(),
                    MessageIntent = "Send"
                };
                sagaUpdatedMessage.ResultingMessages.Add(sagaResultingMessage);
            }
            if (context.DeliveryOptions is PublishOptions)
            {
                var sagaResultingMessage = new SagaChangeOutput
                {
                    ResultingMessageId = context.OutgoingMessage.Id,
                    MessageType = logicalMessage.MessageType.ToString(),
                    MessageIntent = "Publish"
                };
                sagaUpdatedMessage.ResultingMessages.Add(sagaResultingMessage);
            }
        }

    }
}
