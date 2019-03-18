using ObjectApproval;
using Xunit.Abstractions;

public class TestBase
{
    static TestBase()
    {
        SerializerBuilder.ExtraSettings = settings =>
        {
            settings.ContractResolver = new CustomContractResolverEx();
            settings.Converters.Add(new LogEventPropertyConverter());
            settings.Converters.Add(new ScalarValueConverter());
        };
        StringScrubber.AddExtraDatetimeFormat("yyyy-MM-dd HH:mm:ss:ffffff Z");
    }

    public TestBase(ITestOutputHelper output)
    {
        Output = output;
    }

    protected readonly ITestOutputHelper Output;
}