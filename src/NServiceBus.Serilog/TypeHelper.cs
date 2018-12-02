static class TypeHelper
{
    public static string GetShortTypeName(string messageType)
    {
        var indexOf = messageType.IndexOf(',');
        if (indexOf == -1)
        {
            return messageType;
        }

        return messageType.Substring(0, indexOf);
    }
}