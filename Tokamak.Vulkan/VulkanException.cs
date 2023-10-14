using System;

using Silk.NET.Vulkan;

namespace Tokamak.Vulkan
{
    internal class VulkanException : Exception
    {
        public VulkanException(Result result)
        {
            Result = result;
        }

        public Result Result { get; }
    }
}
