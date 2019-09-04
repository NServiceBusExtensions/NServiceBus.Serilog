using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Serilog;
using ObjectApproval;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Xunit;
using Xunit.Abstractions;

public class IntegrationTests : TestBase
{
    public static ManualResetEvent resetEvent;

    public IntegrationTests(ITestOutputHelper output) : base(output)
    {
        HeaderAppender.excludeHeaders.Add(Headers.TimeSent);
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
        Verify<StartHandler>(logEvents);
    }

    [Fact]
    public async Task SagaNotFound()
    {
        var events = await Send(
            new NotFoundSagaMessage(),
            options =>
            {
                options.SetHeader(Headers.SagaId,Guid.NewGuid().ToString());
                options.SetHeader(Headers.SagaType,typeof(TheSaga).FullName);
            });
        var logEvents = events.ToList();
        Verify<NotFoundSagaMessage>(logEvents);
    }

    [Fact]
    public async Task HandlerThatLogs()
    {
        var events = await Send(new StartHandlerThatLogs());
        var logEvents = events.ToList();
        Verify<StartHandlerThatLogs>(logEvents);
    }

    [Fact]
    public async Task HandlerThatThrows()
    {
        var events = await Send(
            new StartHandlerThatThrows
            {
                Property = "TheProperty"
            });
        var logEvents = events.ToList();
        Verify<StartHandlerThatThrows>(logEvents);
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
        Verify<StartSaga>(logEvents);
    }

    static void Verify<T>(List<LogEventEx> logEvents)
    {
        var logsForTarget = logEvents.LogsForType<T>().ToList();
        var nsbVersion = FileVersionInfo.GetVersionInfo(typeof(Endpoint).Assembly.Location);
        var nsbVersionString = $"{nsbVersion.FileMajorPart}.{nsbVersion.FileMinorPart}.{nsbVersion.FileBuildPart}";
        ObjectApprover.Verify(
            new
            {
                logsForTarget,
                logsForNsbSerilog = logEvents.LogsForNsbSerilog().ToList(),
                logsWithExceptions = logEvents.LogsWithExceptions().ToList()
            },
            jsonSerializerSettings: null,
            scrubber: s => s
                .Replace(Environment.MachineName, "MachineName")
                .Replace(nsbVersionString, "NsbVersion")
                .RemoveLinesContaining("StackTraceString"));
    }

    static async Task<IEnumerable<LogEventEx>> Send(object message, Action<SendOptions> optionsAction = null)
    {
        var logs = new List<LogEvent>();
        var eventSink = new EventSink
        {
            Action = logs.Add
        };

        var loggerConfiguration = new LoggerConfiguration();
        loggerConfiguration.Enrich.WithExceptionDetails();
        loggerConfiguration.MinimumLevel.Verbose();
        loggerConfiguration.WriteTo.Sink(eventSink);
        Log.Logger = loggerConfiguration.CreateLogger();
        LogManager.Use<SerilogFactory>();

        var configuration = ConfigBuilder.BuildDefaultConfig("SerilogTests"+message.GetType().Name);
        var serilogTracing = configuration.EnableSerilogTracing();
        serilogTracing.EnableSagaTracing();
        serilogTracing.EnableMessageTracing();

        var recoverability = configuration.Recoverability();
        recoverability.Delayed(settings =>
        {
            settings.TimeIncrease(TimeSpan.FromMilliseconds(1));
            settings.NumberOfRetries(1);
        });
        recoverability.Immediate(settings => { settings.NumberOfRetries(1); });

        configuration.Notifications.Errors.MessageSentToErrorQueue +=
            (sender, retry) => { resetEvent.Set(); };

        var endpoint = await Endpoint.Start(configuration);
        var sendOptions = new SendOptions();
        optionsAction?.Invoke(sendOptions);
        sendOptions.SetMessageId("00000000-0000-0000-0000-000000000001");
        sendOptions.RouteToThisEndpoint();
        await endpoint.Send(message, sendOptions);
        if (!resetEvent.WaitOne(TimeSpan.FromSeconds(5)))
        {
            throw new Exception("No Set received.");
        }

        await endpoint.Stop();
        Log.CloseAndFlush();

        return logs.Select(x =>
            new LogEventEx
            {
                MessageTemplate = x.MessageTemplate,
                Level = x.Level,
                Properties = x.Properties,
                Exception = x.Exception,
            });
    }
}