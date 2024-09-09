using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using Silk.NET.Windowing;

using Stashbox;

using Tokamak.Core.Logging;

using Tokamak.Vulkan.NativeWrapper;

namespace Tokamak.Vulkan
{
    internal unsafe class SwapChain : IDisposable
    {
        private readonly ILogger m_log;

        private readonly VkDevice m_device;

        private readonly List<SwapChainImage> m_images = new List<SwapChainImage>();

        // Don't like having a render pass here, but it's needed for the framebuffers. -- B.Simonds (Nov 16, 2023)
        private VkRenderPass m_renderPass;
        private bool m_needsRebuild = false;

        private bool m_disposed = false;

        private KhrSwapchain m_khrSwapChain;
        private SwapchainKHR m_handle;

        private uint m_imageIndex;

        private SurfaceCapabilitiesKHR m_surfaceCaps;
        private SurfaceFormatKHR m_surfaceFormat;
        private PresentModeKHR m_presentMode;

        public SwapChain(ILogger<SwapChain> logger, VkDevice device)
        {
            m_device = device;
            m_log = logger;
            //m_log = m_device.Parent.Resolver.Resolve<ILogger<SwapChain>>();

            InitializeFormat();

            m_renderPass = new VkRenderPass(m_device, Format);

            CreateSwapChain();
        }

        public void Dispose()
        {
            m_disposed = true;

            Cleanup();

            m_renderPass.Dispose();
        }

        public IReadOnlyList<SwapChainImage> Images => m_images;

        public SwapChainImage CurrentImage => Images[(int)m_imageIndex];

        public uint ImageIndex => m_imageIndex;

        public Format Format { get; private set; }

        public Extent2D Extent { get; private set; }

        private void Cleanup()
        {
            m_device.WaitIdle();

            DisposeImageChain();

            m_khrSwapChain.DestroySwapchain(m_device.LogicalDevice, m_handle, null);
        }

        private void DisposeImageChain()
        {
            foreach (var img in m_images)
                img.Dispose();

            m_images.Clear();
        }

        private void InitializeFormat()
        {
            m_surfaceCaps = m_device.Parent.Surface.GetPhysicalDeviceCapabilities(m_device.PhysicalDevice);

            m_surfaceFormat = ChooseFormat(m_device.Parent.Surface.GetPhysicalDeviceFormats(m_device.PhysicalDevice));
            m_presentMode = ChoosePresentMode(m_device.Parent.Surface.GetPhysicalDevicePresentModes(m_device.PhysicalDevice));

            Format = m_surfaceFormat.Format;
        }

        private void CreateSwapChain()
        {
            InitializeFormat();

            Extent = ChooseExtent(m_surfaceCaps, m_device.Parent.Window);

            m_log.Debug("Building swap chain for {0}x{1}", Extent.Width, Extent.Height);

            uint imageCnt = m_surfaceCaps.MinImageCount + 1;

            if (m_surfaceCaps.MaxImageCount > 0 && imageCnt > m_surfaceCaps.MaxImageCount)
                imageCnt = m_surfaceCaps.MaxImageCount;

            var createInfo = new SwapchainCreateInfoKHR
            {
                SType = StructureType.SwapchainCreateInfoKhr,
                Surface = m_device.Parent.Surface.SurfaceKHR,
                MinImageCount = imageCnt,
                ImageFormat = Format,
                ImageColorSpace = m_surfaceFormat.ColorSpace,
                ImageExtent = Extent,
                ImageArrayLayers = 1,
                ImageUsage = ImageUsageFlags.ColorAttachmentBit,
                ImageSharingMode = SharingMode.Exclusive,
                PreTransform = m_surfaceCaps.CurrentTransform,
                CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
                PresentMode = m_presentMode,
                Clipped = true,
                OldSwapchain = default
            };

            uint* indices = stackalloc[] { m_device.GraphicsQueueIndex, m_device.PresentQueueIndex };

            if (m_device.GraphicsQueueIndex != m_device.PresentQueueIndex)
            {
                createInfo.ImageSharingMode = SharingMode.Concurrent;
                createInfo.QueueFamilyIndexCount = 2;
                createInfo.PQueueFamilyIndices = indices;
            }

            if (!m_device.TryGetExtension(out m_khrSwapChain))
                throw new NotSupportedException("VK_KHR_swapchain extension not found.");

            SafeExecute(sc => sc.CreateSwapchain(m_device.LogicalDevice, ref createInfo, null, out m_handle));

            BuildImageChain();

            m_needsRebuild = false;

            m_log.Trace("Swap chain built for {0}x{1}", Extent.Width, Extent.Height);
        }

        private void BuildImageChain()
        {
            Debug.Assert(!m_images.Any(), "ImageChain not empty!");

            List<VkImage> images = GetImages().ToList();
            m_images.Capacity = Math.Max(m_images.Capacity, images.Count);

            for (int i = 0; i < images.Count; ++i)
            {
                VkImageView view = images[i].CreateView();

                m_images.Add(new SwapChainImage(i)
                {
                    Image = images[i],
                    View = view,
                    Framebuffer = new VkFramebuffer(m_device, Extent, m_renderPass, view)
                });
            }
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

            SafeExecute(sc => sc.GetSwapchainImages(m_device.LogicalDevice, m_handle, cnt, null));

            var rval = new List<VkImage>((int)imageCnt);

            if (imageCnt > 0)
            {
                var images = new Image[imageCnt];

                SafeExecute(sc => sc.GetSwapchainImages(m_device.LogicalDevice, m_handle, cnt, images));

                rval.AddRange(images.Select(i => VkImage.FromHandle(m_device, i, Format, ext3D)));
            }

            return rval;
        }

        public bool AcquireNextImage(VkFence fence)
        {
            Debug.Assert(fence != default, "Invalid fence handle");

            Result res = m_khrSwapChain.AcquireNextImage(m_device.LogicalDevice, m_handle, ulong.MaxValue, default, fence.Handle, ref m_imageIndex);

            switch (res)
            {
            case Result.Success:
                break;

            case Result.ErrorOutOfDateKhr:
            case Result.SuboptimalKhr:
                Rebuild();
                return false;

            default:
                throw new VulkanException(res);
            }

            return true;
        }

        public void Present(Queue presentQueue)
        {
            var swapChains = stackalloc[] { m_handle };

            uint imageIndex = m_imageIndex;

            var presentInfo = new PresentInfoKHR
            {
                SType = StructureType.PresentInfoKhr,
                WaitSemaphoreCount = 0,
                PWaitSemaphores = null,

                SwapchainCount = 1,
                PSwapchains = swapChains,

                PImageIndices = &imageIndex
            };

            var res = m_khrSwapChain.QueuePresent(presentQueue, in presentInfo);

            if (res == Result.SuboptimalKhr || res == Result.ErrorOutOfDateKhr || m_needsRebuild)
                Rebuild();
            else if (res != Result.Success)
                throw new VulkanException(res);
        }

        public void DeferredRebuild()
        {
            m_needsRebuild = true;
        }

        internal void Rebuild()
        {
            m_log.Debug("SwapChain.Rebuild() called.");

            m_device.WaitIdle();

            Cleanup();

            CreateSwapChain();
        }
    }
}
