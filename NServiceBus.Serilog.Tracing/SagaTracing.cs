using NServiceBus.Features;
using NServiceBus.Pipeline;

namespace NServiceBus.Serilog.Tracing {
    public class SagaTracing : Feature 
    {
        public SagaTracing() 
        {
            EnableByDefault();
            DependsOn<Features.Sagas>();
            DependsOn<TracingLog>();
        }

        protected override void Setup(FeatureConfigurationContext context) 
        {
            context.Pipeline.Register<CaptureSagaStateRegistration>();
            context.Pipeline.Register<CaptureSagaResultingMessageRegistration>();
        }

        class CaptureSagaStateRegistration : RegisterStep 
        {
            public CaptureSagaStateRegistration()
                : base("SerilogCaptureSagaState", typeof(CaptureSagaStateBehavior), "Records saga state changes") 
            {
                InsertBefore(WellKnownStep.InvokeSaga);
            }
        }

        class CaptureSagaResultingMessageRegistration : RegisterStep 
        {
            public CaptureSagaResultingMessageRegistration()
                : base("SerilogReportSagaStateChanges", typeof(CaptureSagaResultingMessagesBehavior), "Reports the saga state changes to Serilog") 
            {
                InsertAfter(WellKnownStep.InvokeSaga);
            }
        }
    }
}
