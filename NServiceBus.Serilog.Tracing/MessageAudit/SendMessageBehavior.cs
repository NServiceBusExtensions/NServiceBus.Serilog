using System;
using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;

namespace NServiceBus.Serilog.Tracing
{
    // wrap DispatchMessageToTransportBehavior
    class SendMessageBehavior : IBehavior<OutgoingContext>
    {
        LogBuilder logBuilder;

        public SendMessageBehavior(LogBuilder logBuilder)
        {
            this.logBuilder = logBuilder;
        }

        public void Invoke(OutgoingContext context, Action next)
        {
            var logger = logBuilder.GetLogger("NServiceBus.Serilog.MessageSent");
            var message = context.OutgoingLogicalMessage;
            var forContext = logger
                .ForContext("Message", message.Instance, true)
                .ForContext("MessageType", message.MessageTypeName())
                .ForContext("MessageId", context.OutgoingMessage.Id);
            forContext = forContext.AddHeaders(context.OutgoingLogicalMessage.Headers);

            forContext.Information("Sent message {MessageType} {MessageId}");
            next();
        }
    }
}

