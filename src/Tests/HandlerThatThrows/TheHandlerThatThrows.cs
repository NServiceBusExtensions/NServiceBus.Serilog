public class TheHandlerThatThrows :
    IHandleMessages<StartHandlerThatThrows>
{
    public Task Handle(StartHandlerThatThrows message, HandlerContext context) =>
        throw new();
}