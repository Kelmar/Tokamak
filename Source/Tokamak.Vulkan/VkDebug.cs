using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;

using Stashbox;

using Tokamak.Core.Config;
using Tokamak.Core.Logging;

namespace Tokamak.Vulkan
{
    internal unsafe class VkDebug : IDisposable
    {
        private readonly VkPlatform m_platform;
        private readonly ILogger m_log;

        private ExtDebugUtils m_debugUtils;
        private DebugUtilsMessengerEXT m_messenger;

        public VkDebug(VkPlatform platform)
        {
            m_platform = platform;

            var logFact = platform.Resolver.Resolve<ILogFactory>();
            m_log = logFact.GetLogger("Vulkan");

            var conf = platform.Resolver.Resolve<IConfigReader>();

            ShouldLoad = conf.Get(VkPlatform.VK_VALIDATE_CALLS_CONFIG, false);
        }

        public void Dispose()
        {
            if (m_debugUtils != null)
            {
                m_debugUtils.DestroyDebugUtilsMessenger(m_platform.Instance, m_messenger, null);
                m_debugUtils.Dispose();
            }
        }

        public string Name => "Vulkan Debugger";

        public string Identifier => String.Empty;

        public bool ShouldLoad { get; }

        public IEnumerable<string> GetDependencies()
        {
            return new List<string>
            {
                VkPlatform.VK_VALIDATE_LAYER_NAME
            };
        }

        public DebugUtilsMessengerCreateInfoEXT GetInstanceStartup()
        {
            return new DebugUtilsMessengerCreateInfoEXT
            {
                SType = StructureType.DebugUtilsMessengerCreateInfoExt,
                MessageSeverity =
                    DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt |
                    DebugUtilsMessageSeverityFlagsEXT.WarningBitExt |
                    DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt,
                MessageType =
                    DebugUtilsMessageTypeFlagsEXT.GeneralBitExt |
                    DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt |
                    DebugUtilsMessageTypeFlagsEXT.ValidationBitExt,
                PfnUserCallback = (DebugUtilsMessengerCallbackFunctionEXT)HandleDebug,
                PNext = null
            };
        }
        
        public void Initialize()
        {
            if (!m_platform.Vk.TryGetInstanceExtension(m_platform.Instance, out m_debugUtils))
                return;

            var info = GetInstanceStartup();

            SafeExecute(d => d.CreateDebugUtilsMessenger(m_platform.Instance, in info, null, out m_messenger));
        }

        private void SafeExecute(Func<ExtDebugUtils, Result> cb)
        {
            Result res = cb(m_debugUtils);

            if (res != Result.Success)
                throw new VulkanException(res);
        }

        private uint HandleDebug(
            DebugUtilsMessageSeverityFlagsEXT messageSeverity, 
            DebugUtilsMessageTypeFlagsEXT messageTypes, 
            DebugUtilsMessengerCallbackDataEXT* pCallbackData, 
            void* pUserData)
        {
            var logLevel = messageSeverity switch
            {
                DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt => LogLevel.Trace,
                DebugUtilsMessageSeverityFlagsEXT.WarningBitExt => LogLevel.Warn,
                DebugUtilsMessageSeverityFlagsEXT.InfoBitExt => LogLevel.Info,
                DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt => LogLevel.Error,
                _ => LogLevel.Debug
            };

            if (m_log.LevelEnabled(logLevel))
            {
                string msg = Marshal.PtrToStringAnsi((nint)pCallbackData->PMessage);

                m_log.Log(logLevel, null, msg);
            }

            return Vk.False;
        }
    }
}
