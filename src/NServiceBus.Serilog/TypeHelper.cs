namespace NServiceBus.Serilog
{
    /// <summary>
    /// Extracts the type name from an NServiceBus <see cref="Headers.EnclosedMessageTypes"/> header.
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// Extracts the type name from an NServiceBus <see cref="Headers.EnclosedMessageTypes"/> header.
        /// </summary>
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