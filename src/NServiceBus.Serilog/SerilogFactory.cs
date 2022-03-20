namespace NServiceBus.Serilog;

/// <summary>
/// Configure NServiceBus logging messages to use Serilog.  Use by calling <see cref="LogManager.Use{T}"/> the T is <see cref="SerilogFactory"/>.
/// </summary>
public class SerilogFactory :
    LoggingFactoryDefinition
{
    ILogger? loggerToUse;

    /// <summary>
    /// <see cref="LoggingFactoryDefinition.GetLoggingFactory"/>.
    /// </summary>
    protected override ILoggerFactory GetLoggingFactory() =>
        new LoggerFactory(loggerToUse ?? Log.Logger);

    /// <summary>
    /// Specify an instance of <see cref="ILogger"/> to use. If not specified then the default is <see cref="Log.Logger"/>.
    /// </summary>
    public void WithLogger(ILogger logger) =>
        loggerToUse = logger;
}