using Xunit.Abstractions;
using ObjectApproval;

public class TestBase:
    XunitApprovalBase
{
    public TestBase(ITestOutputHelper output, [CallerFilePath] string sourceFilePath = "") :
        base(output, sourceFilePath)
    {
    }

    static TestBase()
    {
        SerializerBuilder.ExtraSettings = settings =>
        {
            settings.Converters.Add(new LogEventPropertyConverter());
            settings.Converters.Add(new LogEventConverter());
            settings.Converters.Add(new ScalarValueConverter());
        };
        StringScrubber.AddExtraDatetimeFormat("yyyy-MM-dd HH:mm:ss:ffffff Z");
    }
}