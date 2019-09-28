using System;
using Serilog.Core;
using Serilog.Events;

public class EventSink : 
    ILogEventSink
{
    Action<LogEvent> action;

    public EventSink(Action<LogEvent> action)
    {
        this.action = action;
    }

    public void Emit(LogEvent logEvent)
    {
        action(logEvent);
    }
}