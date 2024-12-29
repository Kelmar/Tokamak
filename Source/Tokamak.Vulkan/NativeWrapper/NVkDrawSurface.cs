using System;
using System.Collections.Generic;

using Silk.NET.Core;
using Silk.NET.Core.Contexts;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal unsafe class NVkDrawSurface : IDisposable
    {
        private readonly VkPlatform m_platform;

        private readonly KhrSurface m_khrSurface;
        private readonly SurfaceKHR m_surface;

        public NVkDrawSurface(VkPlatform platform, IVkSurface surface)
        {
            m_platform = platform;

            if (!m_platform.Vk.TryGetInstanceExtension(m_platform.Instance, out m_khrSurface))
                throw new NotSupportedException("KHR_surface extension not found.");

            m_surface = surface.Create<AllocationCallbacks>(m_platform.Instance.ToHandle(), null).ToSurface();
        }

        public void Dispose()
        {
            m_khrSurface.DestroySurface(m_platform.Instance, m_surface, null);
            m_khrSurface.Dispose();

            GC.SuppressFinalize(this);
        }

        public SurfaceKHR SurfaceKHR => m_surface;

        public SurfaceCapabilitiesKHR GetPhysicalDeviceCapabilities(VkPhysicalDevice device)
        {
            m_khrSurface.GetPhysicalDeviceSurfaceCapabilities(device.Handle, m_surface, out SurfaceCapabilitiesKHR caps);
            return caps;
        }

        public IEnumerable<PresentModeKHR> GetPhysicalDevicePresentModes(VkPhysicalDevice device)
        {
            uint modeCount = 0;

            m_khrSurface.GetPhysicalDeviceSurfacePresentModes(device.Handle, m_surface, ref modeCount, null);

            var rval = new List<PresentModeKHR>((int)modeCount);

            if (modeCount > 0)
            {
                var modes = new PresentModeKHR[modeCount];

                fixed (PresentModeKHR* modePtr = modes)
                    m_khrSurface.GetPhysicalDeviceSurfacePresentModes(device.Handle, m_surface, ref modeCount, modePtr);

                rval.AddRange(modes);
            }

            return rval;
        }

        public IEnumerable<SurfaceFormatKHR> GetPhysicalDeviceFormats(VkPhysicalDevice device)
        {
            uint fmtCount = 0;

            m_khrSurface.GetPhysicalDeviceSurfaceFormats(device.Handle, m_surface, ref fmtCount, null);

            var rval = new List<SurfaceFormatKHR>((int)fmtCount);

            if (fmtCount > 0)
            {
                var fmts = new SurfaceFormatKHR[fmtCount];

                fixed (SurfaceFormatKHR* fmtPtr = fmts)
                    m_khrSurface.GetPhysicalDeviceSurfaceFormats(device.Handle, m_surface, ref fmtCount, fmtPtr);

                rval.AddRange(fmts);
            }

            return rval;
        }

        public bool GetPhysicalDeviceSupport(VkPhysicalDevice device, uint queueFamilyIndex)
        {
            m_khrSurface.GetPhysicalDeviceSurfaceSupport(device.Handle, queueFamilyIndex, m_surface, out Bool32 supported);
            return supported;
        }
    }
}
