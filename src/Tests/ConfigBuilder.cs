using NServiceBus;

public static class ConfigBuilder
{
    public static EndpointConfiguration BuildDefaultConfig(string endpointName)
    {
        EndpointConfiguration configuration = new(endpointName);
        configuration.SendFailedMessagesTo("error");
        configuration.UsePersistence<InMemoryPersistence>();
        configuration.UseTransport<LearningTransport>();
        configuration.PurgeOnStartup(true);
        return configuration;
    }
}