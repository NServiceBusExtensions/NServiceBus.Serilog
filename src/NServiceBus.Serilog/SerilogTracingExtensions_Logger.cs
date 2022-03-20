namespace NServiceBus;

/// <summary>
/// Extensions to enable and configure Serilog Tracing.
/// </summary>
public static partial class SerilogTracingExtensions
{
    /// <summary>
    /// <see cref="ILogger.Verbose(string)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogVerbose(this IPipelineContext context, string messageTemplate) =>
        context.Logger().Verbose(messageTemplate);

    /// <summary>
    /// <see cref="ILogger.Verbose{T}(string, T)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogVerbose<T>(this IPipelineContext context, string messageTemplate, T propertyValue) =>
        context.Logger().Verbose(messageTemplate, propertyValue);

    /// <summary>
    /// <see cref="ILogger.Verbose{T0, T1}(string, T0, T1)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogVerbose<T0, T1>(this IPipelineContext context, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        context.Logger().Verbose(messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// <see cref="ILogger.Verbose{T0, T1, T2}(string, T0, T1, T2)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogVerbose<T0, T1, T2>(this IPipelineContext context, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        context.Logger().Verbose(messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// <see cref="ILogger.Verbose(string, object[])"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogVerbose(this IPipelineContext context, string messageTemplate, params object[] propertyValues) =>
        context.Logger().Verbose(messageTemplate, propertyValues);

    /// <summary>
    /// <see cref="ILogger.Verbose(Exception, string)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogVerbose(this IPipelineContext context, Exception exception, string messageTemplate) =>
        context.Logger().Verbose(exception, messageTemplate);

    /// <summary>
    /// <see cref="ILogger.Verbose{T}(Exception, string, T)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogVerbose<T>(this IPipelineContext context, Exception exception, string messageTemplate, T propertyValue) =>
        context.Logger().Verbose(exception, messageTemplate, propertyValue);

    /// <summary>
    /// <see cref="ILogger.Verbose{T0, T1}(Exception, string, T0, T1)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogVerbose<T0, T1>(this IPipelineContext context, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        context.Logger().Verbose(exception, messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// <see cref="ILogger.Verbose{T0, T1, T2}(Exception, string, T0, T1, T2)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogVerbose<T0, T1, T2>(this IPipelineContext context, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        context.Logger().Verbose(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// <see cref="ILogger.Verbose(Exception, string, object[])"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogVerbose(this IPipelineContext context, Exception exception, string messageTemplate, params object[] propertyValues) =>
        context.Logger().Verbose(exception, messageTemplate, propertyValues);

    /// <summary>
    /// <see cref="ILogger.Debug(string)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogDebug(this IPipelineContext context, string messageTemplate) =>
        context.Logger().Debug(messageTemplate);

    /// <summary>
    /// <see cref="ILogger.Debug{T}(string, T)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogDebug<T>(this IPipelineContext context, string messageTemplate, T propertyValue) =>
        context.Logger().Debug(messageTemplate, propertyValue);

    /// <summary>
    /// <see cref="ILogger.Debug{T0, T1}(string, T0, T1)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogDebug<T0, T1>(this IPipelineContext context, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        context.Logger().Debug(messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// <see cref="ILogger.Debug{T0, T1, T2}(string, T0, T1, T2)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogDebug<T0, T1, T2>(this IPipelineContext context, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        context.Logger().Debug(messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// <see cref="ILogger.Debug(string, object[])"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogDebug(this IPipelineContext context, string messageTemplate, params object[] propertyValues) =>
        context.Logger().Debug(messageTemplate, propertyValues);

    /// <summary>
    /// <see cref="ILogger.Debug(Exception, string)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogDebug(this IPipelineContext context, Exception exception, string messageTemplate) =>
        context.Logger().Debug(exception, messageTemplate);

    /// <summary>
    /// <see cref="ILogger.Debug{T}(Exception, string, T)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogDebug<T>(this IPipelineContext context, Exception exception, string messageTemplate, T propertyValue) =>
        context.Logger().Debug(exception, messageTemplate, propertyValue);

    /// <summary>
    /// <see cref="ILogger.Debug{T0, T1}(Exception, string, T0, T1)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogDebug<T0, T1>(this IPipelineContext context, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        context.Logger().Debug(exception, messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// <see cref="ILogger.Debug{T0, T1, T2}(Exception, string, T0, T1, T2)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogDebug<T0, T1, T2>(this IPipelineContext context, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        context.Logger().Debug(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// <see cref="ILogger.Debug(Exception, string, object[])"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogDebug(this IPipelineContext context, Exception exception, string messageTemplate, params object[] propertyValues) =>
        context.Logger().Debug(exception, messageTemplate, propertyValues);

