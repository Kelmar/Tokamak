﻿namespace Tokamak.Readers.FBX.ObjectWrappers
{
    internal class MaterialWrapper : IFBXObject
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public Node Node { get; set; }
    }
}
