using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using Silk.NET.Windowing;

using Tokamak.Vulkan.NativeWrapper;

namespace Tokamak.Vulkan
{
    internal unsafe class SwapChain : IDisposable
    {
        private readonly VkDevice m_device;

        private bool m_disposed = false;

        private KhrSwapchain m_khrSwapChain;
        private SwapchainKHR m_swapChain;
        private Image[] m_images;
        private Format m_format;
        private Extent2D m_extent;

        public SwapChain(VkDevice device)
        {
            m_device = device;

            CreateSwapChain();
        }

        public void Dispose()
        {
            m_disposed = true;
            m_khrSwapChain.DestroySwapchain(m_device.LogicalDevice, m_swapChain, null);
        }

        private void CreateSwapChain()
        {
            var platform = Platform.Services.Find<VkPlatform>();

            SurfaceCapabilitiesKHR caps = platform.Surface.GetPhysicalDeviceCapabilities(m_device.PhysicalDevice);

            SurfaceFormatKHR format = ChooseFormat(platform.Surface.GetPhysicalDeviceFormats(m_device.PhysicalDevice));
            PresentModeKHR presentMode = ChoosePresentMode(platform.Surface.GetPhysicalDevicePresentModes(m_device.PhysicalDevice));
            Extent2D extent = ChooseExtent(caps, platform.Window);

            uint imageCnt = caps.MinImageCount + 1;

            if (caps.MaxImageCount > 0 && imageCnt > caps.MaxImageCount)
                imageCnt = caps.MaxImageCount;

            var createInfo = new SwapchainCreateInfoKHR
            {
                SType = StructureType.SwapchainCreateInfoKhr,
                Surface = platform.Surface.SurfaceKHR,
                MinImageCount = imageCnt,
                ImageFormat = format.Format,
                ImageColorSpace = format.ColorSpace,
                ImageExtent = extent,
                ImageArrayLayers = 1,
                ImageUsage = ImageUsageFlags.ColorAttachmentBit,
                ImageSharingMode = SharingMode.Exclusive,
                PreTransform = caps.CurrentTransform,
                CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
                PresentMode = presentMode,
                Clipped = true,
                OldSwapchain = default
            };

            uint* indices = stackalloc[] { m_device.GraphicsQueueIndex, m_device.SurfaceQueueIndex };

            if (m_device.GraphicsQueueIndex != m_device.SurfaceQueueIndex)
            {
                createInfo.ImageSharingMode = SharingMode.Concurrent;
                createInfo.QueueFamilyIndexCount = 2;
                createInfo.PQueueFamilyIndices = indices;
            }

            if (!m_device.TryGetExtension(out m_khrSwapChain))
                throw new NotSupportedException("VK_KHR_swapchain extension not found.");

            SafeExecute(sc => sc.CreateSwapchain(m_device.LogicalDevice, createInfo, null, out m_swapChain));

            m_format = createInfo.ImageFormat;
            m_extent = createInfo.ImageExtent;
        }

        private void SafeExecute(Func<KhrSwapchain, Result> cb)
        {
            if (m_disposed)
                throw new ObjectDisposedException(nameof(SwapChain));

            Result res = cb(m_khrSwapChain);

            if (res != Result.Success)
                throw new VulkanException(res);
        }

        private Extent2D ChooseExtent(SurfaceCapabilitiesKHR caps, IWindow window)
        {
            if (caps.CurrentExtent.Width != uint.MaxValue)
                return caps.CurrentExtent;

            Extent2D rval = new Extent2D();

            rval.Width = (uint)Math.Clamp(window.FramebufferSize.X, caps.MinImageExtent.Width, caps.MaxImageExtent.Width);
            rval.Height = (uint)Math.Clamp(window.FramebufferSize.Y, caps.MinImageExtent.Height, caps.MaxImageExtent.Height);

            return rval;
        }

        private SurfaceFormatKHR ChooseFormat(IEnumerable<SurfaceFormatKHR> formats)
        {
            // TODO: I presume this is where we would select one of the higher than 8-bits/channel formats for HDR if we want to.

            foreach (var format in formats)
            {
                if (format.Format == Format.B8G8R8A8Srgb && format.ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr)
                    return format;
            }

            return formats.First();
        }

        private PresentModeKHR ChoosePresentMode(IEnumerable<PresentModeKHR> presentModes)
        {
            foreach (var mode in presentModes)
            {
                if (mode == PresentModeKHR.MailboxKhr)
                    return mode;
            }

            return PresentModeKHR.FifoKhr;
        }
    }
}
