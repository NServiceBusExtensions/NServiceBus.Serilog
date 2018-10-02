![Icon](https://raw.github.com/SimonCropp/NServiceBus.Serilog/master/Icons/package_icon.png)

NServiceBus.Serilog
==================

Add support for sending [NServiceBus](http://particular.net/NServiceBus) logging message through [Serilog](http://serilog.net/)


## Standard Logging Library

Plus into the standard NServiceBus logging API to pipe message through to Serilog.


### Documentation

https://docs.particular.net/nuget/NServiceBus.Serilog


### Nuget


#### http://nuget.org/packages/NServiceBus.Serilog/  [![NuGet Status](http://img.shields.io/nuget/v/NServiceBus.Serilog.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/NServiceBus.Serilog/)

This uses the standard approach to constructing a nuget package. It contains a dll which will be added as a reference to your project. You then deploy the binary with your project.

```
PM> Install-Package NServiceBus.Serilog
```


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


### Nuget


#### https://www.nuget.org/packages/NServiceBus.Serilog.Tracing/   [![NuGet Status](http://img.shields.io/nuget/v/NServiceBus.Serilog.Tracing.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/NServiceBus.Serilog.Tracing/)

```
PM> Install-Package NServiceBus.Serilog.Tracing
```


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

![](https://raw.githubusercontent.com/SimonCropp/NServiceBus.Serilog/master/NsbSeq.png)


## Icon

<a href="http://thenounproject.com/noun/brain/#icon-No10411" target="_blank">Brain</a> designed by <a href="http://thenounproject.com/catalarem" target="_blank">Rémy Médard</a> from The Noun Project