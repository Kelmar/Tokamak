using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

using Tokamak.Import.Builders;

using Tokamak.Readers.FBX.DOM;
using Tokamak.Tritium.Geometry;

namespace Tokamak.Readers.FBX.Passes
{
    /// <summary>
    /// Builds assets from the previous read passes.
    /// </summary>
    /// <remarks>
    /// Third pass:
    /// 
    /// Here we'll make calls to the IAssetBuilder to actually construct the imported assets.
    /// 
    /// Note that for textures that refer to external files; then they will get deferred to those files.
    /// </remarks>
    internal class BuildPass(IAssetBuilder builder, ReadState state) : IReadPass
    {
        private readonly IAssetBuilder m_builder = builder;
        private readonly ReadState m_state = state;

        public void Execute()
        {
            foreach (var material in m_state.Materials)
            {
                m_builder.NewMaterial(cfg => cfg
                    .WithName(material.Name)
                // TODO: Add material details here when we figure out a good interface for that.
                );
            }

            ProcessMeshes();
            ProcessSkeletons();

            foreach (var model in m_state.SceneObjects)
            {
                var meshNames = m_state.Meshes
                    .Where(m => model.MeshIds.Contains(m.Id))
                    .Select(m => m.Name);

                var skeleton = m_state.Skeletons
                    .FirstOrDefault(s => s.MeshId.HasValue && model.MeshIds.Contains(s.MeshId.Value));

                m_builder.NewSceneObject(cfg =>
                {
                    cfg.WithName(model.Name)
                        .AddMeshes(meshNames);

                    if (skeleton != null)
                        cfg.WithSkeleton(skeleton.Name);
                });
            }

            m_builder.BuildAll();
        }

        #region Mesh Processing

        private IEnumerable<Vector4> GetMeshColors(long meshId)
        {
            var sceneObj = m_state.SceneObjects.FirstOrDefault(o => o.MeshIds.Contains(meshId));

            if (sceneObj == null)
                yield break;

            foreach (var id in sceneObj.MaterialIds)
            {
                var material = m_state.Materials.FirstOrDefault(m => m.Id == id);

                if (material == null)
                    yield return Vector4.One; // Default to white if not found.
                else
                    yield return material.DiffuseColor;
            }
        }

        private void ProcessMeshes()
        {
            foreach (var mesh in m_state.Meshes)
            {
                var colors = GetMeshColors(mesh.Id).ToList();

                //m_builder.NewMesh(cfg => cfg
                //    .WithName(mesh.Name)
                //    .WithPolygons(mesh.Polygons, (p, polyCfg) => polyCfg
                //        .AddVertices(p.Vertices.Select(v => v.Vertex))
                //        .AddNormals(p.Vertices.Select(v => v.Normal))
                //        .AddUVs(p.Vertices.Select(v => v.TexCoord))
                //        .AddColors(p.Vertices.Select(v =>
                //        {
                //            if (v.MaterialIndex < 0 || v.MaterialIndex >= colors.Count)
                //                return Vector4.One;

                //            return colors[v.MaterialIndex];
                //        }))
                //    )
                //);
            }
        }

        #endregion

        #region Skeleton Processing

        private class SkeletonClosure(SkeletonInfo skeleton)
        {
            private readonly SkeletonInfo m_skeleton = skeleton;

            public void ProcessBone(BoneInfo bone, IBoneBuilder builder)
            {
                var children = m_skeleton.Bones.Where(b => b.ParentBoneId == bone.Id);

                builder
                    .WithName(bone.Name)
                    .WithTransform(bone.Transform)
                    .ForIndices(bone.Indices)
                    .WithWeights(bone.Weights)
                    .WithChildBones(children, ProcessBone);
            }
        }

        private void ProcessSkeletons()
        {
            foreach (var skeleton in m_state.Skeletons)
            {
                var closure = new SkeletonClosure(skeleton);
                var roots = skeleton.Bones.Where(b => !b.ParentBoneId.HasValue);

                m_builder.NewSkeleton(cfg => cfg
                    .WithName(skeleton.Name)
                    .WithBones(roots, closure.ProcessBone)
                );
            }
        }

        #endregion
    }
}
