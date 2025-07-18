﻿var loggerConfiguration = new LoggerConfiguration();
loggerConfiguration.WriteTo.Seq("http://localhost:5341");
loggerConfiguration.WriteTo.File("logFile.txt");
loggerConfiguration.MinimumLevel.Information();
var tracingLog = loggerConfiguration.CreateLogger();
//Set NServiceBus to log to Serilog
var serilogFactory = LogManager.Use<SerilogFactory>();
serilogFactory.WithLogger(tracingLog);

var configuration = new EndpointConfiguration("SeqSample");
var serilogTracing = configuration.EnableSerilogTracing(tracingLog);
serilogTracing.EnableSagaTracing();
serilogTracing.EnableMessageTracing();
configuration.EnableInstallers();
configuration.UsePersistence<NonDurablePersistence>();
configuration.UseSerialization<SystemJsonSerializer>();
configuration.UseTransport<LearningTransport>();
configuration.SendFailedMessagesTo("error");

var settings = configuration.GetSettings();
settings.Set("NServiceBus.Features.LicenseReminder", FeatureState.Deactivated);

var recoverability = configuration.Recoverability();
recoverability.Delayed(_ => _.NumberOfRetries(1));
recoverability.Immediate(_ => _.NumberOfRetries(1));
var endpoint = await Endpoint.Start(configuration);
var createUser = new CreateUser
{
    UserName = "jsmith",
    FamilyName = "Smith",
    GivenNames = "John"
};
await endpoint.SendLocal(createUser);
//  await endpoint.ScheduleEvery(TimeSpan.FromSeconds(1), context => context.SendLocal(createUser));
Console.WriteLine("Press any key to stop program");
Console.Read();