/// <summary>
/// Used to support writing a custom log property for a specific header.
/// </summary>
public delegate LogEventProperty? ConvertHeader(string key, string value);