    /// <summary>
    /// <see cref="ILogger.Information(string)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogInformation(this IPipelineContext context, string messageTemplate) =>
        context.Logger().Information(messageTemplate);

    /// <summary>
    /// <see cref="ILogger.Information{T}(string, T)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogInformation<T>(this IPipelineContext context, string messageTemplate, T propertyValue) =>
        context.Logger().Information(messageTemplate, propertyValue);

    /// <summary>
    /// <see cref="ILogger.Information{T0, T1}(string, T0, T1)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogInformation<T0, T1>(this IPipelineContext context, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        context.Logger().Information(messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// <see cref="ILogger.Information{T0, T1, T2}(string, T0, T1, T2)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogInformation<T0, T1, T2>(this IPipelineContext context, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        context.Logger().Information(messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// <see cref="ILogger.Information(string, object[])"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogInformation(this IPipelineContext context, string messageTemplate, params object[] propertyValues) =>
        context.Logger().Information(messageTemplate, propertyValues);

    /// <summary>
    /// <see cref="ILogger.Information(Exception, string)"/>
    /// </summary>
    public static void LogInformation(this IPipelineContext context, Exception exception, string messageTemplate) =>
        context.Logger().Information(exception, messageTemplate);

    /// <summary>
    /// <see cref="ILogger.Information{T}(Exception, string, T)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogInformation<T>(this IPipelineContext context, Exception exception, string messageTemplate, T propertyValue) =>
        context.Logger().Information(exception, messageTemplate, propertyValue);

    /// <summary>
    /// <see cref="ILogger.Information{T0, T1}(Exception, string, T0, T1)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogInformation<T0, T1>(this IPipelineContext context, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        context.Logger().Information(exception, messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// <see cref="ILogger.Information{T0, T1, T2}(Exception, string, T0, T1, T2)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogInformation<T0, T1, T2>(this IPipelineContext context, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        context.Logger().Information(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// <see cref="ILogger.Information(Exception, string, object[])"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogInformation(this IPipelineContext context, Exception exception, string messageTemplate, params object[] propertyValues) =>
        context.Logger().Information(exception, messageTemplate, propertyValues);

    /// <summary>
    /// <see cref="ILogger.Warning(string)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogWarning(this IPipelineContext context, string messageTemplate) =>
        context.Logger().Warning(messageTemplate);

    /// <summary>
    /// <see cref="ILogger.Warning{T}(string, T)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogWarning<T>(this IPipelineContext context, string messageTemplate, T propertyValue) =>
        context.Logger().Warning(messageTemplate, propertyValue);

    /// <summary>
    /// <see cref="ILogger.Warning{T0, T1}(string, T0, T1)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogWarning<T0, T1>(this IPipelineContext context, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        context.Logger().Warning(messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// <see cref="ILogger.Warning{T0, T1, T2}(string, T0, T1, T2)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogWarning<T0, T1, T2>(this IPipelineContext context, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        context.Logger().Warning(messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// <see cref="ILogger.Warning(string, object[])"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogWarning(this IPipelineContext context, string messageTemplate, params object[] propertyValues) =>
        context.Logger().Warning(messageTemplate, propertyValues);

    /// <summary>
    /// <see cref="ILogger.Warning(Exception, string)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogWarning(this IPipelineContext context, Exception exception, string messageTemplate) =>
        context.Logger().Warning(exception, messageTemplate);

    /// <summary>
    /// <see cref="ILogger.Warning{T}(Exception, string, T)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogWarning<T>(this IPipelineContext context, Exception exception, string messageTemplate, T propertyValue) =>
        context.Logger().Warning(exception, messageTemplate, propertyValue);

    /// <summary>
    /// <see cref="ILogger.Warning{T0, T1}(Exception, string, T0, T1)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogWarning<T0, T1>(this IPipelineContext context, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        context.Logger().Warning(exception, messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// <see cref="ILogger.Warning{T0, T1, T2}(Exception, string, T0, T1, T2)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogWarning<T0, T1, T2>(this IPipelineContext context, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        context.Logger().Warning(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// <see cref="ILogger.Warning(Exception, string,  object[])"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogWarning(this IPipelineContext context, Exception exception, string messageTemplate, params object[] propertyValues) =>
        context.Logger().Warning(exception, messageTemplate, propertyValues);

