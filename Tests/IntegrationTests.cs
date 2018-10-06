using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApprovalTests.Namers;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Serilog;
using ObjectApproval;
using Serilog;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

public class IntegrationTests : TestBase
{
    public static ManualResetEvent resetEvent;

    public IntegrationTests(ITestOutputHelper output) : base(output)
    {
        resetEvent = new ManualResetEvent(false);
    }

    [Fact]
    public async Task Handler()
    {
        var events = await Send(
            new StartHandler
            {
                Property = "TheProperty"
            });
        var logEvents = events.ToList();
        Verify<TheHandler>(logEvents);
    }

    [Fact]
    public async Task Saga()
    {
        var events = await Send(
            new StartSaga
            {
                Property = "TheProperty"
            });
        var logEvents = events.ToList();
        Verify<TheSaga>(logEvents);
    }

    static void Verify<T>(List<LogEvent> logEvents)
    {
        using (ApprovalResults.UniqueForRuntime())
        {
            var logsForTarget = logEvents.LogsForType<T>().ToList();
            ObjectApprover.VerifyWithJson(
                new
                {
                    logsForTarget,
                    logsForNsbSerilog = logEvents.LogsForNsbSerilog().ToList()
                },
                jsonSerializerSettings: SerializerBuilder.Get(), scrubber: s => s.Replace(Environment.MachineName, "MachineName"));
        }
    }

    async Task<IEnumerable<LogEvent>> Send(object message)
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

#if NET472
        var endpointName = "SerilogTestsClassic";
#else
        var endpointName = "SerilogTestsCore";
#endif
        var configuration = new EndpointConfiguration(endpointName);
        configuration.EnableInstallers();
        configuration.EnableSerilogTracing().EnableSagaTracing();

        configuration.SendFailedMessagesTo("error");
        configuration.UsePersistence<InMemoryPersistence>();
        configuration.UseTransport<LearningTransport>();

        var endpoint = await Endpoint.Start(configuration);
        await endpoint.SendLocal(message);
        if (!resetEvent.WaitOne(TimeSpan.FromSeconds(2)))
        {
            throw new Exception("No Set received.");
        }

        await endpoint.Stop();
        Log.CloseAndFlush();
        return logs;
    }
}