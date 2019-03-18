using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;

class LogOutgoingMessageBehavior : Behavior<IOutgoingLogicalMessageContext>
{
    MessageTemplate messageTemplate;

    public LogOutgoingMessageBehavior()
    {
        var templateParser = new MessageTemplateParser();
        messageTemplate = templateParser.Parse("Sent message {OutgoingMessageType} {OutgoingMessageId}.");
    }

    public override Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        var message = context.Message.Instance;
        LogMessage(context, context.Logger(), message);
        return next();
    }

    void LogMessage(IOutgoingLogicalMessageContext context, ILogger forContext, object message)
    {
        var properties = new List<LogEventProperty>();

        if (forContext.BindProperty("OutgoingMessage", message, out var messageProperty))
        {
            properties.Add(messageProperty);
        }

        var addresses = context.UnicastAddresses();
        if (addresses.Count > 0)
        {
            var sequence = new SequenceValue(addresses.Select(x => new ScalarValue(x)));
            properties.Add(new LogEventProperty("UnicastRoutes", sequence));
        }

        properties.AddRange(forContext.BuildHeaders(context.Headers));
        forContext.WriteInfo(messageTemplate, properties);
    }

    public class Registration : RegisterStep
    {
        public Registration()
            : base(
                stepId: $"Serilog{nameof(LogOutgoingMessageBehavior)}",
                behavior: typeof(LogOutgoingMessageBehavior),
                description: "Logs outgoing messages")
        {
        }
    }
}