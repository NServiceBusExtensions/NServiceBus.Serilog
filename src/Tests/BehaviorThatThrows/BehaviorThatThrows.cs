using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

class BehaviorThatThrows :
    Behavior<IIncomingLogicalMessageContext >
{
    public class Registration :
        RegisterStep
    {
        public Registration() :
            base(
                stepId: $"Serilog{nameof(BehaviorThatThrows)}",
                behavior: typeof(BehaviorThatThrows),
                description: "Foo")
        {
        }
    }

    public override Task Invoke(IIncomingLogicalMessageContext  context, Func<Task> next)
    {
        throw new Exception("The Exception");
    }
}