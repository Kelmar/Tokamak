using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Logging.Abstractions;

using Tokamak.Import.Builders;

using Tokamak.Import.FBX.DOM;

namespace Tokamak.Import.FBX.Passes;

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
internal class BuildPass : IReadPass
{
    private readonly ILogger m_log;
    private readonly IAssetBuilder m_builder;
    private readonly ReadState m_state;

    public BuildPass(ILogger log, IAssetBuilder builder, ReadState state)
    {
        m_log = log;
        m_builder = builder;
        m_state = state;
    }

    public void Execute()
    {
        foreach (var material in m_state.Materials)
        {
            m_builder.NewMaterial(cfg => cfg
                .WithName(material.Name)
                // TODO: Add material details here when we figure out a good interface for that.
            );
            
            m_log.Info("Added {0} matierlal", material.Name);
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

            m_builder.NewSceneObject(model.Name, cfg =>
            {
                cfg.AddMeshes(meshNames);

                if (skeleton != null)
                    cfg.WithSkeleton(skeleton.Name);
            });
            
            m_log.Info("Added {0} object", model.Name);
        }
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

    private static VertexInfo ToBuilderVert(FBXVertex vert, List<Vector4> colors)
    {
        Vector4 clr =
            (vert.MaterialIndex < 0 || (vert.MaterialIndex >= colors.Count)) ?
                Vector4.One :
                colors[vert.MaterialIndex];

        return new VertexInfo
        {
            Vector = vert.Vertex,
            Normal = vert.Normal,
            TexCoord = vert.TexCoord,
            Color = clr,
            BoneWeights = vert.BoneWeights
        };
    }

    private static PolygonInfo ToBuilderPoly(FBXPolygon poly)
    {
        return new PolygonInfo
        {
            Vertices = poly.Vertices
        };
    }

    private void ProcessMeshes()
    {
        foreach (var mesh in m_state.Meshes)
        {
            var colors = GetMeshColors(mesh.Id).ToList();

            m_builder.NewMesh(new MeshInfo
            {
                Name = mesh.Name,
                Vertices = mesh.Vertices
                    .OrderBy(v => v.Index)
                    .Select(v => ToBuilderVert(v, colors))
                    .ToList(),
                Polygons = mesh.Polygons.Select(ToBuilderPoly).ToList()
            });
        }
    }

    #endregion

    #region Skeleton Processing

    private BoneInfo ProcessBone(FBXSkeleton fbxSkeleton, FBXBone fbxBone)
    {
        IEnumerable<FBXBone> children = fbxSkeleton.Bones.Where(b => b.ParentBoneId == fbxBone.Id);
        
        return new BoneInfo
        {
            Name = fbxBone.Name,
            Transform = fbxBone.Transform,
            Children = children.Select(b => ProcessBone(fbxSkeleton, b)).ToList()
        };
    }

    private IEnumerable<BoneInfo> BuildBones(FBXSkeleton fbxSkeleton)
    {
        IEnumerable<FBXBone> roots = fbxSkeleton.Bones.Where(b => !b.ParentBoneId.HasValue);
        
        foreach (var bone in roots)
            yield return ProcessBone(fbxSkeleton, bone);
    }

    private void ProcessSkeletons()
    {
        foreach (var skeleton in m_state.Skeletons)
        {
            m_builder.NewSkeleton(new SkeletonInfo
            {
                Name = skeleton.Name,
                Bones = BuildBones(skeleton).ToList()
            });
        }
    }

    #endregion
}