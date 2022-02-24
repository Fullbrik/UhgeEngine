using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using UhgeEngine.Core.PlatformDrivers;
using UhgeEngine.Renderers.Vulkan;
using Silk.NET.GLFW;
using NativeWindow = UhgeEngine.Core.PlatformDrivers.NativeWindow;

namespace UhgeEngine.Targets.Desktop;

public class DesktopWindowDriver : WindowDriver
{
    private readonly Glfw _glfw = Glfw.GetApi();
    private readonly VulkanRenderer _renderer = new VulkanRenderer();

    public override void Init()
    {
        _glfw.Init();

        unsafe
        {
            var glfwExtensions = _glfw.GetRequiredInstanceExtensions(out var glfwExtensionCount);
            _renderer.Init(glfwExtensionCount, glfwExtensions);
        }
    }

    public override NativeWindow SpawnWindow(int width, int height, string title)
    {
        unsafe
        {
            // Create window
            _glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);
            _glfw.WindowHint(WindowHintBool.Resizable, false);
            var glfwWindowPtr = (IntPtr) _glfw.CreateWindow(width, height, title, null, null);

            // Create surface
            VkNonDispatchableHandle surfaceHandle;
            if (_glfw.CreateWindowSurface(_renderer.VulkanInstance.ToHandle(), (WindowHandle*) glfwWindowPtr, null,
                    &surfaceHandle) != 0)
                throw new Exception("Failed to create surface for window.");

            // Create native window
            var vulkanWindow = _renderer.CreateWindow(surfaceHandle.ToSurface());
            return new GlfwNativeWindow(_glfw, glfwWindowPtr, vulkanWindow);
        }
    }

    protected override void Dispose(bool disposing)
    {
        _renderer.Dispose();
        _glfw.Terminate();
        _glfw.Dispose();
        base.Dispose(disposing);
    }
}

public class GlfwNativeWindow : NativeWindow
{
    private bool ShouldClose
    {
        get
        {
            unsafe
            {
                return _glfw.WindowShouldClose((WindowHandle*) _glfwWindowPtr);
            }
        }
    }

    private readonly Glfw _glfw;
    private readonly IntPtr _glfwWindowPtr;
    private readonly VulkanWindow _vulkanWindow;

    private Action<float>? _tick;

    public GlfwNativeWindow(Glfw glfw, IntPtr glfwWindowPtr, VulkanWindow vulkanWindow)
    {
        _glfw = glfw;
        _glfwWindowPtr = glfwWindowPtr;
        _vulkanWindow = vulkanWindow;
    }

    public override void SetTickFunction(Action<float> tick)
    {
        _tick = tick;
    }

    public override void Run()
    {
        _vulkanWindow.Init();
        
        while (!ShouldClose)
        {
            _glfw.PollEvents();
            _tick?.Invoke(0);
        }
    }

    protected override void Dispose(bool disposing)
    {
        unsafe
        {
            _vulkanWindow.Dispose();
            _glfw.DestroyWindow((WindowHandle*) _glfwWindowPtr);
        }

        base.Dispose(disposing);
    }
}