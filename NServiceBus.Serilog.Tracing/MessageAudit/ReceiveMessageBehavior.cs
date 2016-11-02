using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;

namespace NServiceBus.Serilog.Tracing
{
    class ReceiveMessageBehavior : Behavior<IIncomingLogicalMessageContext>
    {
        MessageTemplate messageTemplate;
        ILogger logger;

        public ReceiveMessageBehavior(LogBuilder logBuilder)
        {
            var templateParser = new MessageTemplateParser();
            messageTemplate = templateParser.Parse("Receive message {MessageType} {MessageId}.");
            logger = logBuilder.GetLogger("NServiceBus.Serilog.MessageReceived");
        }

        public class Registration : RegisterStep
        {
            public Registration()
                : base("SerilogReceiveMessage", typeof(ReceiveMessageBehavior), "Logs incoming messages")
            {
                InsertBefore("MutateIncomingMessages");
            }
        }

        public override Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            var message = context.Message;
            IEnumerable<LogEventProperty> properties = new[]
            {
                new LogEventProperty("MessageType", new ScalarValue(message.MessageType)),
                logger.BindProperty("Message", message.Instance),
                logger.BindProperty("MessageId", context.MessageId),
            };
            properties = properties.Concat(logger.BuildHeaders(context.Headers));
            logger.WriteInfo(messageTemplate, properties);
            return next();
        }
    }
}