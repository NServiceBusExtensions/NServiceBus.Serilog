using System;
using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;
using NServiceBus.Settings;

namespace NServiceBus.Serilog.Tracing
{
    class ReceiveMessageBehavior : IBehavior<IncomingContext>
    {
        ReadOnlySettings settings;
        LogBuilder logBuilder;

        public ReceiveMessageBehavior(ReadOnlySettings settings, LogBuilder logBuilder)
        {
            this.settings = settings;
            this.logBuilder = logBuilder;
        }

        public void Invoke(IncomingContext context, Action next)
        {
            var logger = logBuilder.GetLogger("NServiceBus.Serilog.MessageReceived");
            var logicalMessage = context.IncomingLogicalMessage;
            var forContext = logger
                .ForContext("ProcessingEndpoint", settings.EndpointName())
                .ForContext("Message", logicalMessage.Instance, true)
                .ForContext("MessageType", logicalMessage.MessageTypeName());
            forContext = forContext.AddHeaders(logicalMessage.Headers);
            forContext.Information("Receive message {MessageType} {MessageId}");
            next();
        }
    }
}
