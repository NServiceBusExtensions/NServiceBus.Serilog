using System;
using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;
using NServiceBus.Saga;
using NServiceBus.Sagas;
using NServiceBus.Transports;
using Serilog;
using Serilog.Events;

namespace NServiceBus.Serilog.Tracing
{

    // ReSharper disable CSharpWarnings::CS0618
    class CaptureSagaStateBehavior : IBehavior<HandlerInvocationContext>
    {
        static ILogger logger = TracingLog.GetLogger("NServiceBus.Serilog.SagaAudit")
                .ForContext("ProcessingEndpoint", Configure.EndpointName);

        public ISendMessages MessageSender { get; set; }
        SagaUpdatedMessage sagaAudit;

        public void Invoke(HandlerInvocationContext context, Action next)
        {
            var saga = context.MessageHandler.Instance as ISaga;

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

        void AuditSaga(ISaga saga, HandlerInvocationContext context)
        {
            string messageId;

            if (!context.LogicalMessage.Headers.TryGetValue(Headers.MessageId, out messageId))
            {
                return;
            }

            var activeSagaInstance = context.Get<ActiveSagaInstance>();
            var headers = context.LogicalMessage.Headers;
            var originatingMachine = headers["NServiceBus.OriginatingMachine"];
            var originatingEndpoint = headers[Headers.OriginatingEndpoint];
            var intent = context.MessageIntent();

            var initiator = new SagaChangeInitiator
            {
                IsSagaTimeoutMessage = context.LogicalMessage.IsTimeoutMessage(),
                InitiatingMessageId = messageId,
                OriginatingMachine = originatingMachine,
                OriginatingEndpoint = originatingEndpoint,
                MessageType = context.LogicalMessage.MessageType.FullName,
                TimeSent = context.TimeSent(),
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
                .Information("Saga execution occurred: {SagaType} {SagaId}");
        }


        void AssignSagaStateChangeCausedByMessage(BehaviorContext context)
        {
            var physicalMessage = context.Get<TransportMessage>(ReceivePhysicalMessageContext.IncomingPhysicalMessageKey);
            string sagaStateChange;

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