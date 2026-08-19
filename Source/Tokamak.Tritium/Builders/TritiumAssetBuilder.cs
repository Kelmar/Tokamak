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

        public TritiumAssetBuilder(AssetManager assetManager, IGraphicsLayer gfxLayer)
        {
            m_assetManager = assetManager;
            m_gfxLayer = gfxLayer;
        }

        public void NewSceneObject(string name, Action<ISceneObjectBuilder> configure)
        {
            var objectBuilder = new SceneObjectBuilder(m_assetManager, m_gfxLayer);
            objectBuilder.WithName(name);
            configure(objectBuilder);
            objectBuilder.Build();
        }

        public void NewMaterial(Action<IMaterialBuilder> configure)
        {
            var materialBuilder = new MaterialBuilder();
            configure(materialBuilder);
            materialBuilder.Build();
        }

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

        public void NewSkeleton(SkeletonInfo info)
        {
            var builder = new SkeletonBuilder();
            
            foreach (var bone in info.Bones)
                builder.AddBone(bone);

            Skeleton skeleton = builder.Build();

            try
            {
                m_assetManager.RegisterAsset(info.Name, skeleton);
            }
            catch
            {
                skeleton.Dispose();
                throw;
            }
        }
    }
}
