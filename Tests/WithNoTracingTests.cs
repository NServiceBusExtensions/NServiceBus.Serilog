using System;
using System.Threading;
using System.Threading.Tasks;
using ApprovalTests;
using NServiceBus;
using Xunit;
using Xunit.Abstractions;

public class WithNoTracingTests : TestBase
{
    static ManualResetEvent resetEvent;
    static Exception exception;

    [Fact]
    public async Task Handler()
    {
        resetEvent = new ManualResetEvent(false);
        var configuration = new EndpointConfiguration("WithNoTracingTests");
        configuration.SendFailedMessagesTo("error");
        configuration.UsePersistence<InMemoryPersistence>();
        configuration.PurgeOnStartup(true);
        configuration.UseTransport<LearningTransport>();
        configuration.DisableRetries();

        configuration.Notifications.Errors.MessageSentToErrorQueue +=
            (sender, retry) =>
            {
                exception = retry.Exception;
                resetEvent.Set();
            };

        var endpoint = await Endpoint.Start(configuration);
        await endpoint.SendLocal(new StartHandler());
        if (!resetEvent.WaitOne(TimeSpan.FromSeconds(2)))
        {
            throw new Exception("No Set received.");
        }

        await endpoint.Stop();
        Approvals.Verify(exception.Message);
    }

    public WithNoTracingTests(ITestOutputHelper output) : base(output)
    {
    }
}