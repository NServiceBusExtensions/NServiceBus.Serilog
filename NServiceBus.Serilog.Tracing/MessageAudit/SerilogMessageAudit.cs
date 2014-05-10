using NServiceBus.Features;

namespace NServiceBus.Serilog.Tracing
{

    public class SerilogMessageAudit:Feature
    {
        public override void Initialize()
        {
            
        }

        public override bool IsEnabledByDefault
        {
            get { return true; }
        }
    }
}