NServiceBus.Serilog
==================

Add support for sending [NServiceBus](http://nservicebus.com/) logging message through [Serilog](http://serilog.net/)

## Nuget

https://nuget.org/packages/NServiceBus.Serilog/
    
    PM> Install-Package NServiceBus.Serilog

## Usage 

    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();
    SerilogConfigurator.Configure();
    
    var configure = Configure
        .With().DefaultBuilder();

## Is a reference necessary

You may be thinking that having an assembly reference to achieve this functionality is undesirable. If this is the case you should avoid the reference by taking the following steps.
 
* Copy [LoggingFactory](https://github.com/SimonCropp/NServiceBus.Serilog/blob/master/NServiceBus.Serilog/LoggerFactory.cs) into your project 
* Copy [Logger](https://github.com/SimonCropp/NServiceBus.Serilog/blob/master/NServiceBus.Serilog/Logger.cs) into your project
* Call this code when you application starts `NServiceBus.LogManager.LoggerFactory = new LoggerFactory();`

## Icon

<a href="http://thenounproject.com/noun/brain/#icon-No10411" target="_blank">Brain</a> designed by <a href="http://thenounproject.com/catalarem" target="_blank">Rémy Médard</a> from The Noun Project
