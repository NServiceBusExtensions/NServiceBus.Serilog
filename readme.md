# <img src="/src/icon.png" height="30px"> NServiceBus.Serilog

[![Build status](https://ci.appveyor.com/api/projects/status/nmcughyrado8smay/branch/master?svg=true)](https://ci.appveyor.com/project/SimonCropp/nservicebus-Serilog)
[![NuGet Status](https://img.shields.io/nuget/v/NServiceBus.Serilog.svg)](https://www.nuget.org/packages/NServiceBus.Serilog/)

Add support for sending [NServiceBus](http://particular.net/NServiceBus) logging through [Serilog](http://serilog.net/)

<!--- StartOpenCollectiveBackers -->

[Already a Patron? skip past this section](#endofbacking)


## Community backed

**It is expected that all developers either [become a Patron](https://opencollective.com/nservicebusextensions/contribute/patron-6976) or have a [Tidelift Subscription](#support-via-tidelift) to use NServiceBusExtensions. [Go to licensing FAQ](https://github.com/NServiceBusExtensions/Home/#licensingpatron-faq)**


### Sponsors

Support this project by [becoming a Sponsor](https://opencollective.com/nservicebusextensions/contribute/sponsor-6972). The company avatar will show up here with a website link. The avatar will also be added to all GitHub repositories under the [NServiceBusExtensions organization](https://github.com/NServiceBusExtensions).


### Patrons

Thanks to all the backing developers. Support this project by [becoming a patron](https://opencollective.com/nservicebusextensions/contribute/patron-6976).

<img src="https://opencollective.com/nservicebusextensions/tiers/patron.svg?width=890&avatarHeight=60&button=false">

<a href="#" id="endofbacking"></a>

<!--- EndOpenCollectiveBackers -->


## Support via TideLift

Support is available via a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-nservicebus.serilog?utm_source=nuget-nservicebus.serilog&utm_medium=referral&utm_campaign=enterprise).


## NuGet package

https://nuget.org/packages/NServiceBus.Serilog/


## Usage

<!-- snippet: SerilogInCode -->
<a id='snippet-serilogincode'></a>
```cs
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("log.txt")
    .CreateLogger();

LogManager.Use<SerilogFactory>();
```
<sup><a href='/src/Tests/Snippets/Usage.cs#L9-L17' title='Snippet source file'>snippet source</a> | <a href='#snippet-serilogincode' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Filtering

NServiceBus can write a significant amount of information to the log. To limit this information use the filtering features of the underlying logging framework.

For example to limit log output to a specific namespace.

Here is a code configuration example for adding a [Filter](https://github.com/serilog/serilog/wiki/Configuration-Basics#filters).

<!-- snippet: SerilogFiltering -->
<a id='snippet-serilogfiltering'></a>
```cs
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        path:"log.txt",
        restrictedToMinimumLevel: LogEventLevel.Debug
    )
    .Filter.ByIncludingOnly(
        inclusionPredicate: Matching.FromSource("MyNamespace"))
    .CreateLogger();

LogManager.Use<SerilogFactory>();
```
<sup><a href='/src/Tests/Snippets/Filtering.cs#L11-L24' title='Snippet source file'>snippet source</a> | <a href='#snippet-serilogfiltering' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Tracing

Writing diagnostic log entries to [Serilog](https://serilog.net/). Plugs into the low level [pipeline](https://docs.particular.net/nservicebus/pipeline) to give more detailed diagnostics.

When using Serilog for tracing, it is optional to use Serilog as the main NServiceBus logger. i.e. there is no need to include `LogManager.Use<SerilogFactory>();`.


### Create an instance of a Serilog logger

<!-- snippet: SerilogTracingLogger -->
<a id='snippet-serilogtracinglogger'></a>
```cs
var tracingLog = new LoggerConfiguration()
    .WriteTo.File("log.txt")
    .MinimumLevel.Information()
    .CreateLogger();
```
<sup><a href='/src/Tests/Snippets/TracingUsage.cs#L9-L16' title='Snippet source file'>snippet source</a> | <a href='#snippet-serilogtracinglogger' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Configure the tracing feature to use that logger

<!-- snippet: SerilogTracingPassLoggerToFeature -->
<a id='snippet-serilogtracingpassloggertofeature'></a>
```cs
var serilogTracing = configuration.EnableSerilogTracing(tracingLog);
serilogTracing.EnableMessageTracing();
```
<sup><a href='/src/Tests/Snippets/TracingUsage.cs#L20-L25' title='Snippet source file'>snippet source</a> | <a href='#snippet-serilogtracingpassloggertofeature' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Contextual logger

Serilog tracing injects a contextual `Serilog.Ilogger` into the NServiceBus pipeline.

NOTE: Saga and message tracing will use the current contextual logger.

There are several layers of enrichment based on the pipeline phase.


#### Endpoint enrichment

All loggers for an endpoint will have the the property `ProcessingEndpoint` added that contains the current [endpoint name](https://docs.particular.net/nservicebus/endpoints/specify-endpoint-name).


#### Incoming message enrichment

When a message is received, the following enrichment properties are added:

 * [SourceContext](https://github.com/serilog/serilog/wiki/Writing-Log-Events#source-contexts) will be the message type [FullName](https://docs.microsoft.com/de-de/dotnet/api/system.type.fullname) extracted from the [EnclosedMessageTypes header](https://docs.particular.net/nservicebus/messaging/headers#serialization-headers-nservicebus-enclosedmessagetypes). `UnknownMessageType` will be used if no header exists. The same value will be added to a property named `MessageType`.
 * `MessageId` will be the value of the [MessageId header](https://docs.particular.net/nservicebus/messaging/headers#messaging-interaction-headers-nservicebus-messageid).
 * `CorrelationId` will be the value of the [CorrelationId header](https://docs.particular.net/nservicebus/messaging/headers#messaging-interaction-headers-nservicebus-correlationid) if it exists.
 * `ConversationId` will be the value of the [ConversationId header](https://docs.particular.net/nservicebus/messaging/headers#messaging-interaction-headers-nservicebus-conversationid) if it exists.


#### Handler enrichment

When a handler is invoked, a new logger is forked from the above enriched physical logger with a new enriched property named `Handler` that contains the the [FullName](https://docs.microsoft.com/de-de/dotnet/api/system.type.fullname) of the current handler.


#### Outgoing message enrichment

When a message is sent, the same properties as described in "Incoming message enrichment" will be added to the outgoing pipeline. Note that if a handler sends a message, the logger injected into the outgoing pipeline will be forked from the logger instance as described in "Handler enrichment". As such it will contain a property `Handler` for the handler that sent the message.


#### Accessing the logger

The contextual logger instance can be accessed from anywhere in the pipeline via `SerilogTracingExtensions.Logger(this IPipelineContext context)`.

<!-- snippet: ContextualLoggerUsage -->
<a id='snippet-contextualloggerusage'></a>
```cs
public class HandlerUsingLogger :
    IHandleMessages<TheMessage>
{
    public Task Handle(TheMessage message, IMessageHandlerContext context)
    {
        var logger = context.Logger();
        logger.Information("Hello from {@Handler}.");
        return Task.CompletedTask;
    }
}
```
<sup><a href='/src/Tests/Snippets/ContextualLoggerUsage.cs#L4-L16' title='Snippet source file'>snippet source</a> | <a href='#snippet-contextualloggerusage' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Log extension methods

`IPipelineContext` also has extension methods added to expose direct `Log*` methods

<!-- snippet: DirectLogUsage -->
<a id='snippet-directlogusage'></a>
```cs
public class HandlerUsingLog :
    IHandleMessages<TheMessage>
{
    public Task Handle(TheMessage message, IMessageHandlerContext context)
    {
        context.LogInformation("Hello from {@Handler}.");
        return Task.CompletedTask;
    }
}
```
<sup><a href='/src/Tests/Snippets/ContextualLoggerUsage.cs#L18-L29' title='Snippet source file'>snippet source</a> | <a href='#snippet-directlogusage' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Example result of a contextual log entry

<img src="/src/contextualLog.png">


### Exception enrichment

When an exception occurs in the message processing pipeline, the current pipeline state is added to the exception. When that exception is logged that state can be add to the log entry.

When a pipeline exception is logged, it will be enriched with the following properties:

 * `ProcessingEndpoint` will be the current [endpoint name](https://docs.particular.net/nservicebus/endpoints/specify-endpoint-name).
 * `IncomingMessageId` will be the value of the [MessageId header](https://docs.particular.net/nservicebus/messaging/headers#messaging-interaction-headers-nservicebus-messageid).
 * `IncomingTransportMessageId` will be the MessageId from the underlying [transport](https://docs.particular.net/transports/) if it exist.
 * `IncomingHeaders` will be the value of the [Message headers](https://docs.particular.net/nservicebus/messaging/headers).
 * `IncomingMessageType` will be the message type [FullName](https://docs.microsoft.com/de-de/dotnet/api/system.type.fullname) extracted from the [EnclosedMessageTypes header](https://docs.particular.net/nservicebus/messaging/headers#serialization-headers-nservicebus-enclosedmessagetypes). `UnknownMessageType` will be used if no header exists.
 * `CorrelationId` will be the value of the [CorrelationId header](https://docs.particular.net/nservicebus/messaging/headers#messaging-interaction-headers-nservicebus-correlationid) if it exists.
 * `ConversationId` will be the value of the [ConversationId header](https://docs.particular.net/nservicebus/messaging/headers#messaging-interaction-headers-nservicebus-conversationid) if it exists.
 * `HandlerType` will be type name for the current handler if it exists.
 * `IncomingMessage` will be the value of current logical message if it exists.
 * `HandlerStartTime` the UTC timestamp for when the handler started.
 * `HandlerFailureTime` the UTC timestamp for when the handler threw the exception.


### Saga tracing

<!-- snippet: EnableSagaTracing -->
<a id='snippet-enablesagatracing'></a>
```cs
var serilogTracing = configuration.EnableSerilogTracing(logger);
serilogTracing.EnableSagaTracing();
```
<sup><a href='/src/Tests/Snippets/TracingUsage.cs#L30-L35' title='Snippet source file'>snippet source</a> | <a href='#snippet-enablesagatracing' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Example Logs

<!-- snippet: IntegrationTests.Saga.verified.txt -->
<a id='snippet-IntegrationTests.Saga.verified.txt'></a>
```txt
{
  logsForTarget: [
    {
      MessageTemplate: Hello from {@Saga}. Message: {@Message},
      Level: Information,
      Properties: {
        Saga: TheSaga,
        Message: {
          TypeTag: StartSaga,
          Properties: [
            {
              Property: TheProperty
            }
          ]
        },
        Handler: TheSaga,
        IncomingMessageId: Guid_1,
        IncomingMessageType: StartSaga,
        CorrelationId: Guid_1,
        ConversationId: Guid_2,
        SourceContext: StartSaga,
        ProcessingEndpoint: SerilogTestsStartSaga
      }
    },
    {
      MessageTemplate: Receive message {IncomingMessageType} {IncomingMessageId}.,
      Level: Information,
      Properties: {
        IncomingMessage: {
          TypeTag: StartSaga,
          Properties: [
            {
              Property: TheProperty
            }
          ]
        },
        OriginatingHostId: Guid_3,
        MessageIntent: Send,
        OriginatingEndpoint: SerilogTestsStartSaga,
        OriginatingMachine: TheMachineName,
        ReplyToAddress: SerilogTestsStartSaga,
        TimeSent: DateTime_1,
        IncomingMessageId: Guid_1,
        IncomingMessageType: StartSaga,
        CorrelationId: Guid_1,
        ConversationId: Guid_2,
        SourceContext: StartSaga,
        ProcessingEndpoint: SerilogTestsStartSaga
      }
    },
    {
      MessageTemplate: Saga execution {SagaType} {SagaId}.,
      Level: Information,
      Properties: {
        SagaType: TheSaga,
        SagaId: Guid_4,
        StartTime: DateTimeOffset_1,
        FinishTime: DateTimeOffset_2,
        IsCompleted: false,
        IsNew: true,
        Initiator: {
          Elements: {
            "IsSagaTimeout": false,
            "MessageId": Guid_1,
            "OriginatingMachine": TheMachineName,
            "OriginatingEndpoint": SerilogTestsStartSaga,
            "MessageType": StartSaga,
            "TimeSent": DateTime_1,
            "Intent": Send
          }
        },
        ResultingMessages: {
          Elements: [
            {
              Elements: {
                "Id": Guid_5,
                "Type": BackIntoSaga,
                "Intent": Send,
                "Destination": SerilogTestsStartSaga
              }
            }
          ]
        },
        Entity: {
          TypeTag: TheSagaData,
          Properties: [
            {
              Property: TheProperty
            },
            {
              Id: Guid_4
            },
            {
              Originator: SerilogTestsStartSaga
            },
            {
              OriginalMessageId: Guid_1
            }
          ]
        },
        IncomingMessageId: Guid_1,
        IncomingMessageType: StartSaga,
        CorrelationId: Guid_1,
        ConversationId: Guid_2,
        SourceContext: StartSaga,
        ProcessingEndpoint: SerilogTestsStartSaga
      }
    },
    {
      MessageTemplate: Sent message {OutgoingMessageType} {OutgoingMessageId}.,
      Level: Information,
      Properties: {
        OutgoingMessage: {
          TypeTag: StartSaga,
          Properties: [
            {
              Property: TheProperty
            }
          ]
        },
        UnicastRoutes: {
          Elements: [
            SerilogTestsStartSaga
          ]
        },
        OriginatingHostId: Guid_3,
        MessageIntent: Send,
        OriginatingEndpoint: SerilogTestsStartSaga,
        OriginatingMachine: TheMachineName,
        ReplyToAddress: SerilogTestsStartSaga,
        OutgoingMessageId: Guid_1,
        OutgoingMessageType: StartSaga,
        CorrelationId: Guid_1,
        ConversationId: Guid_2,
        SourceContext: StartSaga,
        ProcessingEndpoint: SerilogTestsStartSaga
      }
    },
    {
      MessageTemplate: Sent message {OutgoingMessageType} {OutgoingMessageId}.,
      Level: Information,
      Properties: {
        OutgoingMessage: {
          TypeTag: BackIntoSaga,
          Properties: [
            {
              Property: TheProperty
            }
          ]
        },
        UnicastRoutes: {
          Elements: [
            SerilogTestsStartSaga
          ]
        },
        OriginatingHostId: Guid_3,
        MessageIntent: Send,
        OriginatingEndpoint: SerilogTestsStartSaga,
        OriginatingMachine: TheMachineName,
        OriginatingSagaId: Guid_4,
        OriginatingSagaType: TheSaga,
        RelatedTo: Guid_1,
        ReplyToAddress: SerilogTestsStartSaga,
        OutgoingMessageId: Guid_5,
        OutgoingMessageType: BackIntoSaga,
        CorrelationId: Guid_1,
        ConversationId: Guid_2,
        IncomingMessageId: Guid_1,
        IncomingMessageType: StartSaga,
        SourceContext: StartSaga,
        ProcessingEndpoint: SerilogTestsStartSaga
      }
    }
  ]
}
```
<sup><a href='/src/Tests/IntegrationTests.Saga.verified.txt#L1-L175' title='Snippet source file'>snippet source</a> | <a href='#snippet-IntegrationTests.Saga.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Message tracing

Both incoming and outgoing messages will be logged at the [Information level](https://github.com/serilog/serilog/wiki/Writing-Log-Events#the-role-of-the-information-level). The current message will be included in a property named `Message`. For outgoing messages any unicast routes will be included in a property named `UnicastRoutes`.

<!-- snippet: EnableMessageTracing -->
<a id='snippet-enablemessagetracing'></a>
```cs
var serilogTracing = configuration.EnableSerilogTracing(logger);
serilogTracing.EnableMessageTracing();
```
<sup><a href='/src/Tests/Snippets/TracingUsage.cs#L40-L45' title='Snippet source file'>snippet source</a> | <a href='#snippet-enablemessagetracing' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Example Logs

<!-- snippet: IntegrationTests.Handler.verified.txt -->
<a id='snippet-IntegrationTests.Handler.verified.txt'></a>
```txt
{
  logsForTarget: [
    {
      MessageTemplate: Hello from {@Handler}.,
      Level: Information,
      Properties: {
        Handler: TheHandler,
        IncomingMessageId: Guid_1,
        IncomingMessageType: StartHandler,
        CorrelationId: Guid_1,
        ConversationId: Guid_2,
        SourceContext: StartHandler,
        ProcessingEndpoint: SerilogTestsStartHandler
      }
    },
    {
      MessageTemplate: Receive message {IncomingMessageType} {IncomingMessageId}.,
      Level: Information,
      Properties: {
        IncomingMessage: {
          TypeTag: StartHandler,
          Properties: [
            {
              Property: TheProperty
            }
          ]
        },
        OriginatingHostId: Guid_3,
        MessageIntent: Send,
        OriginatingEndpoint: SerilogTestsStartHandler,
        OriginatingMachine: TheMachineName,
        ReplyToAddress: SerilogTestsStartHandler,
        TimeSent: DateTime_1,
        IncomingMessageId: Guid_1,
        IncomingMessageType: StartHandler,
        CorrelationId: Guid_1,
        ConversationId: Guid_2,
        SourceContext: StartHandler,
        ProcessingEndpoint: SerilogTestsStartHandler
      }
    },
    {
      MessageTemplate: Sent message {OutgoingMessageType} {OutgoingMessageId}.,
      Level: Information,
      Properties: {
        OutgoingMessage: {
          TypeTag: StartHandler,
          Properties: [
            {
              Property: TheProperty
            }
          ]
        },
        UnicastRoutes: {
          Elements: [
            SerilogTestsStartHandler
          ]
        },
        OriginatingHostId: Guid_3,
        MessageIntent: Send,
        OriginatingEndpoint: SerilogTestsStartHandler,
        OriginatingMachine: TheMachineName,
        ReplyToAddress: SerilogTestsStartHandler,
        OutgoingMessageId: Guid_1,
        OutgoingMessageType: StartHandler,
        CorrelationId: Guid_1,
        ConversationId: Guid_2,
        SourceContext: StartHandler,
        ProcessingEndpoint: SerilogTestsStartHandler
      }
    }
  ]
}
```
<sup><a href='/src/Tests/IntegrationTests.Handler.verified.txt#L1-L73' title='Snippet source file'>snippet source</a> | <a href='#snippet-IntegrationTests.Handler.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Startup diagnostics

[Startup diagnostics](https://docs.particular.net/nservicebus/hosting/startup-diagnostics) is, in addition to its default file location, also written to Serilog with the level of `Warning`.

<!-- snippet: WriteStartupDiagnostics -->
<a id='snippet-writestartupdiagnostics'></a>
```cs
class StartupDiagnostics :
    FeatureStartupTask
{
    public StartupDiagnostics(ReadOnlySettings settings, ILogger logger)
    {
        this.settings = settings;
        this.logger = logger.ForContext<StartupDiagnostics>();
    }

    protected override Task OnStart(IMessageSession session)
    {
        var properties = BuildProperties(settings, logger);

        MessageTemplateParser templateParser = new();
        var messageTemplate = templateParser.Parse("DiagnosticEntries");
        LogEvent logEvent = new(
            timestamp: DateTimeOffset.Now,
            level: LogEventLevel.Warning,
            exception: null,
            messageTemplate: messageTemplate,
            properties: properties);
        logger.Write(logEvent);
        return Task.CompletedTask;
    }

    static IEnumerable<LogEventProperty> BuildProperties(
        ReadOnlySettings settings,
        ILogger logger)
    {
        var entries = settings.ReadStartupDiagnosticEntries();
        foreach (var entry in entries)
        {
            if (entry.Name == "Features")
            {
                continue;
            }

            var name = CleanEntry(entry.Name);
            if (logger.BindProperty(name, entry.Data, out var property))
            {
                yield return property!;
            }
        }
    }

    internal static string CleanEntry(string entry)
    {
        if (entry.StartsWith("NServiceBus."))
        {
            return entry.Substring(12);
        }

        return entry;
    }

    protected override Task OnStop(IMessageSession session)
    {
        return Task.CompletedTask;
    }

    ReadOnlySettings settings;
    ILogger logger;
}
```
<sup><a href='/src/NServiceBus.Serilog/StartupDiagnostics/WriteStartupDiagnostics.cs#L11-L77' title='Snippet source file'>snippet source</a> | <a href='#snippet-writestartupdiagnostics' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Logging to Seq

To log to [Seq](https://getseq.net/):

<!-- snippet: SerilogTracingSeq -->
<a id='snippet-serilogtracingseq'></a>
```cs
var tracingLog = new LoggerConfiguration()
    .WriteTo.Seq("http://localhost:5341")
    .MinimumLevel.Information()
    .CreateLogger();
```
<sup><a href='/src/Tests/Snippets/TracingUsage.cs#L50-L57' title='Snippet source file'>snippet source</a> | <a href='#snippet-serilogtracingseq' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Sample

The sample illustrates how to customize logging by configuring Serilog targets and rules.


### Configure Serilog

<!-- snippet: ConfigureSerilog -->
<a id='snippet-configureserilog'></a>
```cs
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
```
<sup><a href='/src/Sample/Program.cs#L8-L12' title='Snippet source file'>snippet source</a> | <a href='#snippet-configureserilog' title='Start of snippet'>anchor</a></sup>
<a id='snippet-configureserilog-1'></a>
```cs
var tracingLog = new LoggerConfiguration()
    .WriteTo.Seq("http://localhost:5341")
    .MinimumLevel.Information()
    .CreateLogger();
var serilogFactory = LogManager.Use<SerilogFactory>();
serilogFactory.WithLogger(tracingLog);
```
<sup><a href='/src/SeqSample/Program.cs#L8-L15' title='Snippet source file'>snippet source</a> | <a href='#snippet-configureserilog-1' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Pass the configuration to NServiceBus

<!-- snippet: UseConfig -->
<a id='snippet-useconfig'></a>
```cs
LogManager.Use<SerilogFactory>();

EndpointConfiguration configuration = new("SerilogSample");
```
<sup><a href='/src/Sample/Program.cs#L14-L19' title='Snippet source file'>snippet source</a> | <a href='#snippet-useconfig' title='Start of snippet'>anchor</a></sup>
<a id='snippet-useconfig-1'></a>
```cs
EndpointConfiguration configuration = new("SeqSample");
var serilogTracing = configuration.EnableSerilogTracing(tracingLog);
serilogTracing.EnableSagaTracing();
serilogTracing.EnableMessageTracing();
```
<sup><a href='/src/SeqSample/Program.cs#L17-L24' title='Snippet source file'>snippet source</a> | <a href='#snippet-useconfig-1' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Ensure logging is flushed on shutdown

<!-- snippet: Cleanup -->
<a id='snippet-cleanup'></a>
```cs
await endpoint.Stop();
Log.CloseAndFlush();
```
<sup><a href='/src/Sample/Program.cs#L29-L32' title='Snippet source file'>snippet source</a> | <a href='#snippet-cleanup' title='Start of snippet'>anchor</a></sup>
<a id='snippet-cleanup-1'></a>
```cs
await endpoint.Stop();
Log.CloseAndFlush();
```
<sup><a href='/src/SeqSample/Program.cs#L40-L43' title='Snippet source file'>snippet source</a> | <a href='#snippet-cleanup-1' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Seq Sample

Illustrates customizing [Serilog](https://serilog.net/) usage to log to [Seq](https://getseq.net/).


### Prerequisites

An instance of [Seq](https://getseq.net/) running one `http://localhost:5341`.


### Configure Serilog

<!-- snippet: ConfigureSerilog -->
<a id='snippet-configureserilog'></a>
```cs
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
```
<sup><a href='/src/Sample/Program.cs#L8-L12' title='Snippet source file'>snippet source</a> | <a href='#snippet-configureserilog' title='Start of snippet'>anchor</a></sup>
<a id='snippet-configureserilog-1'></a>
```cs
var tracingLog = new LoggerConfiguration()
    .WriteTo.Seq("http://localhost:5341")
    .MinimumLevel.Information()
    .CreateLogger();
var serilogFactory = LogManager.Use<SerilogFactory>();
serilogFactory.WithLogger(tracingLog);
```
<sup><a href='/src/SeqSample/Program.cs#L8-L15' title='Snippet source file'>snippet source</a> | <a href='#snippet-configureserilog-1' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Pass that configuration to NServiceBus

<!-- snippet: UseConfig -->
<a id='snippet-useconfig'></a>
```cs
LogManager.Use<SerilogFactory>();

EndpointConfiguration configuration = new("SerilogSample");
```
<sup><a href='/src/Sample/Program.cs#L14-L19' title='Snippet source file'>snippet source</a> | <a href='#snippet-useconfig' title='Start of snippet'>anchor</a></sup>
<a id='snippet-useconfig-1'></a>
```cs
EndpointConfiguration configuration = new("SeqSample");
var serilogTracing = configuration.EnableSerilogTracing(tracingLog);
serilogTracing.EnableSagaTracing();
serilogTracing.EnableMessageTracing();
```
<sup><a href='/src/SeqSample/Program.cs#L17-L24' title='Snippet source file'>snippet source</a> | <a href='#snippet-useconfig-1' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Ensure logging is flushed on shutdown

<!-- snippet: Cleanup -->
<a id='snippet-cleanup'></a>
```cs
await endpoint.Stop();
Log.CloseAndFlush();
```
<sup><a href='/src/Sample/Program.cs#L29-L32' title='Snippet source file'>snippet source</a> | <a href='#snippet-cleanup' title='Start of snippet'>anchor</a></sup>
<a id='snippet-cleanup-1'></a>
```cs
await endpoint.Stop();
Log.CloseAndFlush();
```
<sup><a href='/src/SeqSample/Program.cs#L40-L43' title='Snippet source file'>snippet source</a> | <a href='#snippet-cleanup-1' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Security contact information

To report a security vulnerability, use the [Tidelift security contact](https://tidelift.com/security). Tidelift will coordinate the fix and disclosure.


## Icon

[Brain](https://thenounproject.com/noun/brain/#icon-No10411) designed by [Rémy Médard](https://thenounproject.com/catalarem) from [The Noun Project](https://thenounproject.com).
