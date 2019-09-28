using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using NServiceBus.Sagas;
using NServiceBus.Serilog;
using Serilog.Events;
using Serilog.Parsing;

class CaptureSagaStateBehavior :
    Behavior<IInvokeHandlerContext>
{
    MessageTemplate messageTemplate;

    public CaptureSagaStateBehavior()
    {
        var templateParser = new MessageTemplateParser();
        messageTemplate = templateParser.Parse("Saga execution '{SagaType}' '{SagaId}'.");
    }

    public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        if (!(context.MessageHandler.Instance is Saga))
        {
            // Message was not handled by the saga
            await next();
            return;
        }

        var logger = context.Logger();
        if (!logger.IsEnabled(LogEventLevel.Information))
        {
            await next();
            return;
        }

        var sagaAudit = new SagaUpdatedMessage(DateTimeOffset.UtcNow);
        context.Extensions.Set(sagaAudit);
        await next();

        if (context.Extensions.TryGet(out ActiveSagaInstance activeSagaInstance))
        {
            sagaAudit.SagaType = activeSagaInstance.Instance.GetType().FullName;

            sagaAudit.FinishTime = DateTimeOffset.UtcNow;
            AuditSaga(activeSagaInstance, context, sagaAudit);
        }
    }

    void AuditSaga(ActiveSagaInstance activeSagaInstance, IInvokeHandlerContext context, SagaUpdatedMessage sagaAudit)
    {
        var saga = activeSagaInstance.Instance;

        if (saga.Entity == null)
        {
            //this can happen if it is a timeout or for invoking "saga not found" logic
            return;
        }

        var headers = context.Headers;
        if (!headers.TryGetValue(Headers.MessageId, out var messageId))
        {
            return;
        }

        var intent = context.MessageIntent();

        var initiator = new SagaChangeInitiator
        (
            isSagaTimeoutMessage: context.IsTimeoutMessage(),
            initiatingMessageId: messageId,
            originatingMachine: context.OriginatingMachine(),
            originatingEndpoint: context.OriginatingEndpoint(),
            messageType: context.MessageType(),
            timeSent: context.TimeSent(),
            intent: intent
        );
        sagaAudit.IsNew = activeSagaInstance.IsNew;
        sagaAudit.IsCompleted = saga.Completed;
        sagaAudit.SagaId = saga.Entity.Id;

        AssignSagaStateChangeCausedByMessage(context, sagaAudit);

        var properties = new List<LogEventProperty>
        {
            new LogEventProperty("SagaType", new ScalarValue(sagaAudit.SagaType)),
            new LogEventProperty("SagaId", new ScalarValue(sagaAudit.SagaId)),
            new LogEventProperty("StartTime", new ScalarValue(sagaAudit.StartTime)),
            new LogEventProperty("FinishTime", new ScalarValue(sagaAudit.FinishTime)),
            new LogEventProperty("IsCompleted", new ScalarValue(sagaAudit.IsCompleted)),
            new LogEventProperty("IsNew", new ScalarValue(sagaAudit.IsNew)),
            new LogEventProperty("SagaType", new ScalarValue(sagaAudit.SagaType)),
        };

        var logger = context.Logger();
        if (logger.BindProperty("Initiator", initiator, out var initiatorProperty))
        {
            properties.Add(initiatorProperty);
        }

        if (logger.BindProperty("ResultingMessages", sagaAudit.ResultingMessages, out var resultingMessagesProperty))
        {
            properties.Add(resultingMessagesProperty);
        }

        if (logger.BindProperty("Entity", saga.Entity, out var sagaEntityProperty))
        {
            properties.Add(sagaEntityProperty);
        }

        logger.WriteInfo(messageTemplate, properties);
    }

    void AssignSagaStateChangeCausedByMessage(IInvokeHandlerContext context, SagaUpdatedMessage sagaAudit)
    {
        if (!context.Headers.TryGetValue("NServiceBus.Serilog.SagaStateChange", out var sagaStateChange))
        {
            sagaStateChange = string.Empty;
        }

        var stateChange = "Updated";
        if (sagaAudit.IsNew)
        {
            stateChange = "New";
        }

        if (sagaAudit.IsCompleted)
        {
            stateChange = "Completed";
        }

        if (!string.IsNullOrEmpty(sagaStateChange))
        {
            sagaStateChange += ";";
        }

        sagaStateChange += $"{sagaAudit.SagaId}:{stateChange}";

        context.Headers["NServiceBus.Serilog.SagaStateChange"] = sagaStateChange;
    }

    public class Registration : RegisterStep
    {
        public Registration()
            : base(
                stepId: $"Serilog{nameof(CaptureSagaStateBehavior)}",
                behavior: typeof(CaptureSagaStateBehavior),
                description: "Records saga state changes")
        {
            InsertBefore("InvokeSaga");
        }
    }
}