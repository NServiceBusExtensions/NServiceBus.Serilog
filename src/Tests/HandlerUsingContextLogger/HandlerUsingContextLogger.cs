﻿public class HandlerUsingContextLogger :
    IHandleMessages<StartHandlerUsingContextLogger>
{
    public async Task Handle(StartHandlerUsingContextLogger message, HandlerContext context)
    {
        await Task.Delay(1100, context.CancellationToken);
        Log.Error("The message");
    }
}