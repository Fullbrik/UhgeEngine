using UhgeEngine.Core.PlatformDrivers;

namespace UhgeEngine.Core;

public class Window : IDisposable
{
    public Engine Engine { get; }
    public World World { get; }

    private NativeWindow? _nativeWindow;

    public Window(Engine engine, World world)
    {
        Engine = engine;
        World = world;
    }

    public void Run()
    {
        var driver = Engine.Drivers.Window;
        _nativeWindow = driver.SpawnWindow(500, 500, "My Window");
        _nativeWindow.SetTickFunction(Tick);
        _nativeWindow.Run();
    }

    private void Tick(float deltaTime)
    {
        World.Tick(deltaTime);
    }

    public void Dispose()
    {
        _nativeWindow?.Dispose();
        GC.SuppressFinalize(this);
    }
}