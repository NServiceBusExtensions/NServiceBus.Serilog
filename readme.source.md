<img src="/src/icon.png" height="25px"> Add support for sending [NServiceBus](http://particular.net/NServiceBus) logging through [Serilog](http://serilog.net/)
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

## NuGet package

https://nuget.org/packages/NServiceBus.Serilog/ [![NuGet Status](https://img.shields.io/nuget/v/NServiceBus.Serilog.svg)](https://www.nuget.org/packages/NServiceBus.Serilog/)


## Usage

snippet: SerilogInCode


## Seq

To log to [Seq](https://getseq.net/):

snippet: SerilogSeq


## Filtering

NServiceBus can write a significant amount of information to the log. To limit this information use the filtering features of the underlying logging framework.

For example to limit log output to a specific namespace.

Here is a code configuration example for adding a [Filter](https://github.com/serilog/serilog/wiki/Configuration-Basics#filters).

snippet: SerilogFiltering


## Tracing

Writing diagnostic log entries to [Serilog](https://serilog.net/). Plugs into the low level [pipeline](/nservicebus/pipeline) to give more detailed diagnostics.

When using Serilog for tracing, it is optional to use Serilog as the main NServiceBus logger. i.e. there is no need to include `LogManager.Use<SerilogFactory>();`.


### Create an instance of a Serilog logger

snippet: SerilogTracingLogger


### Configure the tracing feature to use that logger

snippet: SerilogTracingPassLoggerToFeature


### Contextual logger

Serilog tracing injects a contextual `Serilog.Ilogger` into the NServiceBus pipeline.

NOTE: Saga and message tracing will use the current contextual logger.

There are several layers of enrichment based on the pipeline phase.


#### Endpoint enrichment

All loggers for an endpoint will have the the property `ProcessingEndpoint` added that contains the current [endpoint name](/nservicebus/endpoints/specify-endpoint-name.md).


#### Incoming message enrichment

When a message is received, the following enrichment properties are added:

 * [SourceContext](https://github.com/serilog/serilog/wiki/Writing-Log-Events#source-contexts) will be the message type [FullName](https://docs.microsoft.com/de-de/dotnet/api/system.type.fullname) extracted from the [EnclosedMessageTypes header](/nservicebus/messaging/headers.md#serialization-headers-nservicebus-enclosedmessagetypes). `UnknownMessageType` will be used if no header exists. The same value will be added to a property named `MessageType`.
 * `MessageId` will be the value of the [MessageId header](/nservicebus/messaging/headers.md#messaging-interaction-headers-nservicebus-messageid).
 * `CorrelationId` will be the value of the [CorrelationId header](/nservicebus/messaging/headers.md#messaging-interaction-headers-nservicebus-correlationid) if it exists.
 * `ConversationId` will be the value of the [ConversationId header](/nservicebus/messaging/headers.md#messaging-interaction-headers-nservicebus-conversationid) if it exists.


#### Handler enrichment

When a handler is invoked, a new logger is forked from the above enriched physical logger with a new enriched property named `Handler` that contains the the [FullName](https://docs.microsoft.com/de-de/dotnet/api/system.type.fullname) of the current handler.


#### Outgoing message enrichment

When a message is sent, the same properties as described in "Incoming message enrichment" will be added to the outgoing pipeline. Note that if a handler sends a message, the logger injected into the outgoing pipeline will be forked from the logger instance as described in "Handler enrichment". As such it will contain a property `Handler` for the handler that sent the message.


#### Accessing the logger

The contextual logger instance can be accessed from anywhere in the pipeline via `SerilogTracingExtensions.Logger(this IPipelineContext context)`.

snippet: ContextualLoggerUsage


### Exception enrichment

When an exception occurs in the message processing pipeline, the current pipeline state is added to the exception. When that exception is logged that state can be add to the log entry.

The type added to the exception data is `ExceptionLogState`. It contains the following data:

 * `ProcessingEndpoint` will be the current [endpoint name](/nservicebus/endpoints/specify-endpoint-name.md).
 * `MessageId` will be the value of the [MessageId header](/nservicebus/messaging/headers.md#messaging-interaction-headers-nservicebus-messageid).
 * `Headers` will be the value of the [Message headers](/nservicebus/messaging/headers.md).
 * `MessageType` will be the message type [FullName](https://docs.microsoft.com/de-de/dotnet/api/system.type.fullname) extracted from the [EnclosedMessageTypes header](/nservicebus/messaging/headers.md#serialization-headers-nservicebus-enclosedmessagetypes). `UnknownMessageType` will be used if no header exists.
 * `CorrelationId` will be the value of the [CorrelationId header](/nservicebus/messaging/headers.md#messaging-interaction-headers-nservicebus-correlationid) if it exists.
 * `ConversationId` will be the value of the [ConversationId header](/nservicebus/messaging/headers.md#messaging-interaction-headers-nservicebus-conversationid) if it exists.
 * `HandlerType` will be type name for the current handler if it exists.
 * `Message` will be the value of current logical message if it exists.

The instance of `ExceptionLogState` can be accessed using the following.

snippet: ExceptionLogState

When routing the NServiceBus log event with `LogManager.Use<SerilogFactory>();`, the above properties will be promoted to the log event.


### Saga tracing

snippet: EnableSagaTracing


### Message tracing

Both incoming and outgoing messages will be logged at the [Information level](https://github.com/serilog/serilog/wiki/Writing-Log-Events#the-role-of-the-information-level). The current message will be included in a property named `Message`. For outgoing messages any unicast routes will be included in a property named `UnicastRoutes`.

snippet: EnableMessageTracing


## Seq

To log to [Seq](https://getseq.net/):

snippet: SerilogTracingSeq


## Sample

The sample illustrates how to customize logging by configuring Serilog targets and rules.


### Configure Serilog

snippet: ConfigureSerilog


### Pass the configuration to NServiceBus

snippet: UseConfig


### Ensure logging is flushed on shutdown

snippet: Cleanup


## Icon

[Brain](https://thenounproject.com/noun/brain/#icon-No10411) designed by [Rémy Médard](https://thenounproject.com/catalarem) from [The Noun Project](https://thenounproject.com).