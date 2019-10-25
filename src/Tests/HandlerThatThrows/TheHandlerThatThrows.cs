using System;
using System.Threading.Tasks;
using NServiceBus;

public class TheHandlerThatThrows :
    IHandleMessages<StartHandlerThatThrows>
{
    public Task Handle(StartHandlerThatThrows message, IMessageHandlerContext context)
    {
        throw new Exception();
    }
}