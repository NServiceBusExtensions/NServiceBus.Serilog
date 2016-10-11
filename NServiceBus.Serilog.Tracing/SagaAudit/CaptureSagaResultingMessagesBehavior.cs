using System.Linq;
using System.Threading.Tasks;
using NServiceBus.Routing;
using System;
using NServiceBus.Pipeline;

namespace NServiceBus.Serilog.Tracing
{

    class CaptureSagaResultingMessagesBehavior : Behavior<IOutgoingLogicalMessageContext>
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
                Destination = GetDestinationForUnicastMessages(context),
                MessageIntent = context.Headers[Headers.MessageIntent]
            };
            sagaUpdatedMessage.ResultingMessages.Add(sagaResultingMessage);
        }

        static string GetDestinationForUnicastMessages(IOutgoingLogicalMessageContext context)
        {
            var sendAddressTags = context.RoutingStrategies.OfType<UnicastRoutingStrategy>().Select(urs => urs.Apply(context.Headers)).Cast<UnicastAddressTag>().ToList();
            if (sendAddressTags.Count != 1)
            {
                return null;
            }
            return sendAddressTags.First().Destination;
        }

        public class Registration : RegisterStep
        {
            public Registration()
                : base("SerilogCaptureSagaResultingMessages", typeof(CaptureSagaResultingMessagesBehavior), "Reports messages outgoing from a saga to Serilog")
            {
               // InsertAfter("InvokeSaga");
            }
        }

    }
}
