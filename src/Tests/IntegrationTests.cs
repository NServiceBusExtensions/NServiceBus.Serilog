using Serilog.Exceptions;
using TypeNameConverter = NServiceBus.Serilog.TypeNameConverter;

[UsesVerify]
public class IntegrationTests
{
    static List<LogEvent> logs;

    static IntegrationTests()
    {
        logs = new();
        var eventSink = new EventSink(logs.Add);

        var configuration = new LoggerConfiguration();
        var enrich = configuration.Enrich;
        enrich.WithExceptionDetails();
        enrich.WithNsbExceptionDetails();
        configuration.MinimumLevel.Verbose();
        configuration.WriteTo.Sink(eventSink);
        Log.Logger = configuration.CreateLogger();
        LogManager.Use<SerilogFactory>();
    }

//#if NETCOREAPP3_1
//    [Fact]
//    public async Task WriteStartupDiagnostics()
//    {
//        var events = await Send(
//            new StartHandler
//            {
//                Property = "TheProperty"
//            });
//        await Verify<StartupDiagnostics>(events);
//    }
//#endif

    [Fact]
    public async Task Handler()
    {
        var events = await Send(
            new StartHandler
            {
                Property = "TheProperty"
            });
        await Verify<StartHandler>(events);
    }

    [Fact]
    public async Task GenericHandler()
    {
        var events = await Send(
            new StartGenericHandler<string>
            {
                Property = "TheProperty"
            });
        await Verify<StartGenericHandler<string>>(events);
    }

    [Fact]
    public async Task WithCustomHeader()
    {
        var events = await Send(
            new StartHandler
            {
                Property = "TheProperty"
            },
            options => options.SetHeader("CustomHeader", "CustomValue"));
        await Verify<StartHandler>(events);
    }

    [Fact]
    public async Task WithConvertedCustomHeader()
    {
        var events = await Send(
            new StartHandler
            {
                Property = "TheProperty"
            }, options => options.SetHeader("ConvertHeader", "CustomValue"));
        await Verify<StartHandler>(events);
    }

    //[Fact]
    //public async Task SagaNotFound()
    //{
    //    var events = await Send(
    //        new NotFoundSagaMessage(),
    //        options =>
    //        {
    //            options.SetHeader(Headers.SagaId, Guid.NewGuid().ToString());
    //            options.SetHeader(Headers.SagaType, typeof(NotFoundSaga).FullName);
    //        });
    //    await Verify<NotFoundSagaMessage>(events);
    //}

    [Fact]
    public async Task HandlerThatLogs()
    {
        var events = await Send(new StartHandlerThatLogs());
        await Verify<StartHandlerThatLogs>(events);
    }

    [Fact]
    public async Task HandlerThatThrows()
    {
        var events = await Send(
            new StartHandlerThatThrows
            {
                Property = "TheProperty"
            });
        await Verify<StartHandlerThatThrows>(events)
            .ScrubLinesContaining(
                "Handler start time",
                "Handler failure time");
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
        await Verify<StartSaga>(logEvents);
    }

    [Fact]
    public async Task BehaviorThatThrows()
    {
        var events = await Send(
            new StartBehaviorThatThrows
            {
                Property = "TheProperty"
            },
            extraConfiguration: _ => _.EnableFeature<BehaviorThatThrowsFeature>());
        var logEvents = events.ToList();
        await Verify<StartBehaviorThatThrows>(logEvents);
    }

    static SettingsTask Verify<T>(IEnumerable<LogEventEx> logEvents)
    {
        var list = logEvents.ToList();
        var logsForTarget = list.LogsForType<T>().ToList();
        return Verifier.Verify(
            new
            {
                logsForTarget,
                logsForNsbSerilog = list.LogsForNsbSerilog().ToList(),
                logsWithExceptions = list.LogsWithExceptions().ToList()
            });
    }

    static async Task<IEnumerable<LogEventEx>> Send(
        object message,
        Action<SendOptions>? optionsAction = null,
        Action<EndpointConfiguration>? extraConfiguration = null)
    {
        logs.Clear();
        var suffix = TypeNameConverter.GetName(message.GetType())
            .Replace("<","_")
            .Replace(">","_");
        var configuration = ConfigBuilder.BuildDefaultConfig("SerilogTests" + suffix);
        configuration.PurgeOnStartup(true);
        extraConfiguration?.Invoke(configuration);

        var serilogTracing = configuration.EnableSerilogTracing();
        serilogTracing.EnableSagaTracing();
        serilogTracing.UseHeaderConversion((key, _) =>
        {
            if (key == "ConvertHeader")
            {
                return new("NewKey", new ScalarValue("newValue"));
            }

            return null;
        });
        serilogTracing.EnableMessageTracing();
        var resetEvent = new ManualResetEvent(false);
        configuration.RegisterComponents(components => components.RegisterSingleton(resetEvent));

        var recoverability = configuration.Recoverability();
        recoverability.Delayed(settings =>
        {
            settings.TimeIncrease(TimeSpan.FromMilliseconds(1));
            settings.NumberOfRetries(1);
        });
        recoverability.Immediate(settings => { settings.NumberOfRetries(1); });

        recoverability.Failed(settings => settings
            .OnMessageSentToErrorQueue(_ =>
            {
                resetEvent.Set();
                return Task.CompletedTask;
            }));

        var endpoint = await Endpoint.Start(configuration);
        var sendOptions = new SendOptions();
        optionsAction?.Invoke(sendOptions);
        sendOptions.SetMessageId("00000000-0000-0000-0000-000000000001");
        sendOptions.RouteToThisEndpoint();
        await endpoint.Send(message, sendOptions);
        if (!resetEvent.WaitOne(TimeSpan.FromSeconds(10)))
        {
            throw new("No Set received.");
        }

        await endpoint.Stop();

        return logs.Select(x =>
            new LogEventEx
            (
                messageTemplate: x.MessageTemplate,
                level: x.Level,
                properties: x.Properties,
                exception: x.Exception
            ));
    }
}