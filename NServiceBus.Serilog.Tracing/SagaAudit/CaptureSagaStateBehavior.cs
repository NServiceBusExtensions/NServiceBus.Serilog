
namespace NServiceBus.Serilog.Tracing
{
    using System;
    using global::Serilog;
    using global::Serilog.Events;
    using NServiceBus.Pipeline;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Sagas;
    using Saga = NServiceBus.Saga.Saga;

    class CaptureSagaStateBehavior : IBehavior<IncomingContext>
    {
        SagaUpdatedMessage sagaAudit;
        ILogger logger;

        public CaptureSagaStateBehavior(LogBuilder logBuilder)
        {
            logger = logBuilder.GetLogger("NServiceBus.Serilog.SagaAudit");
        }

        public void Invoke(IncomingContext context, Action next)
        {
            var saga = context.MessageHandler.Instance as Saga;

            if (saga == null)
            {
                next();
                return;
            }

            if (!logger.IsEnabled(LogEventLevel.Information))
            {
                next();
                return;
            }
            sagaAudit = new SagaUpdatedMessage
            {
                StartTime = DateTime.UtcNow,
                SagaType = saga.GetType().FullName
            };
            context.Set(sagaAudit);
            next();

            if (saga.Entity == null)
            {
                return; // Message was not handled by the saga
            }

            sagaAudit.FinishTime = DateTime.UtcNow;
            AuditSaga(saga, context);
        }

        void AuditSaga(Saga saga, IncomingContext context)
        {
            string messageId;

            if (!context.IncomingLogicalMessage.Headers.TryGetValue(Headers.MessageId, out messageId))
            {
                return;
            }

            var activeSagaInstance = context.Get<ActiveSagaInstance>();
            var headers = context.IncomingLogicalMessage.Headers;
            var originatingMachine = headers["NServiceBus.OriginatingMachine"];
            var originatingEndpoint = headers[Headers.OriginatingEndpoint];
            var intent = context.IncomingLogicalMessage.MessageIntent();

            var initiator = new SagaChangeInitiator
            {
                IsSagaTimeoutMessage = context.IncomingLogicalMessage.IsTimeoutMessage(),
                InitiatingMessageId = messageId,
                OriginatingMachine = originatingMachine,
                OriginatingEndpoint = originatingEndpoint,
                MessageType = context.IncomingLogicalMessage.MessageType.FullName,
                TimeSent = context.IncomingLogicalMessage.TimeSent(),
                Intent = intent
            };
            sagaAudit.IsNew = activeSagaInstance.IsNew;
            sagaAudit.IsCompleted = saga.Completed;
            sagaAudit.SagaId = saga.Entity.Id;

            AssignSagaStateChangeCausedByMessage(context);

            logger
                .ForContext("SagaId", sagaAudit.SagaId)
                .ForContext("SagaType", sagaAudit.SagaType)
                .ForContext("StartTime", sagaAudit.StartTime)
                .ForContext("FinishTime", sagaAudit.FinishTime)
                .ForContext("IsCompleted", sagaAudit.IsCompleted)
                .ForContext("IsNew", sagaAudit.IsNew)
                .ForContext("Initiator", initiator, true)
                .ForContext("ResultingMessages", sagaAudit.ResultingMessages, true)
                .ForContext("SagaState", saga.Entity, true)
                .Information("Saga execution {SagaType} {SagaId}");
        }


        void AssignSagaStateChangeCausedByMessage(IncomingContext context)
        {
            string sagaStateChange;

            var physicalMessage = context.PhysicalMessage;
            if (!physicalMessage.Headers.TryGetValue("NServiceBus.Serilog.Tracing.SagaStateChange", out sagaStateChange))
            {
                sagaStateChange = String.Empty;
            }

            var statechange = "Updated";
            if (sagaAudit.IsNew)
            {
                statechange = "New";
            }
            if (sagaAudit.IsCompleted)
            {
                statechange = "Completed";
            }

            if (!String.IsNullOrEmpty(sagaStateChange))
            {
                sagaStateChange += ";";
            }
            sagaStateChange += String.Format("{0}:{1}", sagaAudit.SagaId, statechange);

            physicalMessage.Headers["NServiceBus.Serilog.Tracing.SagaStateChange"] = sagaStateChange;
        }


    }

}