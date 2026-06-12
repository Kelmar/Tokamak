using System;

using Silk.NET.Input;

using Tokamak.Abstractions.Input;
using Tokamak.Abstractions.Silk;

using Tokamak.Tritium.APIs;

namespace Tokamak.Silk.Input
{
    internal sealed class SilkInputManager : IInputManager, IDisposable
    {
        public event KeyUp KeyUp;
        public event KeyDown KeyDown;

        private ISilkWindow m_window;
        private IInputContext? m_input = null;

        public SilkInputManager(IGraphicsLayer gfxLayer)
        {
            var win = gfxLayer as ISilkWindow;

            if (win == null)
                throw new Exception("GraphicsLayer is does not use Silk!");

            m_window = win;

            // Have to defer creation of the InputContext object.
            m_window.View.Load += InitInput;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            m_window.View.Load -= InitInput;

            m_input?.Dispose();
        }

        private void InitInput()
        {
            m_input = m_window.View.CreateInput();

            foreach (var kb in m_input.Keyboards)
            {
                kb.KeyDown += OnKeyDown;
                kb.KeyUp += OnKeyUp;
            }
        }

        private void OnKeyUp(IKeyboard source, Key key, int count)
        {
            KeyUp?.Invoke((int)key);
        }

        private void OnKeyDown(IKeyboard source, Key key, int count)
        {
            KeyDown?.Invoke((int)key);
        }
    }
}
