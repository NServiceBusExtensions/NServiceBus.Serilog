class ConfigurationException :
    Exception
{
    public ConfigurationException(string message) :
        base(message)
    {
    }

    public override string ToString() =>
        Message;
}