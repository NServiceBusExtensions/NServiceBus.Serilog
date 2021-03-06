﻿using System;
using System.Threading.Tasks;
using NServiceBus;

public class HandlerUsingContextLogger :
    IHandleMessages<StartHandlerUsingContextLogger>
{
    public Task Handle(StartHandlerUsingContextLogger message, IMessageHandlerContext context)
    {
        context.LogError("The message", new Exception());
        return Task.CompletedTask;
    }
}