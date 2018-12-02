using System;
using System.Collections.Generic;
using Serilog.Events;

public class LogEventEx
{
    public MessageTemplate MessageTemplate;
    public LogEventLevel Level;
    public IReadOnlyDictionary<string, LogEventPropertyValue> Properties;
    public Exception Exception;
}