using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Channels;

using Silk.NET.Input;

using Tokamak.Abstractions.Input;
using Tokamak.Hosting.Abstractions;
using Tokamak.Tritium.Scene;

namespace TestBed
{
    public sealed class PlayerController : IDisposable
    {
        private readonly SceneManager m_scene;

        private readonly IInputManager m_inputManager;
        private readonly IGameLifetime m_gameLifetime;

        private float m_speed = 2;

        private float m_uDelta = 0;
        private float m_vDelta = 0;

        public PlayerController(
            SceneManager scene,
            IInputManager inputManager,
            IGameLifetime gameLifetime)
        {
            m_scene = scene;

            m_inputManager = inputManager;
            m_gameLifetime = gameLifetime;

            m_scene.Camera.Location = new Vector3(0, 1, 5);
            //m_scene.Camera.Location = new Vector3(0, 75, 175);
            m_scene.Camera.Forward = new Vector3(0, 0, -1);

            m_inputManager.KeyDown += OnKeyDown;
            m_inputManager.KeyUp += OnKeyUp;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            m_inputManager.KeyUp -= OnKeyUp;
            m_inputManager.KeyDown -= OnKeyDown;
        }

        private void OnKeyDown(int key)
        {
            Key k = (Key)key;

            switch (k)
            {
            case Key.Escape:
                m_gameLifetime.Shutdown();
                break;

            case Key.W: m_uDelta = -1; break;
            case Key.A: m_vDelta = -1; break;
            case Key.S: m_uDelta = 1; break;
            case Key.D: m_vDelta = 1; break;
            }

            /*
            if (KeyboardState.IsKeyReleased(Keys.W))
                m_render.WireFrame = !m_render.WireFrame;

            if (KeyboardState.IsKeyReleased(Keys.D))
                m_render.Debug = !m_render.Debug;
            */
        }

        private void OnKeyUp(int key)
        {
            Key k = (Key)key;

            switch (k)
            {
            case Key.W: m_uDelta = 0; break;
            case Key.A: m_vDelta = 0; break;
            case Key.S: m_uDelta = 0; break;
            case Key.D: m_vDelta = 0; break;
            }
        }

        public void Update(double timeDelta)
        {
            var l = m_scene.Camera.Location + (new Vector3(m_vDelta, 0, m_uDelta) * (float)timeDelta * m_speed);
            m_scene.Camera.Location = l;
        }
    }
}
