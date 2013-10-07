![Icon](https://raw.github.com/SimonCropp/NServiceBus.Serilog/master/Icons/package_icon.png)

NServiceBus.Serilog
==================

Add support for sending [NServiceBus](http://particular.net/NServiceBus) logging message through [Serilog](http://serilog.net/)

## Nuget

There are two nuget packages

### The [binary version](http://nuget.org/packages/NServiceBus.Serilog/)

This uses the standard approach to constructing a nuget package. It contains a dll which will be added as a reference to your project. You then deploy the binary with your project.

    PM> Install-Package NServiceBus.Serilog

### The [code only version](http://nuget.org/packages/NServiceBus.Serilog-CodeOnly/)

This is a "code only" package that leverages the [Content Convention](http://docs.nuget.org/docs/creating-packages/creating-and-publishing-a-package#From_a_convention_based_working_directory) of Nuget to inject code files into your project. Note that this is only compatible with C# projects. 

The benefits of this approach are ease of debugging and less files to deploy

    PM> Install-Package NServiceBus.Serilog-CodeOnly

## Usage 

    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();
    SerilogConfigurator.Configure();
    
    var configure = Configure
        .With().DefaultBuilder();

## Icon

<a href="http://thenounproject.com/noun/brain/#icon-No10411" target="_blank">Brain</a> designed by <a href="http://thenounproject.com/catalarem" target="_blank">Rémy Médard</a> from The Noun Project
