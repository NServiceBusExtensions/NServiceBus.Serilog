﻿public static class ConfigBuilder
{
    public static EndpointConfiguration BuildDefaultConfig(string endpointName)
    {
        var configuration = new EndpointConfiguration(endpointName);
        configuration.SendFailedMessagesTo("error");
        configuration.UsePersistence<NonDurablePersistence>();
        configuration.UseTransport<LearningTransport>();
        configuration.PurgeOnStartup(true);
        return configuration;
    }
}