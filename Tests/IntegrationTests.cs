using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    public async Task Ensure_handler()
    {
        var events = await Send(new StartHandler {Property = "TheProperty"});
        var logEvents = events.ToList();
        var logsForType = logEvents.LogForType<TheHandler>();
        ObjectApprover.VerifyWithJson(logsForType, jsonSerializerSettings: SerializerBuilder.Get());
    }

    [Fact]
    public async Task Ensure_saga()
    {
        var events = await Send(new StartSaga {Property = "TheProperty"});
        var logEvents = events.ToList();
        var sagaLogEvent = logEvents.LogForType<TheSaga>();
        ObjectApprover.VerifyWithJson(sagaLogEvent, jsonSerializerSettings: SerializerBuilder.Get());
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

        var configuration = new EndpointConfiguration("SerilogTests");
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