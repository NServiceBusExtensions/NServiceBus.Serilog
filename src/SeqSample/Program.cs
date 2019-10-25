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
        Console.Title = "SeqSample";
        #region ConfigureSerilog
        var tracingLog = new LoggerConfiguration()
            .WriteTo.Seq("http://localhost:5341")
            .MinimumLevel.Information()
            .CreateLogger();
        var serilogFactory = LogManager.Use<SerilogFactory>();
        serilogFactory.WithLogger(tracingLog);
        #endregion

        #region UseConfig

        var configuration = new EndpointConfiguration("SeqSample");
        var serilogTracing = configuration.EnableSerilogTracing(tracingLog);
        serilogTracing.EnableSagaTracing();
        serilogTracing.EnableMessageTracing();

        #endregion

        configuration.UsePersistence<LearningPersistence>();
        configuration.UseTransport<LearningTransport>();

        var endpoint = await Endpoint.Start(configuration);
        var createUser = new CreateUser
        {
            UserName = "jsmith",
            FamilyName = "Smith",
            GivenNames = "John",
        };
        await endpoint.SendLocal(createUser);
        Console.WriteLine("Message sent");
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        #region Cleanup
        await endpoint.Stop();
        Log.CloseAndFlush();
        #endregion
    }
}