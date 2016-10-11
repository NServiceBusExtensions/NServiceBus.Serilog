using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Serilog;
using NServiceBus.Serilog.Tracing;
using Serilog;

class Program
{
    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logFile.txt")
            .MinimumLevel.Information()
            .CreateLogger();
        LogManager.Use<SerilogFactory>();

        var tracingLog = new LoggerConfiguration()
            .WriteTo.Seq("http://localhost:5341")
            .MinimumLevel.Information()
            .CreateLogger();

        var config = new EndpointConfiguration("SeqSample");
        config.EnableFeature<TracingLog>();
        config.SerilogTracingTarget(tracingLog);
        config.UseSerialization<JsonSerializer>();
        config.EnableInstallers();
        config.UsePersistence<InMemoryPersistence>();
        config.SendFailedMessagesTo("error");
        var endpoint = await Endpoint.Start(config);
        try
        {
            var createUser = new CreateUser
            {
                UserName = "jsmith",
                FamilyName = "Smith",
                GivenNames = "John",
            };
            await endpoint.SendLocal(createUser);
            Console.WriteLine("Press any key to stop program");
            Console.Read();
        }
        finally
        {
            await endpoint.Stop();
        }
    }
}