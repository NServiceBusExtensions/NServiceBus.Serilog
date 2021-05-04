using System.Threading.Tasks;
using NServiceBus;

#region ContextualLoggerUsage

public class SimpleHandler :
    IHandleMessages<TheMessage>
{
    public Task Handle(TheMessage message, IMessageHandlerContext context)
    {
        context.LogInformation("Hello from {@Handler}.");
        return Task.CompletedTask;
    }
}
#endregion