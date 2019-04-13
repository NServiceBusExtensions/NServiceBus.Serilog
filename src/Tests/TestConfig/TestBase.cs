using ObjectApproval;
using Xunit.Abstractions;

public class TestBase:
    XunitLoggingBase
{
    public TestBase(ITestOutputHelper output) :
        base(output)
    {
    }

    static TestBase()
    {
        SerializerBuilder.ExtraSettings = settings =>
        {
            settings.ContractResolver = new CustomContractResolverEx();
            settings.Converters.Add(new LogEventPropertyConverter());
            settings.Converters.Add(new LogEventConverter());
            settings.Converters.Add(new ScalarValueConverter());
        };
        StringScrubber.AddExtraDatetimeFormat("yyyy-MM-dd HH:mm:ss:ffffff Z");
    }
}