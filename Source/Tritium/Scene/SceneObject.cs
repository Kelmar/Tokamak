using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tokamak.Tritium.APIs;

namespace Tokamak.Tritium.Scene
{
    public abstract class SceneObject : IDisposable
    {
        protected SceneObject()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                SceneManager?.RemoveObject(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The SceneManager that is handling this object.
        /// </summary>
        public SceneManager SceneManager { get; internal set; } = null;

        /// <summary>
        /// Flags for changing the behavior of how the object is rendered/managed by the SceneManager.
        /// </summary>
        public SceneObjectFlag Flags { get; set; } = 0;

        public abstract void Render(ICommandList commandList);
    }
}
