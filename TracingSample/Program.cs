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
        var tracingLog = new LoggerConfiguration()
            .WriteTo.Seq("http://localhost:5341")
            .MinimumLevel.Information()
            .CreateLogger();
        //Set NServiceBus to log to Serilog
        var serilogFactory = LogManager.Use<SerilogFactory>();
        serilogFactory.WithLogger(tracingLog);

        var config = new EndpointConfiguration("SeqSample");
        config.EnableFeature<TracingLog>();
        config.SerilogTracingTarget(tracingLog);
        config.UseSerialization<JsonSerializer>();
        config.EnableInstallers();
        config.UsePersistence<InMemoryPersistence>();
        config.SendFailedMessagesTo("error");
        var endpoint = await Endpoint.Start(config)
            .ConfigureAwait(false);
        var createUser = new CreateUser
        {
            UserName = "jsmith",
            FamilyName = "Smith",
            GivenNames = "John",
        };
        await endpoint.SendLocal(createUser)
            .ConfigureAwait(false);
        Console.WriteLine("Press any key to stop program");
        Console.Read();
    }
}