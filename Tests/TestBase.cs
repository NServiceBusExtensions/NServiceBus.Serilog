using Newtonsoft.Json;
using ObjectApproval;
using Xunit.Abstractions;

public class TestBase
{
    static TestBase()
    {
        SerializerBuilder.ExtraSettings = settings =>
        {
            settings.ContractResolver = new CustomContractResolverEx();
        };
        StringScrubber.AddExtraDatetimeFormat("yyyy-MM-dd HH:mm:ss:ffffff Z");
    }

    public TestBase(ITestOutputHelper output)
    {
        Output = output;
    }

    protected readonly ITestOutputHelper Output;
}