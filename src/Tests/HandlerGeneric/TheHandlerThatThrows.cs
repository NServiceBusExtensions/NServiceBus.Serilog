﻿using System.Threading;
using System.Threading.Tasks;
using NServiceBus;

public class GenericHandler :
    IHandleMessages<StartGenericHandler<string>>
{
    ManualResetEvent resetEvent;

    public GenericHandler(ManualResetEvent resetEvent)
    {
        this.resetEvent = resetEvent;
    }

    public Task Handle(StartGenericHandler<string> message, IMessageHandlerContext context)
    {
        var logger = context.Logger();
        logger.Information("Hello from {@Handler}.");
        resetEvent.Set();
        return Task.CompletedTask;
    }
}