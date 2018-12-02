using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Serilog;
using Serilog;

class Program
{
    static async Task Main()
    {
        //Setup Serilog
        var loggerConfiguration = new LoggerConfiguration();
        loggerConfiguration.WriteTo.Console();
        loggerConfiguration.MinimumLevel.Information();
        loggerConfiguration.WriteTo.File("logFile.txt");
        var logger = loggerConfiguration.CreateLogger();
        Log.Logger = logger;

        //Set NServiceBus to log to Serilog
        var serilogFactory = LogManager.Use<SerilogFactory>();
        serilogFactory.WithLogger(logger);

        //Start using NServiceBus
        var configuration = new EndpointConfiguration("SerilogSample");
        configuration.EnableInstallers();
        configuration.SendFailedMessagesTo("error");
        configuration.UsePersistence<InMemoryPersistence>();
        configuration.UseTransport<LearningTransport>();

        var serilogTracing = configuration.EnableSerilogTracing();
        serilogTracing.EnableMessageTracing();

        var endpoint = await Endpoint.Start(configuration);
        var createUser = new CreateUser
        {
            UserName = "jsmith",
            FamilyName = "Smith",
            GivenNames = "John",
        };
        await endpoint.SendLocal(createUser);
      //  await endpoint.ScheduleEvery(TimeSpan.FromSeconds(5), context => context.SendLocal(createUser));
        Console.WriteLine("Press any key to stop program");
        Console.Read();
    }
}