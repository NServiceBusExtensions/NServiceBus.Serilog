using System.Collections.Concurrent;
using Serilog;
using Serilog.Core;

class LogBuilder
{
    ILogger logger;
    ConcurrentDictionary<string, ILogger> loggers = new ConcurrentDictionary<string, ILogger>();
    string endpointName;

    public LogBuilder(ILogger logger, string endpointName)
    {
        this.logger = logger;
        this.endpointName = endpointName;
    }

    public ILogger GetLogger(string key)
    {
        return loggers.GetOrAdd(key, s => logger
            .ForContext(Constants.SourceContextPropertyName, key)
            .ForContext("ProcessingEndpoint", endpointName));
    }
}