    /// <summary>
    /// <see cref="ILogger.Error(string)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogError(this IPipelineContext context, string messageTemplate) =>
        context.Logger().Error(messageTemplate);

    /// <summary>
    /// <see cref="ILogger.Error{T}(string, T)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogError<T>(this IPipelineContext context, string messageTemplate, T propertyValue) =>
        context.Logger().Error(messageTemplate, propertyValue);

    /// <summary>
    /// <see cref="ILogger.Error{T0, T1}(string, T0, T1)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogError<T0, T1>(this IPipelineContext context, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        context.Logger().Error(messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// <see cref="ILogger.Error{T0, T1, T2}(string, T0, T1, T2)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogError<T0, T1, T2>(this IPipelineContext context, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        context.Logger().Error(messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// <see cref="ILogger.Error(string, object[])"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogError(this IPipelineContext context, string messageTemplate, params object[] propertyValues) =>
        context.Logger().Error(messageTemplate, propertyValues);

    /// <summary>
    /// <see cref="ILogger.Error(Exception, string)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogError(this IPipelineContext context, Exception exception, string messageTemplate) =>
        context.Logger().Error(exception, messageTemplate);

    /// <summary>
    /// <see cref="ILogger.Error{T}(Exception, string, T)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogError<T>(this IPipelineContext context, Exception exception, string messageTemplate, T propertyValue) =>
        context.Logger().Error(exception, messageTemplate, propertyValue);

    /// <summary>
    /// <see cref="ILogger.Error{T0, T1}(Exception, string, T0, T1)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogError<T0, T1>(this IPipelineContext context, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        context.Logger().Error(exception, messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// <see cref="ILogger.Error{T0, T1, T2}(Exception, string, T0, T1, T2)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogError<T0, T1, T2>(this IPipelineContext context, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        context.Logger().Error(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// <see cref="ILogger.Error(Exception, string, object[])"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogError(this IPipelineContext context, Exception exception, string messageTemplate, params object[] propertyValues) =>
        context.Logger().Error(exception, messageTemplate, propertyValues);

    /// <summary>
    /// <see cref="ILogger.Fatal(string)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogFatal(this IPipelineContext context, string messageTemplate) =>
        context.Logger().Fatal(messageTemplate);

    /// <summary>
    /// <see cref="ILogger.Fatal{T}(string, T)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogFatal<T>(this IPipelineContext context, string messageTemplate, T propertyValue) =>
        context.Logger().Fatal(messageTemplate, propertyValue);

    /// <summary>
    /// <see cref="ILogger.Fatal{T0, T1}(string, T0, T1)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogFatal<T0, T1>(this IPipelineContext context, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        context.Logger().Fatal(messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// <see cref="ILogger.Fatal{T0, T1, T2}(string, T0, T1, T2)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogFatal<T0, T1, T2>(this IPipelineContext context, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        context.Logger().Fatal(messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// <see cref="ILogger.Fatal(string, object[])"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogFatal(this IPipelineContext context, string messageTemplate, params object[] propertyValues) =>
        context.Logger().Fatal(messageTemplate, propertyValues);

    /// <summary>
    /// <see cref="ILogger.Fatal(Exception, string)"/>
    /// </summary>
    public static void LogFatal(this IPipelineContext context, Exception exception, string messageTemplate) =>
        context.Logger().Fatal(exception, messageTemplate);

    /// <summary>
    /// <see cref="ILogger.Fatal{T}(Exception, string, T)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogFatal<T>(this IPipelineContext context, Exception exception, string messageTemplate, T propertyValue) =>
        context.Logger().Fatal(exception, messageTemplate, propertyValue);

    /// <summary>
    /// <see cref="ILogger.Fatal{T0, T1}(Exception, string, T0, T1)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogFatal<T0, T1>(this IPipelineContext context, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        context.Logger().Fatal(exception, messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// <see cref="ILogger.Fatal{T0, T1, T2}(Exception, string, T0, T1, T2)"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogFatal<T0, T1, T2>(this IPipelineContext context, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        context.Logger().Fatal(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// <see cref="ILogger.Fatal(Exception, string, object[])"/>
    /// </summary>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void LogFatal(this IPipelineContext context, Exception exception, string messageTemplate, params object[] propertyValues) =>
        context.Logger().Fatal(exception, messageTemplate, propertyValues);
}