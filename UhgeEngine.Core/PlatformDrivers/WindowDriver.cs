namespace UhgeEngine.Core.PlatformDrivers;

public abstract class WindowDriver : IDisposable
{
    public abstract void Init();
    public abstract NativeWindow SpawnWindow(int width, int height, string title);

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~WindowDriver()
    {
        Dispose(false);
    }
}

public abstract class NativeWindow : IDisposable
{
    public abstract void SetTickFunction(Action<float> tick);

    public abstract void Run();

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}