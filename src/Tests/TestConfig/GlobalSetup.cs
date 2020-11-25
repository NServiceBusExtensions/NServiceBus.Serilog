using System.Diagnostics;
using NServiceBus;
using System.Runtime.CompilerServices;
using VerifyTests;

public static class GlobalSetup
{
    [ModuleInitializer]
    public static void Initialize()
    {
        var nsbVersion = FileVersionInfo.GetVersionInfo(typeof(Endpoint).Assembly.Location);
        var nsbVersionString = $"{nsbVersion.FileMajorPart}.{nsbVersion.FileMinorPart}.{nsbVersion.FileBuildPart}";
        VerifierSettings.ScrubLinesContaining("StackTraceString");
        VerifierSettings.ScrubLinesContaining("NServiceBus.TimeSent");
        VerifierSettings.ScrubLinesContaining("HandlerStartTime");
        VerifierSettings.ScrubLinesContaining("HandlerFailureTime");
        VerifierSettings.AddScrubber(x => x.Replace(nsbVersionString, "NsbVersion"));
        VerifierSettings.ScrubMachineName();
        VerifierSettings.ModifySerialization(settings =>
        {
            settings.AddExtraSettings(newtonsoft =>
            {
                newtonsoft.Converters.Add(new LogEventPropertyConverter());
                newtonsoft.Converters.Add(new LogEventConverter());
                newtonsoft.Converters.Add(new ScalarValueConverter());
            });
        });
        VerifierSettings.AddExtraDatetimeFormat("yyyy-MM-dd HH:mm:ss:ffffff Z");
    }
}