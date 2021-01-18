using Serilog.Events;

public delegate LogEventProperty? ConvertHeader(string key, string value);