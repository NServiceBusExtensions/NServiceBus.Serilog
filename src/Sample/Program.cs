using NServiceBus.Logging;
using NServiceBus.Serilog;

void ConfigureSerilog()
{
    #region ConfigureSerilog

    var configuration = new LoggerConfiguration();
    configuration.Enrich.WithNsbExceptionDetails();
    configuration.WriteTo.Console();
    Log.Logger = configuration.CreateLogger();

    #endregion
}

Console.Title = "SerilogSample";
ConfigureSerilog();

#region UseConfig
LogManager.Use<SerilogFactory>();

var configuration = new EndpointConfiguration("SerilogSample");

#endregion

configuration.UsePersistence<LearningPersistence>();
configuration.UseTransport<LearningTransport>();

var endpoint = await Endpoint.Start(configuration);
var message = new MyMessage();
await endpoint.SendLocal(message);
Console.WriteLine("Press any key to exit");
Console.ReadKey();
#region Cleanup
await endpoint.Stop();
Log.CloseAndFlush();
#endregion