public static class ModuleInit
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings
            .ScrubMember("ElapsedTime");
        VerifierSettings.AddExtraDateTimeOffsetFormat("yyyy-MM-ddTHH:mm:ss.fffzz");
        VerifierSettings.InitializePlugins();
    }
}