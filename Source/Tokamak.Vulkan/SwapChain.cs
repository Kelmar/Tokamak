using System;
using System.Collections.Generic;
using System.Linq;

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

        public SwapChain(VkDevice device)
        {
            Images = new List<VkImage>(0);

            m_device = device;

            CreateSwapChain();
        }

        public void Dispose()
        {
            m_disposed = true;
            Cleanup();
        }

        public IReadOnlyList<VkImage> Images { get; private set; }

        public IReadOnlyList<VkImageView> Views { get; private set; }

        public Format Format { get; private set; }

        public Extent2D Extent { get; private set; }

        private void Cleanup()
        {
            m_device.WaitIdle();

            foreach (var img in Images)
                img.Dispose();

            Images = new List<VkImage>(0);

            m_khrSwapChain.DestroySwapchain(m_device.LogicalDevice, m_swapChain, null);
        }

        private void CreateSwapChain()
        {
            var platform = m_device.Parent;

            SurfaceCapabilitiesKHR caps = platform.Surface.GetPhysicalDeviceCapabilities(m_device.PhysicalDevice);

            SurfaceFormatKHR format = ChooseFormat(platform.Surface.GetPhysicalDeviceFormats(m_device.PhysicalDevice));
            PresentModeKHR presentMode = ChoosePresentMode(platform.Surface.GetPhysicalDevicePresentModes(m_device.PhysicalDevice));
            Extent = ChooseExtent(caps, platform.Window);

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
                ImageExtent = Extent,
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

            Format = createInfo.ImageFormat;
            Extent = createInfo.ImageExtent;

            foreach (var img in Images)
                img.Dispose();

            Images = GetImages().ToList();
            Views = Images.Select(i => i.CreateView()).ToList();
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

            return new Extent2D
            {
                Width = (uint)Math.Clamp(window.FramebufferSize.X, caps.MinImageExtent.Width, caps.MaxImageExtent.Width),
                Height = (uint)Math.Clamp(window.FramebufferSize.Y, caps.MinImageExtent.Height, caps.MaxImageExtent.Height)
            };
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

        private IEnumerable<VkImage> GetImages()
        {
            var ext3D = new Extent3D(Extent.Width, Extent.Height, 1);

            uint imageCnt = 0;
            uint* cnt = &imageCnt;

            SafeExecute(sc => sc.GetSwapchainImages(m_device.LogicalDevice, m_swapChain, cnt, null));

            var rval = new List<VkImage>((int)imageCnt);

            if (imageCnt > 0)
            {
                var images = new Image[imageCnt];

                SafeExecute(sc => sc.GetSwapchainImages(m_device.LogicalDevice, m_swapChain, cnt, images));
                

                rval.AddRange(images.Select(i => VkImage.FromHandle(m_device, i, Format, ext3D)));
            }

            return rval;
        }

        internal void Rebuild()
        {
            Cleanup();
            CreateSwapChain();
        }
    }
}
