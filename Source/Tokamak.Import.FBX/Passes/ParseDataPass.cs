using System;

using Tokamak.Import.FBX.DOM;
using Tokamak.Import.FBX.Readers;

namespace Tokamak.Import.FBX.Passes
{
    /// <summary>
    /// Interpret the data that was read in from the FBX file.
    /// </summary>
    /// <remarks>
    /// First pass:
    /// 
    /// Simplify FBXObjects into C# objects for second pass.  This
    /// will handle converting all the raw data into something a bit
    /// easier to work with in the C# world.
    /// </remarks>
    internal class ParseDataPass(ReadState state) : IReadPass
    {
        private readonly ReadState m_state = state;

        public void Execute()
        {
            //ProcessObjects("", obj => new TextureReader(m_state, obj));

            ProcessObjects("Material", obj => new MaterialReader(m_state, obj));
            ProcessObjects("Model", obj => new ModelReader(m_state, obj));
            ProcessObjects("Geometry", obj => new MeshReader(m_state, obj));
            ProcessObjects("Deformer", obj => new DeformerReader(m_state, obj));
        }

        private void ProcessObjects(string objType, Func<FBXObject, IFBXObjectReader> readerFactory)
        {
            foreach (var obj in m_state.ObjectGraph.GetObjectsOfType(objType))
            {
                var reader = readerFactory(obj);
                reader.Process();
            }
        }
    }
}
