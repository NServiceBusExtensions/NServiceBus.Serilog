using System.IO;

public static class AssemblyLocation
{
    static AssemblyLocation()
    {
        var assembly = typeof(AssemblyLocation).Assembly;

        var path = assembly.Location
            .Replace("file:///", "")
            .Replace("file://", "")
            .Replace(@"file:\\\", "")
            .Replace(@"file:\\", "");


        SourceDirectory = new DirectoryInfo(Path.GetDirectoryName(path)).Parent.Parent.Parent.FullName;
    }

    public static string SourceDirectory;
}