public class Filtering
{
    public Filtering()
    {
        #region SerilogFiltering

        var configuration = new LoggerConfiguration();
        configuration.Enrich.WithNsbExceptionDetails();
        configuration
            .WriteTo.File(
                path: "log.txt",
                restrictedToMinimumLevel: LogEventLevel.Debug
            );
        configuration
            .Filter.ByIncludingOnly(
                inclusionPredicate: Matching.FromSource("MyNamespace"));
        Log.Logger = configuration.CreateLogger();

        LogManager.Use<SerilogFactory>();

        #endregion
    }
}