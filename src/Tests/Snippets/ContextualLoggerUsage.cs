﻿#region ContextualLoggerUsage

public class HandlerUsingLogger :
    IHandleMessages<TheMessage>
{
    public Task Handle(TheMessage message, HandlerContext context)
    {
        var logger = context.Logger();
        logger.Information("Hello from {@Handler}.");
        return Task.CompletedTask;
    }
}

#endregion

#region DirectLogUsage

public class HandlerUsingLog :
    IHandleMessages<TheMessage>
{
    public Task Handle(TheMessage message, HandlerContext context)
    {
        context.LogInformation("Hello from {@Handler}.");
        return Task.CompletedTask;
    }
}

#endregion