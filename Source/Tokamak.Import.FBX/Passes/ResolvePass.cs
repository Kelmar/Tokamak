using System;
using System.Linq;

using Tokamak.Logging.Abstractions;

using Tokamak.Mathematics;

using Tokamak.Import.FBX.DOM;

using BoneWeighting = Tokamak.Import.Builders.BoneWeighting;

namespace Tokamak.Import.FBX.Passes
{
    /// <summary>
    /// Resolves references from the first pass.
    /// </summary>
    /// <remarks>
    /// Second pass:
    /// 
    /// This pass takes any FBX based IDs read in the first pass and resolves
    /// them to the C# classes.
    /// 
    /// This pass also handles creating any names for assets where needed
    /// for the asset manager.
    /// </remarks>
    internal class ResolvePass(ILogger log, ReadState state) : IReadPass
    {
        private ILogger m_log = log;

        private readonly ReadState m_state = state;

        public void Execute()
        {
            CreateMissingObjectNames();
            CreateMissingMeshNames();
            ProcessSkeletons();
        }

        #region Object Processing

        private void CreateMissingObjectNames()
        {
            foreach (var obj in m_state.SceneObjects)
            {
                if (!String.IsNullOrWhiteSpace(obj.Name))
                    continue;

                obj.Name = $"{m_state.FileName}_{obj.Id}";
            }
        }

        #endregion Object Processing

        #region Mesh Processing

        private void CreateMissingMeshNames()
        {
            foreach (var mesh in m_state.Meshes)
            {
                if (!String.IsNullOrWhiteSpace(mesh.Name))
                    continue;

                var parent = m_state.SceneObjects
                    .FirstOrDefault(o => o.MeshIds.Contains(mesh.Id));

                mesh.Name = parent != null ?
                    $"{parent.Name}_mesh_{mesh.Id}" :
                    $"{m_state.FileName}_mesh_{mesh.Id}";
            }
        }

        #endregion Mesh Processing

        #region Skeleton Processing

        private void ProcessSkeletons()
        {
            foreach (var skeleton in m_state.Skeletons)
            {
                var parentMesh = m_state.Meshes
                    .FirstOrDefault(m => m.Id == skeleton.MeshId);

                SetSkeletonName(skeleton, parentMesh);
                MapSkeletonWeights(skeleton, parentMesh);
            }
        }

        private void SetSkeletonName(SkeletonInfo skeleton, MeshInfo? parentMesh)
        {
            if (!String.IsNullOrWhiteSpace(skeleton.Name))
                return;

            skeleton.Name = parentMesh != null ?
                $"{parentMesh.Name}_skeleton_{skeleton.Id}" :
                $"{m_state.FileName}_skeleton_{skeleton.Id}";
        }

        private void MapSkeletonWeights(SkeletonInfo skeleton, MeshInfo? mesh)
        {
            if (mesh == null)
                return;

            foreach (var bone in skeleton.Bones)
                MapBoneWeights(skeleton, bone, mesh);
        }

        private void MapBoneWeights(SkeletonInfo skeleton, BoneInfo bone, MeshInfo mesh)
        {
            for (int i = 0; i < bone.Indices.Length; ++i)
            {
                int index = bone.Indices[i];
                float weight = bone.Weights[i];

                if (float.AlmostEquals(weight, 0))
                    continue; // Ignore superfluous weights

                var vert = mesh.Vertices.FirstOrDefault(v => v.FBXIndex == index);

                if (vert == null)
                {
                    m_log.Warn("Unable to find vertex with FBX index {0} for skeleton {1}->bone {2}", index, skeleton.Name, bone.Name);
                    continue;
                }

                vert.BoneWeights.Add(new BoneWeighting
                {
                    BoneIndex = bone.Index,
                    BoneWeight = weight
                });
            }
        }

        #endregion Skeleton Processing
    }
}
