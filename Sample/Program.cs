using System;
using NServiceBus;
using NServiceBus.Installation.Environments;
using NServiceBus.Serilog;
using Serilog;

namespace Sample
{
    class Program
    {
        static void Main()
        {
            Configure.GetEndpointNameAction = () => "NServiceBusSerilogSample";

            var configure = Configure
                .With().DefaultBuilder();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            SerilogConfigurator.Configure();


            configure.JsonSerializer();
            configure.UseTransport<Msmq>();
            configure.UnicastBus();
            var bus = configure
                .CreateBus()
                .Start(() => Configure.Instance.ForInstallationOn<Windows>().Install());
            Console.ReadLine();
        }
    }
}
