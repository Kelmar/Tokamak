using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Silk.NET.Core.Native;
using Silk.NET.Maths;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Windowing;

using Tokamak.Logging.Abstractions;
using Tokamak.Config.Abstractions;

using Tokamak.Utilities;

using Tokamak.Mathematics;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Buffers;
using Tokamak.Tritium.Buffers.Formats;
using Tokamak.Tritium.Pipelines;

using Tokamak.Vulkan.NativeWrapper;

namespace Tokamak.Vulkan
{
    [LogName("Tokamak.Vulkan")]
    public unsafe class VkPlatform //: Platform
    {
        internal const string VK_VALIDATE_CALLS_CONFIG = "Vk.ValidateCalls";

        internal const string VK_VALIDATE_LAYER_NAME = "VK_LAYER_KHRONOS_validation";

        private readonly ILogger m_log;
        private readonly VulkanConfig m_config;

        private readonly List<VkDevice> m_devices = [];

        private readonly Func<VkDebug> m_debugFactory;

        private readonly VkDevice m_device = null;

        private readonly NVkCommandPool m_commandPool = null;

        private VkDebug m_debug = null;

        private NVkDrawSurface m_surface = null;

        public VkPlatform(
            ILogger<VkPlatform> logger,
            IWindow window,
            IOptions<VulkanConfig> config,
            Func<VkDebug> debugFactory)
            : base()
        {
            Window = window;

            m_log = logger;
            m_config = config.Value;
            m_debugFactory = debugFactory;

            if (Window.VkSurface == null)
                throw new PlatformNotSupportedException("Vulkan not supported by platform.");

            Vk = Vk.GetApi();

            InitVK();
        }

        public void Dispose()
        {
            Window.Resize -= ViewResize;

            m_commandPool.Dispose();

            // Release the physical/logical devices.
            foreach (var dev in m_devices)
                dev.Dispose();

            m_surface.Dispose();

            m_debug?.Dispose();

            if (Instance.Handle != IntPtr.Zero)
                Vk.DestroyInstance(Instance, null);

            Vk.Dispose();

            //base.Dispose();
        }

        internal Vk Vk { get; }

        internal Instance Instance { get; private set; }

        internal NVkDrawSurface Surface => m_surface;

        internal IWindow Window { get; }

        private VkDevice InitVK()
        {
            CreateInstance();

            m_debug?.Initialize();

            m_surface = new NVkDrawSurface(this, Window.VkSurface);

            EnumerateDevices();

            var device = SelectDevice();

            m_log.Info("Using device: {0}", device.Name);

            device.InitLogicalDevice();

            Window.Resize += ViewResize;

            return device;
        }

        private void ViewResize(Vector2D<int> obj)
        {
            foreach (var dev in m_devices)
                dev.SwapChain?.DeferredRebuild();
        }

        private void CreateInstance()
        {
            using var s = m_log.BeginScope(new { Phase = "InitVK" });

            var layers = VkLayerProperties.InstanceEnumerate(this).ToList();
            DumpInstanceLayers(layers);

            var extensions = NVkExtensionsProperties.InstanceEnumerate(this).ToList();
            DumpInstanceExtensions(extensions);

            var enableLayers = new List<string>();

            DebugUtilsMessengerCreateInfoEXT debugInfo;

            if (m_config.ValidateCalls)
            {
                if (!layers.Any(l => l.LayerName == VK_VALIDATE_LAYER_NAME))
                    m_log.Warn($"{VK_VALIDATE_CALLS_CONFIG} is set, but validation layer not installed for Vulkan");
                else
                {
                    enableLayers.Add(VK_VALIDATE_LAYER_NAME);

                    m_debug = m_debugFactory();
                    debugInfo = m_debug.GetInstanceStartup();
                }
            }

            var enableExts = new List<string>();
            enableExts.AddRange(GetRequiredExtensions());

            using var pEnableLayers = new VkStringArray(enableLayers);
            using var pEnableExts = new VkStringArray(enableExts);

            using var appName = new VkString("Test Application");
            using var engName = new VkString("Tokamak");

            var appInfo = new ApplicationInfo
            {
                SType = StructureType.ApplicationInfo,
                PNext = null,
                PApplicationName = appName.Pointer,
                PEngineName = engName.Pointer,
                ApplicationVersion = Vk.MakeVersion(0, 0, 1),
                EngineVersion = Vk.MakeVersion(0, 0, 1),
                ApiVersion = Vk.Version13
            };

            var info = new InstanceCreateInfo
            {
                SType = StructureType.InstanceCreateInfo,
                PNext = m_debug != null ? &debugInfo : null,
                Flags = InstanceCreateFlags.None,
                PApplicationInfo = &appInfo,
                EnabledExtensionCount = pEnableExts.Length,
                EnabledLayerCount = pEnableLayers.Length,
                PpEnabledExtensionNames = pEnableExts.Pointer,
                PpEnabledLayerNames = pEnableLayers.Pointer
            };

            Instance instance;

            var result = Vk.CreateInstance(ref info, null, out instance);

            if (result != Result.Success)
                throw new VulkanException(result);

            Instance = instance;
        }

