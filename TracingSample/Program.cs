using System;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Installation.Environments;
using NServiceBus.Serilog;
using NServiceBus.Serilog.Tracing;
using Seq;
using Serilog;

class Program
{
    static void Main()
    {
        Configure.GetEndpointNameAction = () => "NServiceBusSerilogSample";

        //Setup Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logFile.txt")
            .MinimumLevel.Information()
            .CreateLogger();

        TracingLog.Enable(new LoggerConfiguration()
            .WriteTo.Seq("http://localhost:5341")
            .MinimumLevel.Information()
            .CreateLogger());

        //Set NServiceBus to log to Serilog
        SerilogConfigurator.Configure();
        Feature.Enable<Sagas>();

        //Start using NServiceBus
        Configure.Serialization.Json();
        Configure.With()
            .DefaultBuilder()
            .InMemorySagaPersister()
            .UseInMemoryTimeoutPersister()
            .InMemorySubscriptionStorage()
            .UnicastBus()
            .PurgeOnStartup(true)
            .CreateBus()
            .Start(() => Configure.Instance.ForInstallationOn<Windows>().Install());
        Console.ReadLine();
    }
}
