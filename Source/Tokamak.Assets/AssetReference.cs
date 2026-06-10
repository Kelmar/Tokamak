using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

using Tokamak.Utilities;

namespace Tokamak.Assets
{
    /// <summary>
    /// A reference to an asset.
    /// </summary>
    /// <remarks>
    /// Instances of this class maintain reference counts to a given asset.
    /// 
    /// When all instances have been disposed of the asset itself is disposed.
    /// </remarks>
    /// <typeparam name="TAsset">The type of asset managed.</typeparam>
    public sealed class AssetReference<TAsset> : IDisposable
        where TAsset : Asset
    {
        /// <summary>
        /// Global reference counter to the underlying asset.
        /// </summary>
        private readonly ReferenceCounter m_refCount;

        /// <summary>
        /// Object disposal flag.
        /// </summary>
        private int m_disposed = 0;

        public AssetReference(ReferenceCounter refCount, TAsset asset)
        {
            m_refCount = refCount;
            Asset = asset;
        }

        /// <summary>
        /// Disposes of the underlying asset when the last reference to that asset is disposed of.
        /// </summary>
        /// <remarks>
        /// The multiple calls to the Dispose() method of the same AssetReference object will only
        /// decrement the reference count once.
        /// </remarks>
        public void Dispose()
        {
            if (Interlocked.Exchange(ref m_disposed, 1) == 0)
            {
                GC.SuppressFinalize(this);

                int res = m_refCount.Decrement();

                Debug.Assert(res >= 0, "Asset disposed of multiple times?");

                if (res == 0)
                    Asset.Dispose();

                /*
                 * Should we be pedantic about referencing the Asset
                 * after calls to this Dispose() method?
                 * 
                 * I.e.: Should we set the Asset property to null?
                 */
            }
        }

        /// <summary>
        /// The managed asset.
        /// </summary>
        public TAsset Asset { get; }

        public override string ToString()
        {
            int cnt = m_refCount.Value;
            var sb = new StringBuilder();

            if (cnt > 1)
            {
                sb.Append(cnt);
                sb.Append(": ");
            }

            sb.Append(Asset);

            return sb.ToString();
        }
    }
}
