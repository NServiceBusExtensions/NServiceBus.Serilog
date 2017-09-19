using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Serilog;
using NServiceBus.Serilog.Tracing;
using Serilog;

class Program
{
    static async Task Main()
    {
        var tracingLog = new LoggerConfiguration()
            .WriteTo.Seq("http://localhost:5341")
            .MinimumLevel.Information()
            .CreateLogger();
        //Set NServiceBus to log to Serilog
        var serilogFactory = LogManager.Use<SerilogFactory>();
        serilogFactory.WithLogger(tracingLog);

        var endpointConfiguration = new EndpointConfiguration("SeqSample");
        endpointConfiguration.EnableFeature<TracingLog>();
        endpointConfiguration.SerilogTracingTarget(tracingLog);
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        endpointConfiguration.UseTransport<LearningTransport>();
        endpointConfiguration.SendFailedMessagesTo("error");
        var endpoint = await Endpoint.Start(endpointConfiguration)
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