using System;
using System.Collections.Generic;
using System.Text;

using Tokamak.Readers.FBX.DOM;

namespace Tokamak.Readers.FBX
{
    internal interface IFBXObjectReader
    {
        string ObjectType { get; }

        void ReadObject(FBXObject obj);
    }
}
