<!--
GENERATED FILE - DO NOT EDIT
This file was generated by [MarkdownSnippets](https://github.com/SimonCropp/MarkdownSnippets).
Source File: /readme.source.md
To change this file edit the source file and then run MarkdownSnippets.
-->

# <img src="/src/icon.png" height="30px"> NServiceBus.Serilog

[![Build status](https://ci.appveyor.com/api/projects/status/nmcughyrado8smay/branch/master?svg=true)](https://ci.appveyor.com/project/SimonCropp/nservicebus-Serilog)
[![NuGet Status](https://img.shields.io/nuget/v/NServiceBus.Serilog.svg?cacheSeconds=86400)](https://www.nuget.org/packages/NServiceBus.Serilog/)

Add support for sending [NServiceBus](http://particular.net/NServiceBus) logging through [Serilog](http://serilog.net/)

<!-- toc -->
## Contents

  * [Community backed](#community-backed)
    * [Sponsors](#sponsors)
    * [Patrons](#patrons)
  * [Usage](#usage)
  * [Filtering](#filtering)
  * [Tracing](#tracing)
    * [Create an instance of a Serilog logger](#create-an-instance-of-a-serilog-logger)
    * [Configure the tracing feature to use that logger](#configure-the-tracing-feature-to-use-that-logger)
    * [Contextual logger](#contextual-logger)
    * [Exception enrichment](#exception-enrichment)
    * [Saga tracing](#saga-tracing)
    * [Message tracing](#message-tracing)
    * [Startup diagnostics](#startup-diagnostics)
  * [Logging to Seq](#logging-to-seq)
  * [Sample](#sample)
    * [Configure Serilog](#configure-serilog)
    * [Pass the configuration to NServiceBus](#pass-the-configuration-to-nservicebus)
    * [Ensure logging is flushed on shutdown](#ensure-logging-is-flushed-on-shutdown)
  * [Seq Sample](#seq-sample)
    * [Prerequisites](#prerequisites)
    * [Configure Serilog](#configure-serilog-1)
    * [Pass that configuration to NServiceBus](#pass-that-configuration-to-nservicebus)
    * [Ensure logging is flushed on shutdown](#ensure-logging-is-flushed-on-shutdown-1)<!-- endtoc -->

<!--- StartOpenCollectiveBackers -->

[Already a Patron? skip past this section](#endofbacking)


## Community backed

**It is expected that all developers [become a Patron](https://opencollective.com/nservicebusextensions/order/6976) to use any of these libraries. [Go to licensing FAQ](https://github.com/NServiceBusExtensions/Home/blob/master/readme.md#licensingpatron-faq)**


### Sponsors

Support this project by [becoming a Sponsors](https://opencollective.com/nservicebusextensions/order/6972). The company avatar will show up here with a link to your website. The avatar will also be added to all GitHub repositories under this organization.


### Patrons

Thanks to all the backing developers! Support this project by [becoming a patron](https://opencollective.com/nservicebusextensions/order/6976).

<img src="https://opencollective.com/nservicebusextensions/tiers/patron.svg?width=890&avatarHeight=60&button=false">

<!--- EndOpenCollectiveBackers -->

<a href="#" id="endofbacking"></a>


## Usage

<!-- snippet: SerilogInCode -->
<a id='snippet-serilogincode'/></a>
```cs
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("log.txt")
    .CreateLogger();

LogManager.Use<SerilogFactory>();
```
<sup><a href='/src/Tests/Snippets/Usage.cs#L9-L17' title='File snippet `serilogincode` was extracted from'>snippet source</a> | <a href='#snippet-serilogincode' title='Navigate to start of snippet `serilogincode`'>anchor</a></sup>
<!-- endsnippet -->


## Filtering

NServiceBus can write a significant amount of information to the log. To limit this information use the filtering features of the underlying logging framework.

For example to limit log output to a specific namespace.

Here is a code configuration example for adding a [Filter](https://github.com/serilog/serilog/wiki/Configuration-Basics#filters).

<!-- snippet: SerilogFiltering -->
<a id='snippet-serilogfiltering'/></a>
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
<sup><a href='/src/Tests/Snippets/Filtering.cs#L11-L24' title='File snippet `serilogfiltering` was extracted from'>snippet source</a> | <a href='#snippet-serilogfiltering' title='Navigate to start of snippet `serilogfiltering`'>anchor</a></sup>
<!-- endsnippet -->


## Tracing

Writing diagnostic log entries to [Serilog](https://serilog.net/). Plugs into the low level [pipeline](https://docs.particular.net/nservicebus/pipeline) to give more detailed diagnostics.

When using Serilog for tracing, it is optional to use Serilog as the main NServiceBus logger. i.e. there is no need to include `LogManager.Use<SerilogFactory>();`.


### Create an instance of a Serilog logger

<!-- snippet: SerilogTracingLogger -->
<a id='snippet-serilogtracinglogger'/></a>
```cs
var tracingLog = new LoggerConfiguration()
    .WriteTo.File("log.txt")
    .MinimumLevel.Information()
    .CreateLogger();
```
<sup><a href='/src/Tests/Snippets/TracingUsage.cs#L9-L16' title='File snippet `serilogtracinglogger` was extracted from'>snippet source</a> | <a href='#snippet-serilogtracinglogger' title='Navigate to start of snippet `serilogtracinglogger`'>anchor</a></sup>
<!-- endsnippet -->


### Configure the tracing feature to use that logger

<!-- snippet: SerilogTracingPassLoggerToFeature -->
<a id='snippet-serilogtracingpassloggertofeature'/></a>
```cs
var serilogTracing = endpointConfiguration.EnableSerilogTracing(tracingLog);
serilogTracing.EnableMessageTracing();
```
<sup><a href='/src/Tests/Snippets/TracingUsage.cs#L20-L25' title='File snippet `serilogtracingpassloggertofeature` was extracted from'>snippet source</a> | <a href='#snippet-serilogtracingpassloggertofeature' title='Navigate to start of snippet `serilogtracingpassloggertofeature`'>anchor</a></sup>
<!-- endsnippet -->


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
<a id='snippet-contextualloggerusage'/></a>
```cs
public class SimpleHandler :
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
<sup><a href='/src/Tests/Snippets/ContextualLoggerUsage.cs#L4-L16' title='File snippet `contextualloggerusage` was extracted from'>snippet source</a> | <a href='#snippet-contextualloggerusage' title='Navigate to start of snippet `contextualloggerusage`'>anchor</a></sup>
<!-- endsnippet -->


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
<a id='snippet-enablesagatracing'/></a>
```cs
var serilogTracing = endpointConfiguration.EnableSerilogTracing(logger);
serilogTracing.EnableSagaTracing();
```
<sup><a href='/src/Tests/Snippets/TracingUsage.cs#L30-L35' title='File snippet `enablesagatracing` was extracted from'>snippet source</a> | <a href='#snippet-enablesagatracing' title='Navigate to start of snippet `enablesagatracing`'>anchor</a></sup>
<!-- endsnippet -->


### Message tracing

Both incoming and outgoing messages will be logged at the [Information level](https://github.com/serilog/serilog/wiki/Writing-Log-Events#the-role-of-the-information-level). The current message will be included in a property named `Message`. For outgoing messages any unicast routes will be included in a property named `UnicastRoutes`.

<!-- snippet: EnableMessageTracing -->
<a id='snippet-enablemessagetracing'/></a>
```cs
var serilogTracing = endpointConfiguration.EnableSerilogTracing(logger);
serilogTracing.EnableMessageTracing();
```
<sup><a href='/src/Tests/Snippets/TracingUsage.cs#L40-L45' title='File snippet `enablemessagetracing` was extracted from'>snippet source</a> | <a href='#snippet-enablemessagetracing' title='Navigate to start of snippet `enablemessagetracing`'>anchor</a></sup>
<!-- endsnippet -->


### Startup diagnostics

[Startup diagnostics](https://docs.particular.net/nservicebus/hosting/startup-diagnostics) is, in addition to its default file location, also written to Serilog with the level of `Warning`.

<!-- snippet: WriteStartupDiagnostics -->
<a id='snippet-writestartupdiagnostics'/></a>
```cs
class WriteStartupDiagnostics :
    FeatureStartupTask
{
    public WriteStartupDiagnostics(ReadOnlySettings settings, ILogger logger)
    {
        this.settings = settings;
        this.logger = logger;
    }

    protected override Task OnStart(IMessageSession session)
    {
        var properties = BuildProperties(settings, logger);

        var templateParser = new MessageTemplateParser();
        var messageTemplate = templateParser.Parse("DiagnosticEntries");
        var logEvent = new LogEvent(
            timestamp: DateTimeOffset.Now,
            level: LogEventLevel.Warning,
            exception: null,
            messageTemplate: messageTemplate,
            properties: properties);
        logger.Write(logEvent);
        return Task.CompletedTask;
    }

    static IEnumerable<LogEventProperty> BuildProperties(ReadOnlySettings readOnlySettings, ILogger logger)
    {
        var entries = readOnlySettings.ReadStartupDiagnosticEntries();
        foreach (var entry in entries)
        {
            if (entry.Name == "Features")
            {
                continue;
            }
            if (logger.BindProperty(entry.Name, entry.Data, out var property))
            {
                yield return property!;
            }
        }
    }

    protected override Task OnStop(IMessageSession session)
    {
        return Task.CompletedTask;
    }

    ReadOnlySettings settings;
    private readonly ILogger logger;
}
```
<sup><a href='/src/NServiceBus.Serilog/StartupDiagnostics/WriteStartupDiagnostics.cs#L11-L61' title='File snippet `writestartupdiagnostics` was extracted from'>snippet source</a> | <a href='#snippet-writestartupdiagnostics' title='Navigate to start of snippet `writestartupdiagnostics`'>anchor</a></sup>
<!-- endsnippet -->


## Logging to Seq

To log to [Seq](https://getseq.net/):

<!-- snippet: SerilogTracingSeq -->
<a id='snippet-serilogtracingseq'/></a>
```cs
var tracingLog = new LoggerConfiguration()
    .WriteTo.Seq("http://localhost:5341")
    .MinimumLevel.Information()
    .CreateLogger();
```
<sup><a href='/src/Tests/Snippets/TracingUsage.cs#L50-L57' title='File snippet `serilogtracingseq` was extracted from'>snippet source</a> | <a href='#snippet-serilogtracingseq' title='Navigate to start of snippet `serilogtracingseq`'>anchor</a></sup>
<!-- endsnippet -->


## Sample

The sample illustrates how to customize logging by configuring Serilog targets and rules.


### Configure Serilog

<!-- snippet: ConfigureSerilog -->
<a id='snippet-configureserilog'/></a>
```cs
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
```
<sup><a href='/src/Sample/Program.cs#L13-L17' title='File snippet `configureserilog` was extracted from'>snippet source</a> | <a href='#snippet-configureserilog' title='Navigate to start of snippet `configureserilog`'>anchor</a></sup>
<a id='snippet-configureserilog-1'/></a>
```cs
var tracingLog = new LoggerConfiguration()
    .WriteTo.Seq("http://localhost:5341")
    .MinimumLevel.Information()
    .CreateLogger();
var serilogFactory = LogManager.Use<SerilogFactory>();
serilogFactory.WithLogger(tracingLog);
```
<sup><a href='/src/SeqSample/Program.cs#L13-L20' title='File snippet `configureserilog` was extracted from'>snippet source</a> | <a href='#snippet-configureserilog-1' title='Navigate to start of snippet `configureserilog`'>anchor</a></sup>
<!-- endsnippet -->


### Pass the configuration to NServiceBus

<!-- snippet: UseConfig -->
<a id='snippet-useconfig'/></a>
```cs
LogManager.Use<SerilogFactory>();

var configuration = new EndpointConfiguration("SerilogSample");
```
<sup><a href='/src/Sample/Program.cs#L19-L24' title='File snippet `useconfig` was extracted from'>snippet source</a> | <a href='#snippet-useconfig' title='Navigate to start of snippet `useconfig`'>anchor</a></sup>
<a id='snippet-useconfig-1'/></a>
```cs
var configuration = new EndpointConfiguration("SeqSample");
var serilogTracing = configuration.EnableSerilogTracing(tracingLog);
serilogTracing.EnableSagaTracing();
serilogTracing.EnableMessageTracing();
```
<sup><a href='/src/SeqSample/Program.cs#L22-L29' title='File snippet `useconfig` was extracted from'>snippet source</a> | <a href='#snippet-useconfig-1' title='Navigate to start of snippet `useconfig`'>anchor</a></sup>
<!-- endsnippet -->


### Ensure logging is flushed on shutdown

<!-- snippet: Cleanup -->
<a id='snippet-cleanup'/></a>
```cs
await endpoint.Stop();
Log.CloseAndFlush();
```
<sup><a href='/src/Sample/Program.cs#L34-L37' title='File snippet `cleanup` was extracted from'>snippet source</a> | <a href='#snippet-cleanup' title='Navigate to start of snippet `cleanup`'>anchor</a></sup>
<a id='snippet-cleanup-1'/></a>
```cs
await endpoint.Stop();
Log.CloseAndFlush();
```
<sup><a href='/src/SeqSample/Program.cs#L45-L48' title='File snippet `cleanup` was extracted from'>snippet source</a> | <a href='#snippet-cleanup-1' title='Navigate to start of snippet `cleanup`'>anchor</a></sup>
<!-- endsnippet -->


## Seq Sample

Illustrates customizing [Serilog](https://serilog.net/) usage to log to [Seq](https://getseq.net/).


### Prerequisites

An instance of [Seq](https://getseq.net/) running one `http://localhost:5341`.


### Configure Serilog

<!-- snippet: ConfigureSerilog -->
<a id='snippet-configureserilog'/></a>
```cs
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
```
<sup><a href='/src/Sample/Program.cs#L13-L17' title='File snippet `configureserilog` was extracted from'>snippet source</a> | <a href='#snippet-configureserilog' title='Navigate to start of snippet `configureserilog`'>anchor</a></sup>
<a id='snippet-configureserilog-1'/></a>
```cs
var tracingLog = new LoggerConfiguration()
    .WriteTo.Seq("http://localhost:5341")
    .MinimumLevel.Information()
    .CreateLogger();
var serilogFactory = LogManager.Use<SerilogFactory>();
serilogFactory.WithLogger(tracingLog);
```
<sup><a href='/src/SeqSample/Program.cs#L13-L20' title='File snippet `configureserilog` was extracted from'>snippet source</a> | <a href='#snippet-configureserilog-1' title='Navigate to start of snippet `configureserilog`'>anchor</a></sup>
<!-- endsnippet -->


### Pass that configuration to NServiceBus

<!-- snippet: UseConfig -->
<a id='snippet-useconfig'/></a>
```cs
LogManager.Use<SerilogFactory>();

var configuration = new EndpointConfiguration("SerilogSample");
```
<sup><a href='/src/Sample/Program.cs#L19-L24' title='File snippet `useconfig` was extracted from'>snippet source</a> | <a href='#snippet-useconfig' title='Navigate to start of snippet `useconfig`'>anchor</a></sup>
<a id='snippet-useconfig-1'/></a>
```cs
var configuration = new EndpointConfiguration("SeqSample");
var serilogTracing = configuration.EnableSerilogTracing(tracingLog);
serilogTracing.EnableSagaTracing();
serilogTracing.EnableMessageTracing();
```
<sup><a href='/src/SeqSample/Program.cs#L22-L29' title='File snippet `useconfig` was extracted from'>snippet source</a> | <a href='#snippet-useconfig-1' title='Navigate to start of snippet `useconfig`'>anchor</a></sup>
<!-- endsnippet -->


### Ensure logging is flushed on shutdown

<!-- snippet: Cleanup -->
<a id='snippet-cleanup'/></a>
```cs
await endpoint.Stop();
Log.CloseAndFlush();
```
<sup><a href='/src/Sample/Program.cs#L34-L37' title='File snippet `cleanup` was extracted from'>snippet source</a> | <a href='#snippet-cleanup' title='Navigate to start of snippet `cleanup`'>anchor</a></sup>
<a id='snippet-cleanup-1'/></a>
```cs
await endpoint.Stop();
Log.CloseAndFlush();
```
<sup><a href='/src/SeqSample/Program.cs#L45-L48' title='File snippet `cleanup` was extracted from'>snippet source</a> | <a href='#snippet-cleanup-1' title='Navigate to start of snippet `cleanup`'>anchor</a></sup>
<!-- endsnippet -->



## Release Notes

See [closed milestones](../../milestones?state=closed).


## Icon

[Brain](https://thenounproject.com/noun/brain/#icon-No10411) designed by [Rémy Médard](https://thenounproject.com/catalarem) from [The Noun Project](https://thenounproject.com).
