using System.Diagnostics;
using NServiceBus;
using VerifyXunit;
using Xunit;

[GlobalSetUp]
public static class GlobalSetup
{
    public static void Setup()
    {
        var nsbVersion = FileVersionInfo.GetVersionInfo(typeof(Endpoint).Assembly.Location);
        var nsbVersionString = $"{nsbVersion.FileMajorPart}.{nsbVersion.FileMinorPart}.{nsbVersion.FileBuildPart}";
        Global.AddScrubber(x => x.RemoveLinesContaining("StackTraceString"));
        Global.AddScrubber(x => x.Replace(nsbVersionString, "NsbVersion"));
        Global.ModifySerialization(settings =>
        {
            settings.AddExtraSettings(newtonsoft =>
            {
                newtonsoft.Converters.Add(new LogEventPropertyConverter());
                newtonsoft.Converters.Add(new LogEventConverter());
                newtonsoft.Converters.Add(new ScalarValueConverter());
            });
            StringScrubbingConverter.AddExtraDatetimeFormat("yyyy-MM-dd HH:mm:ss:ffffff Z");
        });
    }
}