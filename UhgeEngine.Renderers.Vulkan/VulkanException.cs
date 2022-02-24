namespace UhgeEngine.Renderers.Vulkan;

public class VulkanException : Exception
{
    public static void VKAssert(Result result, string? function = null, params Result[] allowedResults)
    {
        if (result == Result.Success) return;
        if (allowedResults.Length <= 0 || allowedResults.Contains(result)) return;
        
        if (function != null) throw new VulkanException(function, result);
        else throw new VulkanException(result);
    }
    
    public VulkanException(Result result) : base($"Vulkan failed with result {result}")
    {
    }

    public VulkanException(string function, Result result) : base($"Vulkan failed {function} with result {result}")
    {
    }
}