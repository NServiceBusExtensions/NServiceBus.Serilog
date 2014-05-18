using System;
using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;
using Serilog;

namespace NServiceBus.Serilog.Tracing
{
    // ReSharper disable CSharpWarnings::CS0618
    class ReceiveMessageBehavior : IBehavior<ReceiveLogicalMessageContext>
    {
        static ILogger logger = TracingLog.GetLogger("NServiceBus.Serilog.MessageReceived")
                .ForContext("ProcessingEndpoint", Configure.EndpointName);

        public void Invoke(ReceiveLogicalMessageContext context, Action next)
        {
            var logicalMessage = context.LogicalMessage;
            var forContext = logger
                .ForContext("Message", logicalMessage.Instance, true)
                .ForContext("MessageType", logicalMessage.MessageTypeName());
            forContext = forContext.AddHeaders(logicalMessage.Headers);
            forContext.Information("Receive message {MessageType} {MessageId}");
            next();
        }
    }
}
