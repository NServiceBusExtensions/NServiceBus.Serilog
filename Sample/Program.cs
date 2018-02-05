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
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logFile.txt")
            .CreateLogger();

        //Set NServiceBus to log to Serilog
        LogManager.Use<SerilogFactory>();

        //Start using NServiceBus
        var configuration = new EndpointConfiguration("SerilogSample");
        configuration.EnableInstallers();
        configuration.SendFailedMessagesTo("error");
        configuration.UsePersistence<InMemoryPersistence>();
        configuration.UseTransport<LearningTransport>();

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
        await endpoint.ScheduleEvery(TimeSpan.FromSeconds(1), context => context.SendLocal(createUser))
            .ConfigureAwait(false);
        Console.WriteLine("Press any key to stop program");
        Console.Read();
    }
}