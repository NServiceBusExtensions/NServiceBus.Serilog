using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Serilog;
using Serilog;
using Serilog.Events;
using Xunit;

public class IntegrationTests
{
    [Fact]
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

        var configuration = new EndpointConfiguration("SerilogTests");
        configuration.EnableInstallers();
        configuration.SendFailedMessagesTo("error");
        configuration.UsePersistence<InMemoryPersistence>();
        configuration.UseTransport<LearningTransport>();

        var endpoint = await Endpoint.Start(configuration);
        Assert.NotEmpty(logs);
        await endpoint.Stop();
    }
}