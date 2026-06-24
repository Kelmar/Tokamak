using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;

using TestBed.Scenes;

using Tokamak.Abstractions.Input;

using Tokamak.Hosting.Abstractions;
using Tokamak.Logging.Abstractions;

using Tokamak.Assets;
using Tokamak.Import.Builders;

using Tokamak.Mathematics;

using Tokamak.Readers.FBX;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Scene;

namespace TestBed
{
    public class TestGameApp : IGameApp
    {
        private readonly AssetManager m_assetManager;
        private readonly IGraphicsLayer m_gfxLayer;
        private readonly IInputManager m_inputManager;
        private readonly IGameLifetime m_gameLifetime;

        private readonly Func<IAssetBuilder> m_builderFactory;

        private const float ROT_AMOUNT = 1;//0.5f;

        //private ILogger m_log;
        
        private SceneManager? m_scene = null;

        AssetReference<SceneMeshObject>? m_mesh;

        private PlayerController? m_playerController;

        //private readonly List<IRenderable> m_renderers = new List<IRenderable>();

        private float m_rot;

        //public const string FILE = "resources/cube.fbx";
        //public const string FILE = "resources/blox.fbx";
        //public const string FILE = "resources/susan.fbx";
        //public const string FILE = "resources/plane.fbx";
        public const string FILE = "resources/chest.fbx";

        /*
         * This is the X Bot model from Maxima.
         * I'm not sure what the license is for that, so I'm not adding it to the repo.
         */
        //public const string FILE = "resources/xbot.fbx";
        //public const string FILE = "resources/amy.fbx";

        public TestGameApp(
            //ILogger log,
            AssetManager assetManager,
            IGraphicsLayer layer,
            IInputManager inputManager,
            IGameLifetime gameLifetime,
            Func<IAssetBuilder> builderFactory)
        {
            //m_log = log;
            m_assetManager = assetManager;
            m_gfxLayer = layer;
            m_inputManager = inputManager;
            m_gameLifetime = gameLifetime;

            m_builderFactory = builderFactory;

            m_playerController = null;
        }

        public void Dispose()
        {
            if (m_mesh != null)
            {
                m_scene!.RemoveObject(m_mesh.Asset);
                m_mesh.Dispose();
            }

            m_scene?.Dispose();
        }

        public void OnShutdown()
        {
        }

        public void OnLoad()
        {
            var initializer = new SceneInitializer(m_gfxLayer);

            m_scene = new SceneManager(m_gfxLayer, initializer);

            LoadObject();

            m_playerController = new PlayerController(m_scene, m_inputManager, m_gameLifetime);

            //m_scene.Camera.Location = new Vector3(0, 75, 175);
            //m_scene.Camera.Location = new Vector3(0, 1, 5);
            //m_scene.Camera.LookAt = Vector3.Zero;
            //m_scene.Camera.Forward = new Vector3(0, 0, -1);

            //m_renderers.Add(m_scene);
            //m_renderers.Add(m_canvas);

            //var stream = File.Open("resources/test.svg", FileMode.Open, FileAccess.Read);
            //var reader = new SVGReader(stream);

            //reader.Import();
        }

        void LoadObject()
        {
            //using var reader = new FBXReader(File.OpenRead(FILE));

            //var objects = reader.Import().ToList();

            //foreach (var item in objects.OfType<Mesh>())
            //    m_mesh.AddMesh(item);

            //m_scene.AddObject(m_mesh);

            var reader = new FBXImportDirector(m_builderFactory());
            reader.Import(FILE);

            //m_mesh = m_assetManager.Find<SceneMeshObject>("Cube");
            m_mesh = m_assetManager.Find<SceneMeshObject>("Chest");
            //m_mesh = m_assetManager.Find<SceneMeshObject>("Ch46"); // Amy

            if (m_mesh != null)
                m_scene!.AddObject(m_mesh.Asset);
        }

        public void OnRender(double timeDelta)
        {
            //RenderUI();

            m_scene!.RenderAll();

            /*
            foreach (var r in m_renderers)
                r.Render();
            */
        }

        public void OnUpdate(double timeDelta)
        {
            m_playerController?.Update(timeDelta);

            m_rot += (float)(ROT_AMOUNT * timeDelta);
            //m_rot += 1;

            while (m_rot >= 360)
                m_rot -= 360;

            m_mesh?.Asset.Rotation = new Vector3(0, m_rot, 0);
        }
    }
}
