using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Tokamak.Readers.FBX.DOM;

namespace Tokamak.Readers.FBX.Readers
{
    internal class DeformerReader : IFBXObjectReader
    {
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

            var indicies = bone.Node.Children.WithFBXType("Indexes")
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
                Indicies = indicies,
                Weights = weights
            };

            return boneInfo;
        }

        private void ReadSkeleton(FBXObject obj)
        {
            var bones = new List<BoneInfo>();

            foreach (var child in obj.Children.WithFBXType("deformer"))
            {
                bones.Add(ReadBone(child));
            }
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
