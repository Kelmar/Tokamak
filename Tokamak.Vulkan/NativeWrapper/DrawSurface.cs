using System;

using Silk.NET.Core;
using Silk.NET.Core.Contexts;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal unsafe class DrawSurface : IDisposable
    {
        private readonly VkPlatform m_platform;

        private readonly KhrSurface m_khrSurface;
        private readonly SurfaceKHR m_surface;

        public DrawSurface(VkPlatform platform, IVkSurface surface)
        {
            m_platform = platform;

            if (!m_platform.Vk.TryGetInstanceExtension<KhrSurface>(m_platform.Instance, out m_khrSurface))
                throw new NotSupportedException("KHR_surface extension not found.");

            m_surface = surface.Create<AllocationCallbacks>(m_platform.Instance.ToHandle(), null).ToSurface();
        }

        public void Dispose()
        {
            m_khrSurface.DestroySurface(m_platform.Instance, m_surface, null);
            m_khrSurface.Dispose();

            GC.SuppressFinalize(this);
        }

        public bool GetPhysicalDeviceSurfaceSupport(VkPhysicalDevice device, uint queueFamilyIndex)
        {
            m_khrSurface.GetPhysicalDeviceSurfaceSupport(device.Handle, queueFamilyIndex, m_surface, out Bool32 supported);
            return supported;
        }
    }
}
