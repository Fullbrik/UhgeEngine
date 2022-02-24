using UhgeEngine.Core.Assets;
using UhgeEngine.Core.PlatformDrivers;

namespace UhgeEngine.Core;

public class Engine : IDisposable
{
    public DriverCollection Drivers { get; }
    private readonly List<Mod> _mods = new();
    private readonly List<Window> _windows = new();

    private readonly AssetDB _assetDb = new AssetDB();

    public Engine(DriverCollection drivers)
    {
        Drivers = drivers;
    }

    public void LoadMod(Mod mod)
    {
        _mods.Add(mod);
        _assetDb.LoadAssetBundle(mod.AssetBundle);
    }

    public void Run()
    {
        Drivers.Init();
        
        var world = CreateStartingWorld();
        var window = CreateWindow(world);
        window.Run();
    }

    public World CreateStartingWorld()
    {
        var world = new World();
        return world;
    }

    public Window CreateWindow(World world)
    {
        var window = new Window(this, world);
        _windows.Add(window);
        return window;
    }

    public void Dispose()
    {
        foreach(var window in _windows) window.Dispose();
        Drivers.Dispose();
        GC.SuppressFinalize(this);
    }
}