﻿using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Testing;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class InjectIncomingBehaviorTests
{
    [Fact]
    public async Task Empty()
    {
        var logBuilder = new LogBuilder(new FakeLogger(), "endpoint");
        var behavior = new InjectIncomingBehavior(logBuilder, "endpoint");
        var context = new TestableIncomingPhysicalMessageContext();
        await behavior.Invoke(context, () => Task.CompletedTask);
        await Verifier.Verify(context);
    }

    [Fact]
    public async Task WithMessageTypeFullName()
    {
        var logBuilder = new LogBuilder(new FakeLogger(), "endpoint");
        var behavior = new InjectIncomingBehavior(logBuilder, "endpoint");
        var context = new TestableIncomingPhysicalMessageContext();
        context.MessageHeaders.Add(Headers.EnclosedMessageTypes, typeof(Message1).FullName);
        await behavior.Invoke(context, () => Task.CompletedTask);
        await Verifier.Verify(context);
    }

    [Fact]
    public async Task WithMessageTypeAssemblyQualifiedName()
    {
        var logBuilder = new LogBuilder(new FakeLogger(), "endpoint");
        var behavior = new InjectIncomingBehavior(logBuilder, "endpoint");
        var context = new TestableIncomingPhysicalMessageContext();
        context.MessageHeaders.Add(Headers.EnclosedMessageTypes, typeof(Message1).AssemblyQualifiedName);
        await behavior.Invoke(context, () => Task.CompletedTask);
        await Verifier.Verify(context);
    }

    [Fact]
    public async Task WithMultipleMessageTypesFullName()
    {
        var logBuilder = new LogBuilder(new FakeLogger(), "endpoint");
        var behavior = new InjectIncomingBehavior(logBuilder, "endpoint");
        var context = new TestableIncomingPhysicalMessageContext();
        context.MessageHeaders.Add(Headers.EnclosedMessageTypes, $"{typeof(Message1).FullName};{typeof(Message2).FullName}");
        await behavior.Invoke(context, () => Task.CompletedTask);
        await Verifier.Verify(context);
    }

    [Fact]
    public async Task WithMultipleMessageTypesAssemblyQualifiedName()
    {
        var logBuilder = new LogBuilder(new FakeLogger(), "endpoint");
        var behavior = new InjectIncomingBehavior(logBuilder, "endpoint");
        var context = new TestableIncomingPhysicalMessageContext();
        context.MessageHeaders.Add(Headers.EnclosedMessageTypes, $"{typeof(Message1).AssemblyQualifiedName};{typeof(Message2).AssemblyQualifiedName}");
        await behavior.Invoke(context, () => Task.CompletedTask);
        await Verifier.Verify(context);
    }

    class Message1
    {

    }

    class Message2
    {

    }
}