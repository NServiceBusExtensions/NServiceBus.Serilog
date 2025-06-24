﻿using Serilog.Exceptions;
using TypeNameConverter = NServiceBus.Serilog.TypeNameConverter;

[TestFixture]
public class IntegrationTests
{
    static IntegrationTests()
    {
        var configuration = new LoggerConfiguration();
        var enrich = configuration.Enrich;
        enrich.WithExceptionDetails();
        enrich.WithNsbExceptionDetails();
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

    [Test]
    public async Task Handler()
    {
        Recording.Start();
        await Send(
            new StartHandler
            {
                Property = "TheProperty"
            });
        await Verify();
    }

    [Test]
    public async Task GenericHandler()
    {
        Recording.Start();
        await Send(
            new StartGenericHandler<string>
            {
                Property = "TheProperty"
            });
        await Verify();
    }

    [Test]
    public async Task WithCustomHeader()
    {
        Recording.Start();
        await Send(
            new StartHandler
            {
                Property = "TheProperty"
            },
            options => options.SetHeader("CustomHeader", "CustomValue"));
        await Verify();
    }

    [Test]
    public async Task WithConvertedCustomHeader()
    {
        Recording.Start();
        await Send(
            new StartHandler
            {
                Property = "TheProperty"
            }, options => options.SetHeader("ConvertHeader", "CustomValue"));
        await Verify();
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

    [Test]
    public async Task HandlerThatLogs()
    {
        Recording.Start();
        await Send(new StartHandlerThatLogs());
        await Verify();
    }

    [Test]
    public async Task HandlerThatThrows()
    {
        Recording.Start();
        await Send(
            new StartHandlerThatThrows
            {
                Property = "TheProperty"
            });
        await Verify();
    }

#if Debug

    [Test]
    public async Task Saga()
    {
        var events = await Send(
            new StartSaga
            {
                Property = "TheProperty"
            });
        var logEvents = events.ToList();
        await Verify<StartSaga>(logEvents)
            .ScrubMember("Serilog.SagaStateChange");
    }

#endif

    [Test]
    public async Task BehaviorThatThrows()
    {
        Recording.Start();
        await Send(
            new StartBehaviorThatThrows
            {
                Property = "TheProperty"
            },
            extraConfiguration: _ => _.EnableFeature<BehaviorThatThrowsFeature>());
        await Verify();
    }


    static async Task Send(
        object message,
        Action<SendOptions>? optionsAction = null,
        Action<EndpointConfiguration>? extraConfiguration = null)
    {
        var suffix = TypeNameConverter
            .GetName(message.GetType())
            .MessageTypeName
            .Replace('<', '_')
            .Replace('>', '_');
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
        configuration.RegisterComponents(_ => _.AddSingleton(resetEvent));

        var recoverability = configuration.Recoverability();
        recoverability.Delayed(settings =>
        {
            settings.TimeIncrease(TimeSpan.FromMilliseconds(1));
            settings.NumberOfRetries(1);
        });
        recoverability.Immediate(_ => _.NumberOfRetries(1));

        recoverability.Failed(_ => _
            .OnMessageSentToErrorQueue((_, _) =>
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
    }
}