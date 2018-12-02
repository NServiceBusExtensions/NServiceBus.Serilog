<img src="https://raw.github.com/NServiceBusExtensions/NServiceBus.Serilog/master/Icons/package_icon.png" height="25px"> Add support for sending [NServiceBus](http://particular.net/NServiceBus) logging message through [Serilog](http://serilog.net/)

<!--- StartOpenCollectiveBackers -->

## Community backed

**This is a community backed project. Backing is done via [opencollective.com/nservicebusextensions](https://opencollective.com/nservicebusextensions/).**

**It is expected that any developer that uses any of these libraries [become at a backer](https://opencollective.com/nservicebusextensions#contribute).** This is an honesty system, there is no code that enforces this requirement. However when raising an issue or a pull request, the GitHub users name may be checked against [the list of backers](https://github.com/NServiceBusExtensions/Home/blob/master/backers.md), and that issue/PR may be closed without further examination.


### Backers

Thanks to all the backers! [[Become a backer](https://opencollective.com/nservicebusextensions#contribute)]

<a href="https://opencollective.com/nservicebusextensions#contribute" target="_blank"><img src="https://opencollective.com/nservicebusextensions/tiers/backer.svg"></a>

[<img src="https://opencollective.com/nservicebusextensions/donate/button@2x.png?color=blue" width="200px">](https://opencollective.com/nservicebusextensions#contribute)

<!--- EndOpenCollectiveBackers -->


## The NuGet package [![NuGet Status](http://img.shields.io/nuget/v/Newtonsoft.Serilog.svg?style=flat)](https://www.nuget.org/packages/Newtonsoft.Serilog/)

https://nuget.org/packages/Newtonsoft.Json.Serilog/

    PM> Install-Package Newtonsoft.Serilog


## Standard Logging Library

Pipe [NServiceBus logging messages](https://docs.particular.net/nservicebus/logging/) through to Serilog.


### Documentation

https://docs.particular.net/nuget/NServiceBus.Serilog


### Usage

```csharp
var loggerConfiguration = new LoggerConfiguration();
loggerConfiguration.WriteTo.Console();
loggerConfiguration.MinimumLevel.Debug();
loggerConfiguration.WriteTo.File("logFile.txt");
var logger = loggerConfiguration.CreateLogger();

Log.Logger = logger;

//Set NServiceBus to log to Serilog
var serilogFactory = LogManager.Use<SerilogFactory>();
serilogFactory.WithLogger(logger);
```


## Tracing Library

Plugs into the low level NServiceBus pipeline to give more detailed diagnostics.


### Documentation

https://docs.particular.net/nuget/NServiceBus.Serilog.Tracing


### Usage

```csharp
var tracingLog = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logFile.txt")
    .MinimumLevel.Information()
    .CreateLogger();

var serilogFactory = LogManager.Use<SerilogFactory>();
serilogFactory.WithLogger(tracingLog);

var config = new EndpointConfiguration("SeqSample");
var serilogTracing = configuration.EnableSerilogTracing(tracingLog);
serilogTracing.EnableSagaTracing();
```

To log to [Seq](http://getseq.net/ "Seq") use the following to create the Logger.

```csharp
var tracingLog = new LoggerConfiguration()
    .WriteTo.Seq("http://localhost:5341")
    .MinimumLevel.Information()
    .CreateLogger();
```

Which will result in something like this

![](https://raw.githubusercontent.com/NServiceBusExtensions/NServiceBus.Serilog/master/NsbSeq.png)


## Icon

<a href="http://thenounproject.com/noun/brain/#icon-No10411" target="_blank">Brain</a> designed by <a href="http://thenounproject.com/catalarem" target="_blank">Rémy Médard</a> from The Noun Project