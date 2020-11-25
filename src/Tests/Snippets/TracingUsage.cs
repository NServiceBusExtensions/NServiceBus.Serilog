using NServiceBus;
using Serilog;
// ReSharper disable UnusedVariable

class TracingUsage
{
    TracingUsage()
    {
        #region SerilogTracingLogger

        var tracingLog = new LoggerConfiguration()
            .WriteTo.File("log.txt")
            .MinimumLevel.Information()
            .CreateLogger();

        #endregion

        EndpointConfiguration configuration = new("EndpointName");

        #region SerilogTracingPassLoggerToFeature

        var serilogTracing = configuration.EnableSerilogTracing(tracingLog);
        serilogTracing.EnableMessageTracing();

        #endregion
    }

    void EnableSagaTracing(EndpointConfiguration configuration, ILogger logger)
    {
        #region EnableSagaTracing

        var serilogTracing = configuration.EnableSerilogTracing(logger);
        serilogTracing.EnableSagaTracing();

        #endregion
    }

    void EnableMessageTracing(EndpointConfiguration configuration, ILogger logger)
    {
        #region EnableMessageTracing

        var serilogTracing = configuration.EnableSerilogTracing(logger);
        serilogTracing.EnableMessageTracing();

        #endregion
    }

    void Seq()
    {
        #region SerilogTracingSeq

        var tracingLog = new LoggerConfiguration()
            .WriteTo.Seq("http://localhost:5341")
            .MinimumLevel.Information()
            .CreateLogger();

        #endregion
    }
}

public class TheMessage
{
}