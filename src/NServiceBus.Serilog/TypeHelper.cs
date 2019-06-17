namespace NServiceBus.Serilog
{
    public static class TypeHelper
    {
        public static string GetShortTypeName(string messageType)
        {
            Guard.AgainstNullOrEmpty(messageType, nameof(messageType));
            var indexOf = messageType.IndexOf(',');
            if (indexOf == -1)
            {
                return messageType;
            }

            return messageType.Substring(0, indexOf);
        }
    }
}