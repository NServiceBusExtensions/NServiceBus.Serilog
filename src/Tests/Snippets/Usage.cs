using System;
using NServiceBus.Logging;
using NServiceBus.Serilog;
using Serilog;

class Usage
{
    Usage()
    {
        #region SerilogInCode

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("log.txt")
            .CreateLogger();

        LogManager.Use<SerilogFactory>();

        #endregion
    }

    void Seq()
    {
        #region SerilogSeq

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Seq("http://localhost:5341")
            .MinimumLevel.Information()
            .CreateLogger();

        LogManager.Use<SerilogFactory>();

        #endregion
    }

    void ExceptionLogStateUse(Exception exception)
    {
        #region ExceptionLogState

        if (ExceptionLogState.TryReadFromException(exception, out var logState))
        {
            var endpoint = logState.ProcessingEndpoint;
            var correlationId = logState.CorrelationId;
            var conversationId = logState.ConversationId;
            var handlerType = logState.HandlerType;
            var incomingHeaders = logState.IncomingHeaders;
            var incomingMessage = logState.IncomingMessage;
        }

        #endregion
    }
}