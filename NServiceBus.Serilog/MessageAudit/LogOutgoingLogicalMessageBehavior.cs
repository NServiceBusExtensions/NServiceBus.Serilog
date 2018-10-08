using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;

class LogOutgoingLogicalMessageBehavior : Behavior<IOutgoingLogicalMessageContext>
{
    MessageTemplate messageTemplate;

    public LogOutgoingLogicalMessageBehavior()
    {
        var templateParser = new MessageTemplateParser();
        messageTemplate = templateParser.Parse("Sent message {MessageType} {MessageId}.");
    }

    public override Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        var message = context.Message.Instance;
        LogMessage(context, context.Logger(), message);
        return next();
    }

    void LogMessage(IOutgoingContext context, ILogger forContext, object message)
    {
        var logProperties = new List<LogEventProperty>();

        if (forContext.BindProperty("Message", message, out var messageProperty))
        {
            logProperties.Add(messageProperty);
        }

        logProperties.AddRange(forContext.BuildHeaders(context.Headers));
        forContext.WriteInfo(messageTemplate, logProperties);
    }

    public class Registration : RegisterStep
    {
        public Registration()
            : base(
                stepId: $"Serilog{nameof(LogOutgoingLogicalMessageBehavior)}",
                behavior: typeof(LogOutgoingLogicalMessageBehavior),
                description: "Logs outgoing messages",
                factoryMethod: builder => new LogOutgoingLogicalMessageBehavior())
        {
        }
    }
}