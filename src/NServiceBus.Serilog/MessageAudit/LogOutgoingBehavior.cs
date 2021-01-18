using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;

class LogOutgoingBehavior :
    Behavior<IOutgoingPhysicalMessageContext>
{
    bool useFullTypeName;
    ConvertHeader convertHeader;
    MessageTemplate messageTemplate;

    public LogOutgoingBehavior(bool useFullTypeName, ConvertHeader convertHeader)
    {
        this.useFullTypeName = useFullTypeName;
        this.convertHeader = convertHeader;
        MessageTemplateParser templateParser = new();
        messageTemplate = templateParser.Parse("Sent message {OutgoingMessageType} {OutgoingMessageId}.");
    }

    public override Task Invoke(IOutgoingPhysicalMessageContext context, Func<Task> next)
    {
        var message = context.Extensions.Get<OutgoingLogicalMessage>().Instance;
        LogMessage(context, context.Logger(), message);
        return next();
    }

    void LogMessage(IOutgoingPhysicalMessageContext context, ILogger forContext, object message)
    {
        List<LogEventProperty> properties = new();

        if (forContext.BindProperty("OutgoingMessage", message, out var messageProperty))
        {
            properties.Add(messageProperty);
        }

        var addresses = context.UnicastAddresses();
        if (addresses.Count > 0)
        {
            SequenceValue sequence = new(addresses.Select(x => new ScalarValue(x)));
            properties.Add(new("UnicastRoutes", sequence));
        }

        properties.AddRange(forContext.BuildHeaders(useFullTypeName, context.Headers, convertHeader));
        forContext.WriteInfo(messageTemplate, properties);
    }

    public class Registration :
        RegisterStep
    {
        public Registration(bool useFullTypeName, ConvertHeader convertHeader) :
            base(
                stepId: $"Serilog{nameof(LogOutgoingBehavior)}",
                behavior: typeof(LogOutgoingBehavior),
                description: "Logs outgoing messages",
                factoryMethod: _ => new LogOutgoingBehavior(useFullTypeName, convertHeader))
        {
        }
    }
}