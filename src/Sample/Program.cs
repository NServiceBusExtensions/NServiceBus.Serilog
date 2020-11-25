using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Serilog;
using Serilog;

static class Program
{
    static async Task Main()
    {
        Console.Title = "SerilogSample";
        #region ConfigureSerilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        #endregion

        #region UseConfig
        LogManager.Use<SerilogFactory>();

        var configuration = new EndpointConfiguration("SerilogSample");

        #endregion

        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();

        var endpoint = await Endpoint.Start(configuration);
        MyMessage myMessage = new();
        await endpoint.SendLocal(myMessage);
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        #region Cleanup
        await endpoint.Stop();
        Log.CloseAndFlush();
        #endregion
    }
}