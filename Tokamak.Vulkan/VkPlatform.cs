using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Silk.NET.Core.Contexts;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;

using Tokamak.Buffer;
using Tokamak.Config;
using Tokamak.Formats;
using Tokamak.Logging;
using Tokamak.Mathematics;
using Tokamak.Vulkan.NativeWrapper;

namespace Tokamak.Vulkan
{
    public unsafe class VkPlatform : Platform
    {
        internal const string VK_VALIDATE_CALLS_CONFIG = "Vk.ValidateCalls";
        //internal const string VK_DEBUG_CONFIG = "Vk.DebugCalls";

        internal const string VK_VALIDATE_LAYER_NAME = "VK_LAYER_KHRONOS_validation";
        //internal const string KV_DEBUG_LAYER_NAME = "";

        private readonly ILogger m_log;
        private readonly IConfigReader m_config;

        private readonly List<VkDevice> m_devices = new List<VkDevice>();

        private DrawSurface m_surface = null;

        public VkPlatform(IWindow window)
        {
            Window = window;

            m_log = Platform.Services.GetLogger<VkPlatform>();
            m_config = Platform.Services.Find<IConfigReader>();

            if (Window.VkSurface == null)
            {
                m_log.Error("Vulkan not supported by platform");
                throw new Exception("Vulkan not supported by platform.");
            }

            // We're needed for several things during startup.
            Platform.Services.Register(this);

            Vk = Vk.GetApi();

            InitVK();

            Monitors = EnumerateMonitors().ToList();
        }

        public override void Dispose()
        {
            // Release the physical/logical devices.
            foreach (var dev in m_devices)
                dev.Dispose();

            m_surface.Dispose();

            if (Instance.Handle != IntPtr.Zero)
                Vk.DestroyInstance(Instance, null);

            Vk.Dispose();

            Platform.Services.Unregister(this);

            base.Dispose();
        }

        internal Vk Vk { get; }

        internal Instance Instance { get; private set; }

        internal DrawSurface Surface => m_surface;

        internal IWindow Window { get; }

        private void InitVK()
        {
            CreateInstance(Window.VkSurface);

            m_surface = new DrawSurface(this, Window.VkSurface);

            EnumerateDevices();

            var device = SelectDevice();

            m_log.Info("Using device: {0}", device.Name);

            device.InitLogicalDevice();
        }

        private void CreateInstance(IVkSurface surface)
        {
            using var s = m_log.BeginScope(new { Phase = "InitVK" });

            var layers = VkLayerProperties.InstanceEnumerate(this).ToList();
            DumpInstanceLayers(layers);

            var extensions = VkExtensionsProperties.InstanceEnumerate(this).ToList();
            DumpInstanceExtensions(extensions);

            var enableLayers = new List<string>();

            if (m_config.Get(VK_VALIDATE_CALLS_CONFIG, false))
            {
                if (!layers.Any(l => l.LayerName == VK_VALIDATE_LAYER_NAME))
                    m_log.Warn($"{VK_VALIDATE_CALLS_CONFIG} is set, but validation layer not installed for Vulkan");
                else
                    enableLayers.Add(VK_VALIDATE_LAYER_NAME);
            }

            /*
            if (m_config.Get(VK_DEBUG_CONFIG, false))
            {
                if (!layers.Any(l => l.LayerName == KV_DEBUG_LAYER_NAME))
                    m_log.Warn($"{VK_DEBUG_CONFIG} is set, but validation layer not installed for Vulkan");
                else
                    enableLayers.Add(KV_DEBUG_LAYER_NAME);
            }
            */

            var enableExts = new List<string>();
            enableExts.AddRange(GetRequiredExtensions(surface));

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
                PNext = null,
                Flags = InstanceCreateFlags.None,
                PApplicationInfo = &appInfo,
                EnabledExtensionCount = pEnableExts.Length,
                EnabledLayerCount = pEnableLayers.Length,
                PpEnabledExtensionNames = pEnableExts.Pointer,
                PpEnabledLayerNames = pEnableLayers.Pointer
            };

            Instance instance;

            var result = Vk.CreateInstance(info, null, out instance);

            if (result != Result.Success)
                throw new VulkanException(result);

            Instance = instance;
        }

        private IEnumerable<string> GetRequiredExtensions(IVkSurface surface)
        {
            byte** items = surface.GetRequiredExtensions(out uint cnt);

            var rval = new List<string>(SilkMarshal.PtrToStringArray((nint)items, (int)cnt));

            m_log.Debug("Surface requires extensions: {0}", String.Join(", ", rval));

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

        private void DumpInstanceExtensions(List<VkExtensionsProperties> extensions)
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
                throw new Exception("No physical Vulkan devices detected!");
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
                throw new Exception("No graphical Vulkan devices detected.");
            }

            VkDevice rval = candidates.First();

            // TODO: In the future we will want to allow the user to be able to select which device they want.
            // We should probably also look for the more capable device (more memory, best selection of features, etc)

            return rval;
        }

        private IEnumerable<Monitor> EnumerateMonitors()
        {
            yield return new Monitor
            {
                Index = 0,
                IsMain = true,
                Gamma = 2.2f,
                DPI = new Point(192, 192),
                RawDPI = new System.Numerics.Vector2(192, 192),
                WorkArea = new Rect(0, 0, 3840, 2160)
            };
        }

        public override void ClearBoundTexture()
        {
        }

        public override void ClearBuffers(GlobalBuffer buffers)
        {
        }

        public override void DrawArrays(PrimitiveType primitive, int vertexOffset, int vertexCount)
        {
        }

        public override void DrawElements(PrimitiveType primitive, int length)
        {
        }

        public override IElementBuffer GetElementBuffer(BufferType type)
        {
            return null;
        }

        public override IShaderFactory GetShaderFactory()
        {
            return new ShaderFactory(this);
        }

        public override ITextureObject GetTextureObject(PixelFormat format, Point size)
        {
            return new TextureObject(this, format, size);
        }

        public override IVertexBuffer<T> GetVertexBuffer<T>(BufferType type)
        {
            return null;
        }

        public override void SetRenderState(RenderState state)
        {
        }
    }
}