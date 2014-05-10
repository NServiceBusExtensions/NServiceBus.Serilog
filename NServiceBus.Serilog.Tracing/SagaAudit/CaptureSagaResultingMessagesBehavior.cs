namespace NServiceBus.Serilog.Tracing
{
    using System;
    using System.Linq;
    using Logging;
    using Pipeline;
    using Pipeline.Contexts;

    // ReSharper disable CSharpWarnings::CS0618
    class CaptureSagaResultingMessagesBehavior : IBehavior<SendPhysicalMessageContext>
    {

        static ILog logger = LogManager.GetLogger(typeof(CaptureSagaResultingMessagesBehavior));
        SagaUpdatedMessage sagaUpdatedMessage;

        public void Invoke(SendPhysicalMessageContext context, Action next)
        {
            AppendMessageToState(context);
            next();
        }

        void AppendMessageToState(SendPhysicalMessageContext context)
        {
            if (!context.TryGet(out sagaUpdatedMessage))
            {
                return;
            }
            var messages = context.LogicalMessages.ToList();
            if (messages.Count > 1)
            {
                logger.WarnFormat("Could not audit outgoing messages for the the saga `{0}` since the SagaAuditing plugin does not support batch messages. Consider not using batch messages from this saga.", sagaUpdatedMessage.SagaType);
                return;
            }
            if (messages.Count == 0)
            {
                //this can happen on control messages
                return;
            }
            var logicalMessage = messages.First();
            
            var sagaResultingMessage = new SagaChangeOutput
                {
                    ResultingMessageId = context.MessageToSend.Id,
                    TimeSent = context.TimeSent(),
                    MessageType = logicalMessage.MessageType.ToString(),
                    DeliveryDelay = context.SendOptions.DelayDeliveryWith,
                    DeliveryAt = context.SendOptions.DeliverAt,
                    Destination = context.GetDestination(),
                    Intent = context.SendOptions.Intent.ToString()
                };
            sagaUpdatedMessage.ResultingMessages.Add(sagaResultingMessage);
        }

    }
}
