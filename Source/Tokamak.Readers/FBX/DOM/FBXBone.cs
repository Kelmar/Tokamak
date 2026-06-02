using System;
using System.Collections.Generic;
using System.Text;

namespace Tokamak.Readers.FBX.DOM
{
    internal class FBXBone : FBXObject
    {
        public FBXBone(ReadState state, FBXBone parent, Node node)
            : base(state, node)
        {
        }
    }
}
