public class StartGenericHandler<T>:
    IMessage
{
    public T? Property { get; set; }
}