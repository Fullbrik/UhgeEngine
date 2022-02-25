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
        if (args.Length > 0)
        {
            var project = EditorProject.Load(args[0]);
            new EditorInteractiveMode(project).Run();
        }
        else
        {
            throw new Exception("You need to pass in a project to continue.");
        }
    }
}