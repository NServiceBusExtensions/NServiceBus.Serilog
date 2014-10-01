![Icon](https://raw.github.com/SimonCropp/NServiceBus.Serilog/master/Icons/package_icon.png)

NServiceBus.Serilog
==================

Add support for sending [NServiceBus](http://particular.net/NServiceBus) logging message through [Serilog](http://serilog.net/)

## Standard Logging Library

Plus into the standard NServiceBus logging API to pipe message through to Serilog.

### Nuget

There are two nuget packages

#### http://nuget.org/packages/NServiceBus.Serilog/

This uses the standard approach to constructing a nuget package. It contains a dll which will be added as a reference to your project. You then deploy the binary with your project.

    PM> Install-Package NServiceBus.Serilog

#### http://nuget.org/packages/NServiceBus.Serilog-CodeOnly/

This is a "code only" package that leverages the [Content Convention](http://docs.nuget.org/docs/creating-packages/creating-and-publishing-a-package#From_a_convention_based_working_directory) of Nuget to inject code files into your project. Note that this is only compatible with C# projects. 

The benefits of this approach are ease of debugging and less files to deploy

    PM> Install-Package NServiceBus.Serilog-CodeOnly

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

#### https://www.nuget.org/packages/NServiceBus.Serilog.Tracing/

    PM> Install-Package NServiceBus.Serilog.Tracing

### Usage 

    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("logFile.txt")
        .MinimumLevel.Information()
        .CreateLogger();
    LogManager.Use<SerilogFactory>();



    var busConfig = new BusConfiguration();
    busConfig.EndpointName("SeqSample");
    busConfig.EnableFeature<TracingLog>();

To log to [Seq](http://getseq.net/ "Seq") use 

    var tracingLog = new LoggerConfiguration()
        .WriteTo.Seq("http://localhost:5341")
        .MinimumLevel.Information()
        .CreateLogger();

Then call

    busConfig.SerilogTracingTarget(tracingLog);

Which will result in something like this

![](https://raw.githubusercontent.com/SimonCropp/NServiceBus.Serilog/master/NsbSeq.png)

## Icon

<a href="http://thenounproject.com/noun/brain/#icon-No10411" target="_blank">Brain</a> designed by <a href="http://thenounproject.com/catalarem" target="_blank">Rémy Médard</a> from The Noun Project
