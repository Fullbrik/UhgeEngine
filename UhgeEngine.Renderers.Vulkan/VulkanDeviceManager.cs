using System.Runtime.InteropServices;
using Silk.NET.Vulkan.Extensions.KHR;

namespace UhgeEngine.Renderers.Vulkan;

struct QueueFamilyIndices
{
    public uint? GraphicsFamily;
    public uint? PresentFamily;

    public bool IsComplete => GraphicsFamily.HasValue && PresentFamily.HasValue;
}

public class VulkanDeviceManager : IDisposable
{
    private readonly Vk _vk;
    private readonly Instance _instance;
    private readonly KhrSurface _surfaceApi;
    private readonly SurfaceKHR _surface;

    private PhysicalDevice _physicalDevice;
    private Device _device;
    private Queue _graphicsQueue;
    private Queue _presentQueue;

    public VulkanDeviceManager(Vk vk, Instance instance, KhrSurface surfaceApi, SurfaceKHR surface)
    {
        _vk = vk;
        _instance = instance;
        _surfaceApi = surfaceApi;
        _surface = surface;
    }

    public void Init()
    {
        SelectPhysicalDevice();
        CreateLogicalDevice();
    }

    private void SelectPhysicalDevice()
    {
        var deviceAndProperties = _vk.GetPhysicalDevices(_instance)
            .Select((pd) => // Get device properties
            {
                _vk.GetPhysicalDeviceProperties(pd, out var properties);
                return (pd, properties);
            })
            .Where(tuple => IsDeviceUsable(tuple.pd, tuple.properties))
            .OrderBy((tuple =>
                tuple.properties.DeviceType == PhysicalDeviceType.DiscreteGpu ? 0 : 1)) // Prioritize a discrete gpu
            .First();
        _physicalDevice = deviceAndProperties.pd;

        unsafe
        {
            // Print out the device we selected (Useful for debugging performance)
            var deviceName = Marshal.PtrToStringAnsi((IntPtr) deviceAndProperties.properties.DeviceName);
            Console.WriteLine("Selected device: {0}", deviceName);
        }
    }

    private bool IsDeviceUsable(PhysicalDevice pd, PhysicalDeviceProperties properties) =>
        FindQueueFamilies(pd).IsComplete;

    private unsafe void CreateLogicalDevice()
    {
        QueueFamilyIndices indices = FindQueueFamilies(_physicalDevice);

        var queuePriority = 1f;

        var queueCreateInfos = new[]
        {
            new DeviceQueueCreateInfo(StructureType.DeviceQueueCreateInfo)
            {
                QueueFamilyIndex = indices.GraphicsFamily!.Value,
                QueueCount = 1,
                PQueuePriorities = &queuePriority
            },
            new DeviceQueueCreateInfo(StructureType.DeviceQueueCreateInfo)
            {
                QueueFamilyIndex = indices.PresentFamily!.Value,
                QueueCount = 1,
                PQueuePriorities = &queuePriority
            }
        };

        PhysicalDeviceFeatures deviceFeatures = new();

        fixed (DeviceQueueCreateInfo* queueCreateInfosPtr = queueCreateInfos)
        {
            DeviceCreateInfo createInfo = new(StructureType.DeviceCreateInfo)
            {
                PQueueCreateInfos = queueCreateInfosPtr,
                QueueCreateInfoCount = (uint) queueCreateInfos.Length,
                PEnabledFeatures = &deviceFeatures
            };
            
            VKAssert(_vk.CreateDevice(_physicalDevice, &createInfo, null, out _device), nameof(_vk.CreateDevice));
        }
        
        GetQueues(indices);
    }

    private unsafe QueueFamilyIndices FindQueueFamilies(PhysicalDevice physicalDevice)
    {
        QueueFamilyIndices indices = new QueueFamilyIndices();

        uint queueFamilyCount = 0;
        _vk.GetPhysicalDeviceQueueFamilyProperties(physicalDevice, &queueFamilyCount, null);

        var queueFamilies = new QueueFamilyProperties[queueFamilyCount];
        _vk.GetPhysicalDeviceQueueFamilyProperties(physicalDevice, &queueFamilyCount, queueFamilies);

        indices.GraphicsFamily = (uint) queueFamilies
            .Select((properties, i) => (properties, i))
            .Where(tuple => (tuple.properties.QueueFlags & QueueFlags.QueueGraphicsBit) != 0)
            .Select(tuple => tuple.i)
            .FirstOrDefault();

        indices.PresentFamily = (uint) queueFamilies
            .Select((properties, i) => (properties, i))
            .Where(tuple =>
            {
                _surfaceApi.GetPhysicalDeviceSurfaceSupport(_physicalDevice, (uint) tuple.i, _surface,
                    out var presentSupport);
                return presentSupport;
            })
            .Select(tuple => tuple.i)
            .FirstOrDefault();

        return indices;
    }

    private void GetQueues(QueueFamilyIndices indices)
    {
        _vk.GetDeviceQueue(_device, indices.GraphicsFamily!.Value, 0, out _graphicsQueue);
        _vk.GetDeviceQueue(_device, indices.PresentFamily!.Value, 0, out _presentQueue);
    }

    private void ReleaseUnmanagedResources()
    {
        unsafe
        {
            _vk.DestroyDevice(_device, null);
        }
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~VulkanDeviceManager()
    {
        ReleaseUnmanagedResources();
    }
}