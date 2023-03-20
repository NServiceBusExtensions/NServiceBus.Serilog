class ExceptionEnricher :
    ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var exception = logEvent.Exception;
        if (exception is null)
        {
            return;
        }

        if (exception.TryReadData("Message type", out string messageType))
        {
            logEvent.AddPropertyIfAbsent(
                new("IncomingMessageType", new ScalarValue(messageType)));
        }

        if (exception.TryReadData("Message ID", out string incomingMessageId))
        {
            logEvent.AddPropertyIfAbsent(
                new("IncomingMessageId", new ScalarValue(incomingMessageId)));
        }

        if (exception.TryReadData("Transport message ID", out string incomingTransportMessageId))
        {
            logEvent.AddPropertyIfAbsent(
                new("IncomingTransportMessageId", new ScalarValue(incomingTransportMessageId)));
        }

        if (exception.TryReadData("Handler start time", out string handlerStartTime))
        {
            logEvent.AddPropertyIfAbsent(
                new("HandlerStartTime", new ScalarValue(DateTimeOffsetHelper.ToDateTimeOffset(handlerStartTime))));
        }

        if (exception.TryReadData("Handler failure time", out string handlerFailureTime))
        {
            logEvent.AddPropertyIfAbsent(
                new("HandlerFailureTime", new ScalarValue(DateTimeOffsetHelper.ToDateTimeOffset(handlerFailureTime))));
        }

        if (exception.TryReadData("Handler type", out string handlerType))
        {
            logEvent.AddPropertyIfAbsent(new("HandlerType", new ScalarValue(handlerType)));
        }

        if (exception.TryReadData("ExceptionLogState", out ExceptionLogState logState))
        {
            logEvent.AddPropertyIfAbsent(
                new("ProcessingEndpoint", new ScalarValue(logState.ProcessingEndpoint)));
            if (logState.CorrelationId is not null)
            {
                logEvent.AddPropertyIfAbsent(
                    new("CorrelationId", new ScalarValue(logState.CorrelationId)));
            }

            if (logState.ConversationId is not null)
            {
                logEvent.AddPropertyIfAbsent(
                    new("ConversationId", new ScalarValue(logState.ConversationId)));
            }

            if (logState.IncomingMessage is not null)
            {
                if (Log.BindProperty("IncomingMessage", logState.IncomingMessage, true, out var messageProperty))
                {
                    logEvent.AddPropertyIfAbsent(messageProperty);
                }
            }

            if (!logEvent.Properties.ContainsKey("IncomingHeaders"))
            {
                logEvent.AddPropertyIfAbsent(
                    SerilogExtensions.BuildDictionaryProperty("IncomingHeaders",
                        CleanedHeaders(logState.IncomingHeaders)));
            }
        }
    }

    static Dictionary<string, string> CleanedHeaders(IReadOnlyDictionary<string, string> headers)
    {
        var dictionary = new Dictionary<string, string>(headers.Count);
        foreach (var header in headers)
        {
            var key = header.Key;

            if (key == Headers.ConversationId)
            {
                continue;
            }
            if (key == Headers.CorrelationId)
            {
                continue;
            }
            if (key == Headers.EnclosedMessageTypes)
            {
                continue;
            }
            if (key.StartsWith("NServiceBus."))
            {
                key = key[12..];
            }
            else if(key =="$.diagnostics.originating.hostid")
            {
                key = "HostId";
            }

            dictionary.Add(key, header.Value);
        }

        return dictionary;
    }
}