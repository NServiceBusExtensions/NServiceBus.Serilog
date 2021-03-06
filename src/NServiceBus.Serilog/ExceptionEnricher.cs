﻿using NServiceBus;
using Serilog;
using Serilog.Core;
using Serilog.Events;

class ExceptionEnricher :
    ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var exception = logEvent.Exception;
        if (exception == null)
        {
            return;
        }

        if (exception.TryReadData("Message type", out string messageType))
        {
            logEvent.AddPropertyIfAbsent(new("IncomingMessageType", new ScalarValue(messageType)));
        }

        if (exception.TryReadData("Message ID", out string incomingMessageId))
        {
            logEvent.AddPropertyIfAbsent(new("IncomingMessageId", new ScalarValue(incomingMessageId)));
        }

        if (exception.TryReadData("Transport message ID", out string incomingTransportMessageId))
        {
            logEvent.AddPropertyIfAbsent(new("IncomingTransportMessageId", new ScalarValue(incomingTransportMessageId)));
        }

        if (exception.TryReadData("Handler start time", out string handlerStartTime))
        {
            logEvent.AddPropertyIfAbsent(new("HandlerStartTime", new ScalarValue(DateTimeExtensions.ToUtcDateTime(handlerStartTime))));
        }

        if (exception.TryReadData("Handler failure time", out string handlerFailureTime))
        {
            logEvent.AddPropertyIfAbsent(new("HandlerFailureTime", new ScalarValue(DateTimeExtensions.ToUtcDateTime(handlerFailureTime))));
        }

        if (exception.TryReadData("Handler type", out string handlerType))
        {
            logEvent.AddPropertyIfAbsent(new("HandlerType", new ScalarValue(handlerType)));
        }

        if (exception.TryReadData("ExceptionLogState", out ExceptionLogState logState))
        {
            logEvent.AddPropertyIfAbsent(new("ProcessingEndpoint", new ScalarValue(logState.ProcessingEndpoint)));
            if (logState.CorrelationId != null)
            {
                logEvent.AddPropertyIfAbsent(new("CorrelationId", new ScalarValue(logState.CorrelationId)));
            }

            if (logState.ConversationId != null)
            {
                logEvent.AddPropertyIfAbsent(new("ConversationId", new ScalarValue(logState.ConversationId)));
            }

            if (logState.IncomingMessage != null)
            {
                if (Log.BindProperty("IncomingMessage", logState.IncomingMessage, true, out var messageProperty))
                {
                    logEvent.AddPropertyIfAbsent(messageProperty);
                }
            }

            if (Log.BindProperty("IncomingHeaders", logState.IncomingHeaders, true, out var headersProperty))
            {
                logEvent.AddPropertyIfAbsent(headersProperty);
            }
        }
    }
}