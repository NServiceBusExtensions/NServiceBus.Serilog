using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Serilog;
using Serilog;

class Program
{
    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        //Setup Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logFile.txt")
            .CreateLogger();

        //Set NServiceBus to log to Serilog
        LogManager.Use<SerilogFactory>();

        //Start using NServiceBus
        var config = new EndpointConfiguration("SerilogSample");
        config.UseSerialization<JsonSerializer>();
        config.EnableInstallers();
        config.SendFailedMessagesTo("error");
        config.UsePersistence<InMemoryPersistence>();

        var endpoint = await Endpoint.Start(config).ConfigureAwait(false);
        try
        {
            Console.WriteLine("\r\nPress any key to stop program\r\n");
            Console.Read();
        }
        finally
        {
            await endpoint.Stop().ConfigureAwait(false);
        }
    }
}