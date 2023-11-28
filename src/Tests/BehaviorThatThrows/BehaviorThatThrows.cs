class BehaviorThatThrows :
    Behavior<IInvokeHandlerContext>
{
    public class Registration() :
        RegisterStep(
            stepId: $"Serilog{nameof(BehaviorThatThrows)}",
            behavior: typeof(BehaviorThatThrows),
            description: "Foo");

    public override Task Invoke(IInvokeHandlerContext context, Func<Task> next) =>
        throw new("The Exception");
}