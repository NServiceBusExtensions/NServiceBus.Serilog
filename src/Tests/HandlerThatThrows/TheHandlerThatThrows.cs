public class TheHandlerThatThrows :
    IHandleMessages<StartHandlerThatThrows>
{
    public Task Handle(StartHandlerThatThrows message, IMessageHandlerContext context) =>
        throw new();
}