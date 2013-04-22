NServiceBusSerilog
==================

Add support for sending [NServiceBus](http://nservicebus.com/) logging message through [Serilog](http://serilog.net/)

## Nuget

https://nuget.org/packages/NServiceBusSerilog/
    
    PM> Install-Package NServiceBusSerilog -Pre

## Usage 

    var configure = Configure
        .With().DefaultBuilder();

    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();
    SerilogConfigurator.Configure();

## Icon

<a href="http://thenounproject.com/noun/brain/#icon-No10411" target="_blank">Brain</a> designed by <a href="http://thenounproject.com/catalarem" target="_blank">Rémy Médard</a> from The Noun Project