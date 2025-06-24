﻿[TestFixture]
public class InjectIncomingPhysicalBehaviorTests
{
    [Test]
    public async Task Empty()
    {
        var logBuilder = new LogBuilder(new FakeLogger(), "endpoint");
        var behavior = new InjectIncomingPhysicalBehavior(logBuilder, "endpoint");
        var context = new RecordingIncomingPhysicalMessageContext();
        Recording.Start();
        await behavior.Invoke(context, () => Task.CompletedTask);
        await Verify(context);
    }

    [Test]
    public async Task WithMessageTypeFullName()
    {
        var logBuilder = new LogBuilder(new FakeLogger(), "endpoint");
        var behavior = new InjectIncomingPhysicalBehavior(logBuilder, "endpoint");
        var context = new RecordingIncomingPhysicalMessageContext(
            headers: [new(Headers.EnclosedMessageTypes, typeof(Message1).FullName!)]);
        Recording.Start();
        await behavior.Invoke(context, () => Task.CompletedTask);
        await Verify(context);
    }

    [Test]
    public async Task WithMessageTypeAssemblyQualifiedName()
    {
        var logBuilder = new LogBuilder(new FakeLogger(), "endpoint");
        var behavior = new InjectIncomingPhysicalBehavior(logBuilder, "endpoint");
        var context = new RecordingIncomingPhysicalMessageContext(
            headers: [new(Headers.EnclosedMessageTypes, typeof(Message1).AssemblyQualifiedName!)]);
        Recording.Start();
        await behavior.Invoke(context, () => Task.CompletedTask);
        await Verify(context);
    }

    [Test]
    public async Task WithMultipleMessageTypesFullName()
    {
        var logBuilder = new LogBuilder(new FakeLogger(), "endpoint");
        var behavior = new InjectIncomingPhysicalBehavior(logBuilder, "endpoint");
        var context = new RecordingIncomingPhysicalMessageContext(
            headers: [new(Headers.EnclosedMessageTypes, $"{typeof(Message1).FullName};{typeof(Message2).FullName}")]);
        Recording.Start();
        await behavior.Invoke(context, () => Task.CompletedTask);
        await Verify(context);
    }

    [Test]
    public async Task WithMultipleMessageTypesAssemblyQualifiedName()
    {
        var logBuilder = new LogBuilder(new FakeLogger(), "endpoint");
        var behavior = new InjectIncomingPhysicalBehavior(logBuilder, "endpoint");
        var context = new RecordingIncomingPhysicalMessageContext(
            headers: [new(Headers.EnclosedMessageTypes, $"{typeof(Message1).AssemblyQualifiedName};{typeof(Message2).AssemblyQualifiedName}")]);
        Recording.Start();
        await behavior.Invoke(context, () => Task.CompletedTask);
        await Verify(context);
    }

    class Message1;

    class Message2;
}