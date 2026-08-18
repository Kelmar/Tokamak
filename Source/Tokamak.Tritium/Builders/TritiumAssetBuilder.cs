using System;
using System.Collections.Generic;
using System.Linq;

using Tokamak.Assets;
using Tokamak.Import.Builders;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Geometry;

namespace Tokamak.Tritium.Builders
{
    public class TritiumAssetBuilder : IAssetBuilder
    {
        private readonly AssetManager m_assetManager;
        private readonly IGraphicsLayer m_gfxLayer;

        private readonly List<Action<IMaterialBuilder>> m_materialConfig = [];

        private readonly List<Action<ISkeletonBuilder>> m_skeletonConfig = [];

        private readonly List<Action<ISceneObjectBuilder>> m_objectConfig = [];

        public TritiumAssetBuilder(AssetManager assetManager, IGraphicsLayer gfxLayer)
        {
            m_assetManager = assetManager;
            m_gfxLayer = gfxLayer;
        }

        public void NewSceneObject(Action<ISceneObjectBuilder> configure)
           => m_objectConfig.Add(configure);

        public void NewMaterial(Action<IMaterialBuilder> configure)
            => m_materialConfig.Add(configure);

        private IEnumerable<Polygon> ToTriPolygon(MeshInfo mesh, PolygonInfo p, int index)
        {
            var verts = p.Vertices.Select(idx => mesh.Vertices[idx]);

            var poly = new Polygon
            {
                Vectors = verts.Select(v => v.Vector).ToList(),
                Normals = verts.Select(v => v.Normal).ToList(),
                TexCoord = verts.Select(v => v.TexCoord).ToList(),
                Colors = verts.Select(v => v.Color).ToList()
            };

            return poly.SplitIntoTriangles();
        }

        public void NewMesh(MeshInfo mesh)
        {
            var m = new Mesh(m_gfxLayer);

            try
            {
                var polygons = mesh.Polygons.SelectMany((x, i) => ToTriPolygon(mesh, x, i)).ToList();
                m.SetData(polygons);
                m_assetManager.RegisterAsset(mesh.Name, m);
            }
            catch
            {
                m.Dispose();
                throw;
            }
        }

        public void NewSkeleton(Action<ISkeletonBuilder> configure)
            => m_skeletonConfig.Add(configure);

        public void BuildAll()
        {
            foreach (var config in m_materialConfig)
            {
                var materialBuilder = new MaterialBuilder();
                config(materialBuilder);
                materialBuilder.Build();
            }

            foreach (var config in m_skeletonConfig)
            {
                var skeletonBuilder = new SkeletonBuilder(m_assetManager);
                config(skeletonBuilder);
                skeletonBuilder.Build();
            }

            foreach (var config in m_objectConfig)
            {
                var objectBuilder = new SceneObjectBuilder(m_assetManager, m_gfxLayer);
                config(objectBuilder);
                objectBuilder.Build();
            }

            m_materialConfig.Clear();
            m_skeletonConfig.Clear();
            m_objectConfig.Clear();
        }
    }
}