        private IEnumerable<string> GetRequiredExtensions()
        {
            byte** items = Window.VkSurface.GetRequiredExtensions(out uint cnt);

            var rval = new List<string>(SilkMarshal.PtrToStringArray((nint)items, (int)cnt));

            m_log.Debug("Surface requires extensions: {0}", String.Join(", ", rval));

            if (m_debug != null)
                rval.Add(ExtDebugUtils.ExtensionName);

            return rval;
        }

        private void DumpInstanceLayers(List<VkLayerProperties> layers)
        {
            if (!m_log.LevelEnabled(LogLevel.Debug))
                return;

            var sb = new StringBuilder();

            foreach (var layer in layers)
            {
                sb.AppendLine();
                sb.AppendFormat("    LAYER {0} {1:X8}: {2}", layer.LayerName, layer.SpecVersion, layer.Description);
            }

            m_log.Debug("Detected Vulkan instance layers:{0}", sb);
        }

        private void DumpInstanceExtensions(List<NVkExtensionsProperties> extensions)
        {
            if (!m_log.LevelEnabled(LogLevel.Debug))
                return;

            var sb = new StringBuilder();

            foreach (var ext in extensions)
            {
                sb.AppendLine();
                sb.AppendFormat("    EXT {0} {1:X8}", ext.ExtensionName, ext.SpecVersion);
            }

            m_log.Debug("Detected Vulkan instance extensions:{0}", sb);
        }

        internal void SafeExecute(Func<Vk, Result> fn)
        {
            Result res = fn(Vk);

            if (res != Result.Success)
                throw new VulkanException(res);
        }

        private void EnumerateDevices()
        {
            m_devices.AddRange(VkDevice.EnumerateAll(this));

            if (!m_devices.Any())
            {
                m_log.Fatal("No physical Vulkan devices detected!");
                throw new PlatformNotSupportedException("No physical Vulkan devices detected!");
            }
        }

        private VkDevice SelectDevice()
        {
            var candidates =
            (
                from dev in m_devices
                where dev.GetQueues().Any(q => q.QueueFlags.HasFlag(QueueFlags.GraphicsBit))
                select dev
            ).ToList();

            if (!candidates.Any())
            {
                m_log.Fatal("No graphical Vulkan devices detected!");
                throw new PlatformNotSupportedException("No graphical Vulkan devices detected.");
            }

            VkDevice rval = candidates.First();

            // TODO: In the future we will want to allow the user to be able to select which device they want.
            // We should probably also look for the more capable device (more memory, best selection of features, etc)

            return rval;
        }

        protected IFactory<IPipeline> GetPipelineFactory(PipelineConfig config)
        {
            return new PipelineFactory(m_device, config);
        }

        public ICommandList GetCommandList()
        {
            return new CommandList(null, m_device, m_commandPool);
        }

        public IVertexBuffer<T> GetVertexBuffer<T>(BufferUsage usage)
           where T : unmanaged
        {
            return null;
        }

        public IElementBuffer GetElementBuffer(BufferUsage usage)
        {
            return null;
        }

        public ITextureObject GetTextureObject(PixelFormat format, Point size)
        {
            return new TextureObject(this, format, size);
        }
    }
}