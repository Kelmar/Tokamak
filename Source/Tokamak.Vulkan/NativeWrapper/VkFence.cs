using System;
using System.Linq;

using Silk.NET.Vulkan;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal unsafe sealed class VkFence : IDisposable
    {
        private readonly VkDevice m_device;

        public VkFence(VkDevice device, bool signaled)
        {
            m_device = device;
            Handle = CreateHandle(signaled);
        }

        public void Dispose()
        {
            m_device.Parent.Vk.DestroyFence(m_device.LogicalDevice, Handle, null);
        }

        public Fence Handle { get; }

        public void Reset()
        {
            var handles = new Fence[] { Handle };
            m_device.Parent.SafeExecute(vk => vk.ResetFences(m_device.LogicalDevice, handles));
        }

        /// <summary>
        /// Waits for the fence.
        /// </summary>
        /// <param name="timeout">A timeout value to wait for</param>
        /// <returns>True if the fence has been signaled, false if the timeout was reached.</returns>
        public bool Wait(ulong timeout = ulong.MaxValue)
        {
            var handles = new Fence[] { Handle };
            Result result = m_device.Parent.Vk.WaitForFences(m_device.LogicalDevice, handles, true, timeout);

            switch (result)
            {
            case Result.Success:
                return true;

            case Result.Timeout:
                return false;

            default:
                throw new VulkanException(result);
            }
        }

        /// <summary>
        /// Waits for a list of fences.
        /// </summary>
        /// <param name="timeout">A timeout value to wait for</param>
        /// <returns>True if all of the fences have been signaled, false if the timeout was reached.</returns>
        public static bool WaitAll(ulong timeout = ulong.MaxValue, params VkFence[] fences)
        {
            if (fences == null || !fences.Any())
                return true;

            // Probably not the best way to handle a list of fences that cross device boundaries.

            var first = fences.First();

            var handles = fences.Select(f => f.Handle).ToArray();
            Result result = first.m_device.Parent.Vk.WaitForFences(first.m_device.LogicalDevice, handles, true, timeout);

            switch (result)
            {
            case Result.Success:
                return true;

            case Result.Timeout:
                return false;

            default:
                throw new VulkanException(result);
            }
        }

        /// <summary>
        /// Waits for any one in a list of fences.
        /// </summary>
        /// <param name="timeout">A timeout value to wait for</param>
        /// <returns>True if any of the fences have been signaled, false if the timeout was reached.</returns>
        public static bool WaitAny(ulong timeout = ulong.MaxValue, params VkFence[] fences)
        {
            if (fences == null || !fences.Any())
                return true;

            // Probably not the best way to handle a list of fences that cross device boundaries.

            var first = fences.First();

            var handles = fences.Select(f => f.Handle).ToArray();
            Result result = first.m_device.Parent.Vk.WaitForFences(first.m_device.LogicalDevice, handles, false, timeout);

            switch (result)
            {
            case Result.Success:
                return true;

            case Result.Timeout:
                return false;

            default:
                throw new VulkanException(result);
            }
        }

        private Fence CreateHandle(bool signaled)
        {
            var createInfo = new FenceCreateInfo
            {
                SType = StructureType.FenceCreateInfo,
                Flags = signaled ? FenceCreateFlags.SignaledBit : FenceCreateFlags.None
            };

            Fence handle = default;

            m_device.Parent.SafeExecute(vk => vk.CreateFence(m_device.LogicalDevice, createInfo, null, out handle));

            return handle;
        }
    }
}
