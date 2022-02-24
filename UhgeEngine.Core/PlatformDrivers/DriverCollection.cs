namespace UhgeEngine.Core.PlatformDrivers;

public class DriverCollection : IDisposable
{
    public WindowDriver Window { get; }

    public DriverCollection(WindowDriver window)
    {
        Window = window;
    }

    public void Init()
    {
        Window.Init();
    }

    public void Dispose()
    {
        Window.Dispose();
        GC.SuppressFinalize(this);
    }
}