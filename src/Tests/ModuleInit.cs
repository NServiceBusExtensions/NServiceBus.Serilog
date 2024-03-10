public static class ModuleInit
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings
            .ScrubMember("ElapsedTime");
        VerifierSettings.AddExtraDatetimeOffsetFormat("yyyy-MM-ddTHH:mm:ss.fffzz");
        VerifierSettings.InitializePlugins();
    }
}