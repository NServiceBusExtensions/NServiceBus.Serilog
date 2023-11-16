class LogBuilder(ILogger logger, string endpointName)
{
    ConcurrentDictionary<string, ILogger> loggers = new();

    public ILogger Logger { get; } = logger
        .ForContext(
        [
            new PropertyEnricher("ProcessingEndpoint", endpointName)
        ]);

    public ILogger GetLogger(string key) =>
        loggers.GetOrAdd(
            key,
            _ => Logger
                .ForContext(Constants.SourceContextPropertyName, _));
}