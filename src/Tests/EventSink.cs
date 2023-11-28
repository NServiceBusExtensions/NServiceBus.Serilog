public class EventSink(Action<LogEvent> action) :
    ILogEventSink
{
    public void Emit(LogEvent logEvent) =>
        action(logEvent);
}