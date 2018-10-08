using System.Collections.Concurrent;
using Serilog;
using Serilog.Core;
using Serilog.Core.Enrichers;

class LogBuilder
{
    ILogger logger;
    ConcurrentDictionary<string, ILogger> loggers = new ConcurrentDictionary<string, ILogger>();

    public LogBuilder(ILogger logger, string endpointName)
    {
        this.logger = logger
            .ForContext(new[]
            {
                new PropertyEnricher("ProcessingEndpoint", endpointName)
            });
    }

    public ILogger GetLogger(string key)
    {
        return loggers.GetOrAdd(key, s => logger
            .ForContext(Constants.SourceContextPropertyName, key));
    }
}