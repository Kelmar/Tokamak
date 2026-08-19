using System.Collections.Generic;

using Tokamak.Import.Builders;

using Tokamak.Tritium.Geometry;

namespace Tokamak.Tritium.Builders;

public class SkeletonBuilder
{
    private readonly Dictionary<string, int> m_nameIndexMap = [];
    private readonly List<Bone> m_bones = [];
    
    public void AddBone(BoneInfo info, int parentIndex = -1)
    {
        var bone = new Bone
        {
            Index = m_bones.Count,
            ParentIndex = parentIndex,
            Transform = info.Transform
        };
        
        m_bones.Add(bone);
        m_nameIndexMap[info.Name] = bone.Index;
        
        foreach (var child in info.Children)
            AddBone(child, bone.Index);
    }

    public Skeleton Build()
    {
        return new Skeleton(m_nameIndexMap, m_bones);
    }
}