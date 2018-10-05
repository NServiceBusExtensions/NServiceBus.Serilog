using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using NServiceBus.Sagas;
using NServiceBus.Serilog.Tracing;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;

class CaptureSagaStateBehavior : Behavior<IInvokeHandlerContext>
{
    internal const string Key = "NServiceBus.Serilog.SagaAudit";
    SagaUpdatedMessage sagaAudit;
    ILogger logger;
    MessageTemplate messageTemplate;

    public CaptureSagaStateBehavior(LogBuilder logBuilder)
    {
        var templateParser = new MessageTemplateParser();
        messageTemplate = templateParser.Parse("Saga execution '{SagaType}' '{SagaId}'.");

        logger = logBuilder.GetLogger(Key);
    }

    public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        if (!(context.MessageHandler.Instance is Saga))
        {
            // Message was not handled by the saga
            await next().ConfigureAwait(false);
            return;
        }

        if (!logger.IsEnabled(LogEventLevel.Information))
        {
            await next().ConfigureAwait(false);
            return;
        }

        sagaAudit = new SagaUpdatedMessage
        {
            StartTime = DateTime.UtcNow
        };
        context.Extensions.Set(sagaAudit);
        await next()
            .ConfigureAwait(false);

        if (context.Extensions.TryGet(out ActiveSagaInstance activeSagaInstance))
        {
            sagaAudit.SagaType = activeSagaInstance.Instance.GetType().FullName;

            sagaAudit.FinishTime = DateTime.UtcNow;
            AuditSaga(activeSagaInstance, context);
        }
    }

    void AuditSaga(ActiveSagaInstance activeSagaInstance, IInvokeHandlerContext context)
    {
        var saga = activeSagaInstance.Instance;

        if (saga.Entity == null)
        {
            if (context.IsTimeoutMessage())
            {
                //Receiving a timeout for a saga that has completed
                return;
            }
            throw new Exception("Expected saga.Entity to contain a value.");
        }
        if (!context.Headers.TryGetValue(Headers.MessageId, out var messageId))
        {
            return;
        }

        var headers = context.Headers;
        var originatingMachine = headers["NServiceBus.OriginatingMachine"];
        var originatingEndpoint = headers[Headers.OriginatingEndpoint];
        var intent = context.MessageIntent();

        var initiator = new SagaChangeInitiator
        {
            IsSagaTimeoutMessage = context.IsTimeoutMessage(),
            InitiatingMessageId = messageId,
            OriginatingMachine = originatingMachine,
            OriginatingEndpoint = originatingEndpoint,
            MessageType = context.MessageMetadata.MessageType.FullName,
            TimeSent = context.TimeSent(),
            Intent = intent
        };
        sagaAudit.IsNew = activeSagaInstance.IsNew;
        sagaAudit.IsCompleted = saga.Completed;
        sagaAudit.SagaId = saga.Entity.Id;

        AssignSagaStateChangeCausedByMessage(context);

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

    void AssignSagaStateChangeCausedByMessage(IInvokeHandlerContext context)
    {
        if (!context.Headers.TryGetValue("NServiceBus.Serilog.Tracing.SagaStateChange", out var sagaStateChange))
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

        context.Headers["NServiceBus.Serilog.Tracing.SagaStateChange"] = sagaStateChange;
    }

    public class Registration : RegisterStep
    {
        public Registration(LogBuilder logBuilder)
            : base(
                stepId: "SerilogCaptureSagaState",
                behavior: typeof(CaptureSagaStateBehavior),
                description: "Records saga state changes",
                factoryMethod: builder => new CaptureSagaStateBehavior(logBuilder))
        {
            InsertBefore("InvokeSaga");
        }
    }
}