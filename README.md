![Icon](https://raw.github.com/SimonCropp/NServiceBus.Serilog/master/Icons/package_icon.png)

NServiceBus.Serilog
==================

Add support for sending [NServiceBus](http://particular.net/NServiceBus) logging message through [Serilog](http://serilog.net/)


## Standard Logging Library

Plus into the standard NServiceBus logging API to pipe message through to Serilog.


### Nuget


#### http://nuget.org/packages/NServiceBus.Serilog/  [![NuGet Status](http://img.shields.io/nuget/v/NServiceBus.Serilog.svg?style=flat)](https://www.nuget.org/packages/NServiceBus.Serilog/)

This uses the standard approach to constructing a nuget package. It contains a dll which will be added as a reference to your project. You then deploy the binary with your project.

    PM> Install-Package NServiceBus.Serilog

### Usage 

    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("logFile.txt")
        .CreateLogger();

    //Set NServiceBus to log to Serilog
    LogManager.Use<SerilogFactory>();


## Tracing Library

Plugs into the low level NServiceBus pipeline to give more detailed diagnostics.


### Nuget


#### https://www.nuget.org/packages/NServiceBus.Serilog.Tracing/   [![NuGet Status](http://img.shields.io/nuget/v/NServiceBus.Serilog.Tracing.svg?style=flat)](https://www.nuget.org/packages/NServiceBus.Serilog.Tracing/)

    PM> Install-Package NServiceBus.Serilog.Tracing


### Usage 

    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("logFile.txt")
        .MinimumLevel.Information()
        .CreateLogger();
    LogManager.Use<SerilogFactory>();

    var config = new EndpointConfiguration("SeqSample");
    config.EnableFeature<TracingLog>();

To log to [Seq](http://getseq.net/ "Seq") use 

    var tracingLog = new LoggerConfiguration()
        .WriteTo.Seq("http://localhost:5341")
        .MinimumLevel.Information()
        .CreateLogger();

Then call

    config.SerilogTracingTarget(tracingLog);

Which will result in something like this

![](https://raw.githubusercontent.com/SimonCropp/NServiceBus.Serilog/master/NsbSeq.png)


## Icon

<a href="http://thenounproject.com/noun/brain/#icon-No10411" target="_blank">Brain</a> designed by <a href="http://thenounproject.com/catalarem" target="_blank">Rémy Médard</a> from The Noun Project
