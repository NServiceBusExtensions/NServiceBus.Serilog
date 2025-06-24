﻿[TestFixture]
public class IncomingPhysicalBehaviorTests
{
    [Test]
    public async Task Empty()
    {
        var behavior = new IncomingPhysicalBehavior("endpoint");
        var context = new RecordingIncomingPhysicalMessageContext();
        Recording.Start();
        await behavior.Invoke(context, TestExtensions.WriteLog);
        await Verify(context);
    }

    [Test]
    public async Task WithMessageTypeFullName()
    {
        var behavior = new IncomingPhysicalBehavior("endpoint");
        var context = new RecordingIncomingPhysicalMessageContext(
            headers: [new(Headers.EnclosedMessageTypes, typeof(Message1).FullName!)]);
        Recording.Start();
        await behavior.Invoke(context, TestExtensions.WriteLog);
        await Verify(context);
    }

    [Test]
    public async Task WithMessageTypeAssemblyQualifiedName()
    {
        var behavior = new IncomingPhysicalBehavior("endpoint");
        var context = new RecordingIncomingPhysicalMessageContext(
            headers: [new(Headers.EnclosedMessageTypes, typeof(Message1).AssemblyQualifiedName!)]);
        Recording.Start();
        await behavior.Invoke(context, TestExtensions.WriteLog);
        await Verify(context);
    }

    [Test]
    public async Task WithMultipleMessageTypesFullName()
    {
        var behavior = new IncomingPhysicalBehavior("endpoint");
        var context = new RecordingIncomingPhysicalMessageContext(
            headers: [new(Headers.EnclosedMessageTypes, $"{typeof(Message1).FullName};{typeof(Message2).FullName}")]);
        Recording.Start();
        await behavior.Invoke(context, TestExtensions.WriteLog);
        await Verify(context);
    }

    [Test]
    public async Task WithMultipleMessageTypesAssemblyQualifiedName()
    {
        var behavior = new IncomingPhysicalBehavior("endpoint");
        var context = new RecordingIncomingPhysicalMessageContext(
            headers: [new(Headers.EnclosedMessageTypes, $"{typeof(Message1).AssemblyQualifiedName};{typeof(Message2).AssemblyQualifiedName}")]);
        Recording.Start();
        await behavior.Invoke(context, TestExtensions.WriteLog);
        await Verify(context);
    }

    class Message1;

    class Message2;
}