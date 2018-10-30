using ObjectApproval;
using Xunit.Abstractions;

public class TestBase
{
    static TestBase()
    {
        StringScrubber.AddExtraDatetimeFormat("yyyy-MM-dd HH:mm:ss:ffffff Z");
    }

    public TestBase(ITestOutputHelper output)
    {
        Output = output;
    }

    protected readonly ITestOutputHelper Output;
}