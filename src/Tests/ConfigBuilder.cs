public static class ConfigBuilder
{
    public static EndpointConfiguration BuildDefaultConfig(string endpointName)
    {
        var configuration = new EndpointConfiguration(endpointName);
        configuration.SendFailedMessagesTo("error");
        configuration.UsePersistence<NonDurablePersistence>();
        configuration.UseTransport<LearningTransport>();
        configuration.PurgeOnStartup(true);
        configuration.UseSerialization<SystemJsonSerializer>();
        configuration.AssemblyScanner()
            .ExcludeAssemblies("xunit.runner.utility.netcoreapp10.dll");
        return configuration;
    }
}