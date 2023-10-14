using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using Silk.NET.Core.Contexts;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;

using Tokamak.Buffer;
using Tokamak.Formats;
using Tokamak.Logging;
using Tokamak.Mathematics;
using Tokamak.Vulkan.NativeWrapper;

namespace Tokamak.Vulkan
{
    public unsafe class VkDevice : Device
    {
        private readonly ILogger m_log;
        private readonly Instance m_instance;

        public VkDevice(ILogger<VkDevice> log, IWindow window)
        {
            m_log = log;

            if (window.VkSurface == null)
            {
                m_log.Error("Vulkan not supported by platform");
                throw new Exception("Vulkan not supported by platform.");
            }

            Vk = Vk.GetApi();

            InitVK(out m_instance);

            Monitors = EnumerateMonitors().ToList();
        }

        public override void Dispose()
        {
            if (m_instance.Handle != IntPtr.Zero)
                Vk.DestroyInstance(m_instance, null);

            Vk.Dispose();

            base.Dispose();
        }

        public Vk Vk { get; }

        private void InitVK(out Instance instance)
        {
            using var s = m_log.BeginScope(new { Phase = "InitVK" });

            //var layers = VkLayerProperties.Enumerate(this).ToList();

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
                EnabledExtensionCount = 0,
                EnabledLayerCount = 0,
                PpEnabledExtensionNames = null,
                PpEnabledLayerNames = null
            };

            var result = Vk.CreateInstance(info, null, out instance);

            if (result != Result.Success)
                throw new VulkanException(result);
        }

        internal void SafeExecute(Func<Vk, Result> fn)
        {
            Result res = fn(Vk);

            if (res != Result.Success)
                throw new VulkanException(res);
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