static class TypeHelper
{
    public static string GetShortTypeName(string messageType) => messageType.Substring(0, messageType.IndexOf(","));
}