using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Mathematics;

using Tokamak.Readers.FBX.DOM;
using Tokamak.Readers.FBX.Mappers;

namespace Tokamak.Readers.FBX.Readers
{
    internal class DeformerReader : IFBXObjectReader
    {
        private readonly ReadState m_state;
        private readonly FBXObject m_fbxObject;

        public DeformerReader(ReadState state, FBXObject obj)
        {
            m_state = state;
            m_fbxObject = obj;
        }

        private long? FindParentBoneID(FBXObject bone, FBXObject? limb)
        {
            // Next we need to find the parent Model/LimbNode of that object....
            var parentLimb = limb?.Parents
                .WithFBXType("Model")
                .FirstOrDefault(p => p.IsSubClass("LimbNode"));

            // Finally we see if we can find the deformer that owns that node, which should be our parent bone.
            var parentDeformer = parentLimb?.Parents
                .WithFBXType("Deformer")
                .FirstOrDefault();

            return parentDeformer?.Id;
        }

        private BoneInfo ReadBone(FBXObject bone)
        {
            // We have a bit of a long walk through the tree to find the parent of this bone.

            // We first need to find the owned Model/LimbNode of this bone....
            var limb = bone.Children
                .WithFBXType("model")
                .FirstOrDefault(c => c.IsSubClass("LimbNode"));

            var indices = bone.Node.Children.WithFBXType("Indexes")
                .Where(n => n.Properties.Count > 0)
                .SelectMany(n => n.Properties[0].AsEnumerable<int>())
                .ToArray();

            var weights = bone.Node.Children.WithFBXType("Weights")
                .Where(n => n.Properties.Count > 0)
                .SelectMany(n => n.Properties[0].AsEnumerable<double>())
                .Select(d => (float)d) // .....
                .ToArray();

            var transformNode = bone.Node.Children.WithFBXType("Transform").FirstOrDefault();
            Matrix4x4 transform = Matrix4x4.Identity;

            if (transformNode != null && transformNode.Properties.Count > 0)
            {
                var matValues = transformNode.Properties[0]
                    .AsEnumerable<double>()
                    .Select(v => (float)v)
                    .ToArray();

                transform = Matrix4x4.CreateFromColumnArray(matValues);
            }

            // Now continue to find the parent ID of the bone.
            long? parentId = FindParentBoneID(bone, limb);

            string? name = String.IsNullOrWhiteSpace(bone.Name) ? limb?.Name : bone.Name;

            var boneInfo = new BoneInfo
            {
                Id = bone.Id,
                ParentBoneId = parentId,
                Name = name ?? String.Empty,
                Indices = indices,
                Weights = weights,
                Transform = transform
            };

            limb?.Properties.MapTo(boneInfo);

            return boneInfo;
        }

        private static IEnumerable<BoneInfo> SortBones(IEnumerable<BoneInfo> allBones, long? parentId)
        {
            var children = allBones
                .Where(b => b.ParentBoneId == parentId)
                .ToList();

            foreach (var child in children)
            {
                yield return child;

                foreach (var item in SortBones(allBones, child.Id))
                    yield return item;
            }
        }

        private BoneInfo SetBoneIndex(BoneInfo bone, int index)
        {
            bone.Index = index;
            return bone;
        }

        private List<BoneInfo> GetSortedBones()
        {
            var bones = m_fbxObject.Children
                .WithFBXType("Deformer")
                .Select(ReadBone)
                .ToList();

            // Sort the bones based on their parent IDs
            bones = SortBones(bones, null)
                .Select(SetBoneIndex)
                .ToList();

            foreach (BoneInfo bone in bones)
            {
                if (!bone.ParentBoneId.HasValue)
                    continue;

                BoneInfo? parent = bones.FirstOrDefault(b => b.Id == bone.ParentBoneId);

                if (parent == null)
                {
                    // TODO: Maybe warn on this condition?
                    continue;
                }

                bone.ParentIndex = parent.Index;
            }

            return bones;
        }

        private void ReadSkeleton()
        {
            FBXObject? meshObj = m_fbxObject.Parents
                .WithFBXType("Geometry")
                .FirstOrDefault();

            var result = new SkeletonInfo
            {
                Id = m_fbxObject.Id,
                Name = m_fbxObject.Name,
                MeshId = meshObj?.Id,
                Bones = GetSortedBones()
            };

            m_state.Skeletons.Add(result);
        }

        public void Process()
        {
            if (m_fbxObject.IsClass("Deformer"))
            {
                ReadSkeleton();
                return;
            }
        }
    }
}
