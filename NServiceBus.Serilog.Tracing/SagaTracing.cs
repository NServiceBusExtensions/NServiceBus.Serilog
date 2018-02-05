using NServiceBus.Features;

namespace NServiceBus.Serilog.Tracing
{
    /// <summary>
    /// The serilog saga tracing feature.
    /// </summary>
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

            var pipeline = context.Pipeline;
            pipeline.Register<CaptureSagaStateBehavior.Registration>();
            pipeline.Register<CaptureSagaResultingMessagesBehavior.Registration>();
        }
    }
}