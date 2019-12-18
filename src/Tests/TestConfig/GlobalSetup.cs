using System.Diagnostics;
using NServiceBus;
using Verify;
using Xunit;

[GlobalSetUp]
public static class GlobalSetup
{
    public static void Setup()
    {
        var nsbVersion = FileVersionInfo.GetVersionInfo(typeof(Endpoint).Assembly.Location);
        var nsbVersionString = $"{nsbVersion.FileMajorPart}.{nsbVersion.FileMinorPart}.{nsbVersion.FileBuildPart}";
        SharedVerifySettings.ScrubLinesContaining("StackTraceString");
        SharedVerifySettings.AddScrubber(x => x.Replace(nsbVersionString, "NsbVersion"));
        SharedVerifySettings.ScrubMachineName();
        SharedVerifySettings.ModifySerialization(settings =>
        {
            settings.AddExtraSettings(newtonsoft =>
            {
                newtonsoft.Converters.Add(new LogEventPropertyConverter());
                newtonsoft.Converters.Add(new LogEventConverter());
                newtonsoft.Converters.Add(new ScalarValueConverter());
            });
        });
        SharedVerifySettings.AddExtraDatetimeFormat("yyyy-MM-dd HH:mm:ss:ffffff Z");
    }
}