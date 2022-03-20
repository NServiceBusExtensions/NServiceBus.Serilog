Logger ConfigureSerilog()
{
    #region ConfigureSerilog

    var configuration = new LoggerConfiguration();
    configuration.Enrich.WithNsbExceptionDetails();
    configuration.WriteTo.Seq("http://localhost:5341");
    configuration.MinimumLevel.Information();
    var logger = configuration.CreateLogger();
    var serilogFactory = LogManager.Use<SerilogFactory>();
    serilogFactory.WithLogger(logger);

    #endregion

    return logger;
}

Console.Title = "SeqSample";
var tracingLog = ConfigureSerilog();

#region UseConfig

var configuration = new EndpointConfiguration("SeqSample");
var serilogTracing = configuration.EnableSerilogTracing(tracingLog);
serilogTracing.EnableSagaTracing();
serilogTracing.EnableMessageTracing();

#endregion

configuration.UsePersistence<LearningPersistence>();
configuration.UseTransport<LearningTransport>();

var endpoint = await Endpoint.Start(configuration);
var createUser = new CreateUser
{
    UserName = "jsmith",
    FamilyName = "Smith",
    GivenNames = "John",
};
await endpoint.SendLocal(createUser);
Console.WriteLine("Message sent");
Console.WriteLine("Press any key to exit");
Console.ReadKey();
#region Cleanup
await endpoint.Stop();
Log.CloseAndFlush();
#endregion