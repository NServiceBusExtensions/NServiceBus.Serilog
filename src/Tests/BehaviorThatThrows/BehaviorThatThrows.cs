using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

class BehaviorThatThrows :
    Behavior<IInvokeHandlerContext>
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

    public override Task Invoke(IInvokeHandlerContext context, Func<Task> next)
    {
        throw new Exception("The Exception");
    }
}