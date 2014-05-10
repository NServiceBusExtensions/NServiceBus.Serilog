using NServiceBus.Features;

namespace NServiceBus.Serilog.Tracing
{

    public class SerilogSagaAudit:Feature
    {
        public override void Initialize()
        {
            
        }

        public override bool IsEnabledByDefault
        {
            get { return true; }
        }

        public override bool ShouldBeEnabled()
        {
            return IsEnabled<Features.Sagas>();
        }
    }
}