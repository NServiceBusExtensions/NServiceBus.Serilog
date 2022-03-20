class CaptureSagaStateBehavior :
    Behavior<IInvokeHandlerContext>
{
    MessageTemplate messageTemplate;

    CaptureSagaStateBehavior()
    {
        var templateParser = new MessageTemplateParser();
        messageTemplate = templateParser.Parse("Saga execution {SagaType} {SagaId}.");
    }

    public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        if (context.MessageHandler.Instance is not Saga)
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
            var sagaType = activeSagaInstance.Instance.GetType().Name;
            sagaAudit.SagaType = sagaType;
            sagaAudit.FinishTime = DateTimeOffset.UtcNow;
            AuditSaga(activeSagaInstance, context, sagaAudit);
        }
    }

    void AuditSaga(ActiveSagaInstance activeSagaInstance, IInvokeHandlerContext context, SagaUpdatedMessage sagaAudit)
    {
        var saga = activeSagaInstance.Instance;

        if (saga.Entity is null)
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

        sagaAudit.IsNew = activeSagaInstance.IsNew;
        sagaAudit.IsCompleted = saga.Completed;
        sagaAudit.SagaId = saga.Entity.Id;

        AssignSagaStateChangeCausedByMessage(context, sagaAudit);

        var properties = new List<LogEventProperty>
        {
            new("SagaType", new ScalarValue(sagaAudit.SagaType)),
            new("SagaId", new ScalarValue(sagaAudit.SagaId)),
            new("StartTime", new ScalarValue(sagaAudit.StartTime)),
            new("FinishTime", new ScalarValue(sagaAudit.FinishTime)),
            new("IsCompleted", new ScalarValue(sagaAudit.IsCompleted)),
            new("IsNew", new ScalarValue(sagaAudit.IsNew))
        };

        var logger = context.Logger();
        var messageType = TypeNameConverter.GetName(context.MessageType());

        var initiator = new Dictionary<ScalarValue, LogEventPropertyValue>
        {
            {new("IsSagaTimeout"), new ScalarValue(context.IsTimeoutMessage())},
            {new("MessageId"), new ScalarValue(messageId)},
            {new("OriginatingMachine"), new ScalarValue(context.OriginatingMachine())},
            {new("OriginatingEndpoint"), new ScalarValue(context.OriginatingEndpoint())},
            {new("MessageType"), new ScalarValue(messageType)},
            {new("TimeSent"), new ScalarValue(context.TimeSent())},
            {new("Intent"), new ScalarValue(intent)}
        };
        properties.Add(new("Initiator", new DictionaryValue(initiator)));

        if (sagaAudit.ResultingMessages.Any())
        {
            if (logger.BindProperty("ResultingMessages", sagaAudit.ResultingMessages, out var resultingMessagesProperty))
            {
                properties.Add(resultingMessagesProperty);
            }
        }

        if (logger.BindProperty("Entity", saga.Entity, out var sagaEntityProperty))
        {
            properties.Add(sagaEntityProperty);
        }

        logger.WriteInfo(messageTemplate, properties);
    }

    static void AssignSagaStateChangeCausedByMessage(IInvokeHandlerContext context, SagaUpdatedMessage sagaAudit)
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

    public class Registration :
        RegisterStep
    {
        public Registration() :
            base(
                stepId: $"Serilog{nameof(CaptureSagaStateBehavior)}",
                behavior: typeof(CaptureSagaStateBehavior),
                description: "Records saga state changes",
                factoryMethod: _ => new CaptureSagaStateBehavior()) =>
            InsertBefore("InvokeSaga");
    }
}