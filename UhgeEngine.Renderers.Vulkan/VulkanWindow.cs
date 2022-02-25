using Silk.NET.Vulkan.Extensions.KHR;

namespace UhgeEngine.Renderers.Vulkan;

public class VulkanWindow : IDisposable
{
    private readonly Vk _vk;
    private readonly KhrSurface _surfaceApi;
    private readonly Instance _instance;
    private readonly VulkanDeviceManager _deviceManager;
    private readonly SurfaceKHR _surface;

    public VulkanWindow(Vk vk, Instance instance, SurfaceKHR surface)
    {
        _vk = vk;
        if (!vk.TryGetInstanceExtension(_instance, out _surfaceApi))
            throw new Exception("Failed to get surface api, which is required for managing vulkan windows.");
        _instance = instance;
        _deviceManager = new VulkanDeviceManager(_vk, _instance, _surfaceApi, _surface);
        _surface = surface;
    }

    public void Init()
    {
        _deviceManager.Init();
    }

    private unsafe void ReleaseUnmanagedResources()
    {
        _surfaceApi.DestroySurface(_instance, _surface, null);
    }

    private void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        if (disposing)
        {
            _surfaceApi.Dispose();
            _deviceManager.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~VulkanWindow()
    {
        Dispose(false);
    }
}