using System.Diagnostics;
using UhgeEngine.Core;
using UhgeEngine.Core.Assets;
using UhgeEngine.Core.PlatformDrivers;
using UhgeEngine.Targets.Desktop;

namespace UhgeEngine.Tools.Editor;

internal static class Program
{
    private static void Main(string[] args)
    {
        Engine engine = CreateEngine();
        
        if (args.Length > 0)
        {
            var projectFilePath = Path.GetFullPath(args[0]);
            var mod = LoadModFromProjectFilePath(projectFilePath);
        }
        
        
    }

    private static Engine CreateEngine()
    {
        var engine = new Engine(new DriverCollection(new DesktopWindowDriver()));

        return engine;
    }

    private static Mod LoadModFromProjectFilePath(string projectFilePath)
    {
        projectFilePath = Path.GetFullPath(projectFilePath);
        
        Debug.Assert(File.Exists(projectFilePath), "Unable to find project file {0}", projectFilePath);

        var projectFolderPath = Path.GetDirectoryName(projectFilePath)?? throw new Exception("Unable to get directory of project file.");
        
        var assetBundle = new FsAssetBundle(Path.Combine(projectFilePath, "Content"));
        
        var mod = new Mod("ExampleMod", assetBundle);

        return mod;
    }
}