using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Serilog;
using NUnit.Framework;
using Serilog;
using Serilog.Events;

[TestFixture]
[Explicit]
public class IntegrationTests
{
    [Test]
    public async Task Ensure_log_messages_are_redirected()
    {
        var logs = new ConcurrentBag<LogEvent>();
        var eventSink = new EventSink
        {
            Action = logs.Add
        };

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(eventSink)
            .CreateLogger();
        LogManager.Use<SerilogFactory>();

        var endpointConfiguration = new EndpointConfiguration("SerilogTests");
        endpointConfiguration.UseSerialization<JsonSerializer>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.UsePersistence<InMemoryPersistence>();

        var endpoint = await Endpoint.Start(endpointConfiguration);
        try
        {
            Assert.IsNotEmpty(logs);
        }
        finally
        {
            await endpoint.Stop();
        }
    }

    [Test]
    public async Task Use_default_logger_even_if_it_gets_changed()
    {
        LogManager.Use<SerilogFactory>();

        var logs = new ConcurrentBag<LogEvent>();
        var eventSink = new EventSink
        {
            Action = logs.Add
        };

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(eventSink)
            .CreateLogger();

        var endpointConfiguration = new EndpointConfiguration("SerilogTests");
        endpointConfiguration.UseSerialization<JsonSerializer>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.UsePersistence<InMemoryPersistence>();

        var endpoint = await Endpoint.Start(endpointConfiguration);
        try
        {
            Assert.IsNotEmpty(logs);
        }
        finally
        {
            await endpoint.Stop();
        }

    }
}