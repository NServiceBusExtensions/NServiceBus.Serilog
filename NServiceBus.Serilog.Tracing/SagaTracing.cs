using NServiceBus.Features;

namespace NServiceBus.Serilog.Tracing
{
    public class SagaTracing : Feature
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SagaTracing"/>.
        /// </summary>
        public SagaTracing()
        {
            EnableByDefault();
            DependsOn<Features.Sagas>();
            DependsOn<TracingLog>();
        }

        /// <summary>
        /// <see cref="Feature.Setup"/>
        /// </summary>
        protected override void Setup(FeatureConfigurationContext context)
        {
            Guard.AgainstNull(context, "context");

            context.Pipeline.Register<CaptureSagaStateBehavior.Registration>();
            context.Pipeline.Register<CaptureSagaResultingMessagesBehavior.Registration>();
        }

    }
}