using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tokamak.Readers.FBX.DOM;
using Tokamak.Readers.FBX.Mappers;

namespace Tokamak.Readers.FBX.Readers
{
    internal class DeformerReader : IFBXObjectReader
    {
        private int m_importSkelCount = 0;
        private int m_importBoneCount = 0;

        public DeformerReader(ReadState state)
        {
            State = state;
        }

        public string ObjectType => "deformer";

        public ReadState State { get; }

        private long? FindParentBoneID(FBXObject bone, FBXObject? limb)
        {
            // We have a bit of a long walk through the tree to find the parent of this bone.

            // Next we need to find the parent Model/LimbNode of that object....
            var parentLimb = limb?.Parents
                .WithFBXType("model")
                .Where(p => p.IsSubClass("LimbNode"))
                .FirstOrDefault();

            // Finally we see if we can find the deformer that owns that node, which should be our parent bone.
            var parentDeformer = parentLimb?.Parents
                .WithFBXType("deformer")
                .FirstOrDefault();

            return parentDeformer?.Id;
        }

        private string GetSkeletonName(FBXObject skeleton, MeshInfo? mesh)
        {
            string name = skeleton.Name;

            if (String.IsNullOrWhiteSpace(skeleton.Name))
            {
                var nameBuilder = new StringBuilder();

                if (!String.IsNullOrWhiteSpace(mesh?.Name))
                    nameBuilder.Append(mesh.Name);
                else
                    nameBuilder.Append(State.FileName);

                nameBuilder.Append("_skel_");
                nameBuilder.Append(m_importSkelCount);

                name = nameBuilder.ToString();
            }

            return name;
        }

        private string GetBoneName(FBXObject bone, FBXObject? limb)
        {
            if (!String.IsNullOrWhiteSpace(bone.Name))
                return bone.Name;

            if (!String.IsNullOrWhiteSpace(limb?.Name))
                return limb.Name;

            return $"bone_{m_importBoneCount}";
        }

        private BoneInfo ReadBone(FBXObject bone)
        {
            // We first need to find the owned Model/LimbNode of this bone....
            var limb = bone.Children
                .WithFBXType("model")
                .Where(c => c.IsSubClass("LimbNode"))
                .FirstOrDefault();

            var indices = bone.Node.Children.WithFBXType("Indexes")
                .Where(n => n.Properties.Count > 0)
                .SelectMany(n => n.Properties[0].AsEnumerable<int>())
                .ToArray();

            var weights = bone.Node.Children.WithFBXType("Weights")
                .Where(n => n.Properties.Count > 0)
                .SelectMany(n => n.Properties[0].AsEnumerable<double>())
                .Select(d => (float)d) // .....
                .ToArray();

            long? parentId = FindParentBoneID(bone, limb);

            ++m_importBoneCount;

            var boneInfo = new BoneInfo
            {
                Id = bone.Id,
                ParentBoneId = parentId,
                Name = GetBoneName(bone, limb),
                Indices = indices,
                Weights = weights
            };

            limb?.Properties.MapTo(boneInfo);

            return boneInfo;
        }

        private void ReadSkeleton(FBXObject skeleton)
        {
            var bones = new List<BoneInfo>();

            foreach (var child in skeleton.Children.WithFBXType("deformer"))
            {
                bones.Add(ReadBone(child));
            }

            FBXObject? meshObj = skeleton.Parents.WithFBXType("Geometry").FirstOrDefault();
            MeshInfo? mesh = null;

            long? meshId = meshObj?.Id;
            mesh = State.Meshes.FirstOrDefault(m => m.Id == meshId);

            ++m_importSkelCount;

            var result = new SkeletonInfo
            {
                Id = skeleton.Id,
                Name = GetSkeletonName(skeleton, mesh),
                MeshId = meshId,
                Bones = bones
            };

            State.Skeletons.Add(result);
        }

        public void ReadObject(FBXObject obj)
        {
            if (obj.IsClass("Deformer"))
            {
                ReadSkeleton(obj);
                return;
            }
        }
    }
}
