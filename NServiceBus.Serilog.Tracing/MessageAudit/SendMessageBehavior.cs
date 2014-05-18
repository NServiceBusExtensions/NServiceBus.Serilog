using System;
using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;
using Serilog;

namespace NServiceBus.Serilog.Tracing
{
    // ReSharper disable CSharpWarnings::CS0618
    class SendMessageBehavior : IBehavior<SendPhysicalMessageContext>
    {
        static ILogger logger = TracingLog.GetLogger("NServiceBus.Serilog.MessageSent")
                .ForContext("SendingEndpoint", Configure.EndpointName);

        public void Invoke(SendPhysicalMessageContext context, Action next)
        {
            foreach (var message in context.LogicalMessages)
            {

                var logicalMessage = context.MessageToSend;
                var forContext = logger
                    .ForContext("Message", message.Instance, true)
                    .ForContext("MessageType", message.MessageTypeName())
                    .ForContext("MessageId", context.MessageToSend.Id);
                forContext = forContext.AddHeaders(logicalMessage.Headers);

                forContext.Information("Sent message {MessageType} {MessageId}");
            }
            next();
        }
    }
}

