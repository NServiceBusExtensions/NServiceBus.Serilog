using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Serilog;
using Serilog;
using Serilog.Exceptions;

class Program
{
    static async Task Main()
    {
        var loggerConfiguration = new LoggerConfiguration();
        loggerConfiguration.Enrich.WithExceptionDetails();
        loggerConfiguration.WriteTo.Seq("http://localhost:5341");
        loggerConfiguration.MinimumLevel.Information();
        var tracingLog = loggerConfiguration.CreateLogger();
        //Set NServiceBus to log to Serilog
        var serilogFactory = LogManager.Use<SerilogFactory>();
        serilogFactory.WithLogger(tracingLog);

        var configuration = new EndpointConfiguration("SeqSample");
        var serilogTracing = configuration.EnableSerilogTracing(tracingLog);
        serilogTracing.EnableSagaTracing();
        serilogTracing.EnableMessageTracing();
        configuration.EnableInstallers();
        configuration.UsePersistence<InMemoryPersistence>();
        configuration.UseTransport<LearningTransport>();
        configuration.SendFailedMessagesTo("error");
        var recoverability = configuration.Recoverability();
        recoverability.Delayed(settings => { settings.NumberOfRetries(1); });
        recoverability.Immediate(settings => { settings.NumberOfRetries(1); });
        var endpoint = await Endpoint.Start(configuration)
            .ConfigureAwait(false);
        var createUser = new CreateUser
        {
            UserName = "jsmith",
            FamilyName = "Smith",
            GivenNames = "John",
        };
        await endpoint.SendLocal(createUser)
            .ConfigureAwait(false);
      //  await endpoint.ScheduleEvery(TimeSpan.FromSeconds(1), context => context.SendLocal(createUser));
        Console.WriteLine("Press any key to stop program");
        Console.Read();
    }
}