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
    class SendMessageBehavior : Behavior<IOutgoingLogicalMessageContext>
    {
        ILogger logger;
        MessageTemplate messageTemplate;

        public SendMessageBehavior(LogBuilder logBuilder)
        {
            var templateParser = new MessageTemplateParser();
            logger = logBuilder.GetLogger("NServiceBus.Serilog.MessageSent");
            messageTemplate = templateParser.Parse("Sent message {MessageType} {MessageId}.");
        }

        public override Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
        {
            IEnumerable<LogEventProperty> properties = new[]
            {
                new LogEventProperty("MessageType", new ScalarValue(context.Message.MessageType)),
                logger.BindProperty("Message", context.Message.Instance),
                logger.BindProperty("MessageId", context.MessageId),
            };
            properties = properties.Concat(logger.BuildHeaders(context.Headers));
            logger.WriteInfo(messageTemplate, properties);
            return next();
        }

        public class Registration : RegisterStep
        {
            public Registration()
                : base("SerilogSendMessage", typeof(SendMessageBehavior), "Logs outgoing messages")
            {
            }
        }
    }
}

