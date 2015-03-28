using NServiceBus.Serilog.Tracing;

namespace NServiceBus.AcceptanceTests.EndpointTemplates
{
    public partial class DefaultServer
    {
        void AddExtraTypes()
        {
            typesToInclude.AddRange(typeof(TracingLog).Assembly.GetTypes());
        }
    }
}