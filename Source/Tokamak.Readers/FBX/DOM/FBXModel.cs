using System.Collections.Generic;
using System.Linq;

using Tokamak.Readers.FBX.Builders;

namespace Tokamak.Readers.FBX.DOM
{
    internal class FBXModel : FBXObject
    {
        public FBXModel(ReadState state, Node node)
            : base(state, node)
        {
            MaterialIds = [];
            MeshIds = [];

            //Materials = [];
            Meshes = [];
        }

        public List<long> MaterialIds
        {
            get;
            set => field = value ?? [];
        }

        //public List<FBXMaterial> Materials
        //{
        //    get;
        //    set => field = value ?? [];
        //}

        public List<long> MeshIds
        {
            get;
            set => field = value ?? [];
        }

        public List<FBXMesh> Meshes
        {
            get;
            set => field = value ?? [];
        }
    }
}
