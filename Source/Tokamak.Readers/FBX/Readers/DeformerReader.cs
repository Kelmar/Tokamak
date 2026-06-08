using System;
using System.Collections.Generic;
using System.Text;

using Tokamak.Readers.FBX.DOM;

namespace Tokamak.Readers.FBX.Readers
{
    internal class DeformerReader : IFBXObjectReader
    {
        public DeformerReader(ReadState state)
        {
            State = state;
        }

        public string ObjectType => "deformer";

        public ReadState State { get; }

        private void ReadSkeleton(FBXObject obj)
        {

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
