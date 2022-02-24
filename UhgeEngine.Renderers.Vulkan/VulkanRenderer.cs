#if DEBUG // Enable validation layers when in debug mode
//#define USE_VALIDATION_LAYERS
#endif

using System.Runtime.InteropServices;

namespace UhgeEngine.Renderers.Vulkan;

public class VulkanRenderer : IDisposable
{
    private const string ValidationLayerName = "VK_LAYER_KHRONOS_validation";

    private readonly Vk _vk = Vk.GetApi();

    public Instance VulkanInstance => _instance;
    private Instance _instance;

    public unsafe void Init(uint extensionCount, byte** extensions)
    {
        CreateVulkanInstance(extensionCount, extensions);
    }

    private unsafe void CreateVulkanInstance(uint extensionCount, byte** extensions)
    {
        var appName = Marshal.StringToHGlobalAnsi("ApplicationName");
        var engineName = Marshal.StringToHGlobalAnsi("UhgeEngine");
        var appInfo = new ApplicationInfo()
        {
            SType = StructureType.ApplicationInfo,
            PApplicationName = (byte*) appName,
            ApplicationVersion = Vk.MakeVersion(0, 0, 1),
            PEngineName = (byte*) engineName,
            EngineVersion = Vk.MakeVersion(0, 0, 1),
            ApiVersion = Vk.Version12
        };

#if USE_VALIDATION_LAYERS
        if (!CheckValidationLayerSupport())
            throw new Exception("Failed to enable validation layers because they are not available.");

        var strPtr = (byte*) Marshal.StringToHGlobalAnsi(ValidationLayerName);
        var strArr = new[] {strPtr};
        fixed (byte** strArrPtr = strArr)
        {
#endif
            var createInfo = new InstanceCreateInfo()
            {
                SType = StructureType.InstanceCreateInfo,
                PApplicationInfo = &appInfo,
                EnabledExtensionCount = extensionCount,
                PpEnabledExtensionNames = extensions,
#if USE_VALIDATION_LAYERS
                EnabledLayerCount = 1,
                PpEnabledLayerNames = strArrPtr
#else
                EnabledLayerCount = 0
#endif
            };

            VKAssert(_vk.CreateInstance(createInfo, null, out _instance), nameof(_vk.CreateInstance));

#if USE_VALIDATION_LAYERS
        }
#endif

        Marshal.FreeHGlobal(appName);
        Marshal.FreeHGlobal(engineName);
    }

#if USE_VALIDATION_LAYERS
    private bool CheckValidationLayerSupport()
    {
        unsafe
        {
            uint layerCount;
            _vk.EnumerateInstanceLayerProperties(&layerCount, null);

            var availableLayers = new LayerProperties[layerCount];
            _vk.EnumerateInstanceLayerProperties(&layerCount, availableLayers);

            return availableLayers.Any(layer =>
                Marshal.PtrToStringAnsi((IntPtr) layer.LayerName) == ValidationLayerName);
        }
    }
#endif

    public VulkanWindow CreateWindow(SurfaceKHR surface)
    {
        return new VulkanWindow(_vk, _instance, surface);
    }
    
    public void Dispose()
    {
        unsafe
        {
            _vk.DestroyInstance(_instance, null);
        }

        _vk.Dispose();
        GC.SuppressFinalize(this);
    }
}