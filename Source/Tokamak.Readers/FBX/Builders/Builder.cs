using System;
using System.Collections.Generic;
using System.Text;

namespace Tokamak.Readers.FBX.Builders
{
    internal abstract class Builder
    {
        protected Builder(ReadState state)
        {
            State = state;
        }

        public ReadState State { get; }

        public GlobalSettings Settings => State.Settings;

        public ObjectGraph ObjectGraph => State.ObjectGraph;
    }
}
