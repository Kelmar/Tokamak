using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Tokamak.Logging.Abstractions;

using Tokamak.Utilities;

namespace Tokamak.Assets
{
    /// <summary>
    /// Manages asset references.
    /// </summary>
    /// <remarks>
    /// The asset manager manages a list of loaded assets and returns new references to those assets.
    /// 
    /// The references are counted, and when the last one is disposed of then the asset itself is disposed of.
    /// </remarks>
    public class AssetManager : IDisposable
    {
        private class AssetInfo
        {
            public ReferenceCounter ReferenceCounter { get; } = new();

            [NotNull]
            public IDisposable? Asset { get; set; }
        }

        /*
         * The lock is needed because we want to make sure that there isn't away 
         * for multiple requests to accidentally get an invalid disposed of reference.
         */
        private readonly SlimLock m_lock = new();
        private readonly ILogger m_log;
        private readonly Dictionary<string, AssetInfo> m_assets = [];
        private readonly Dictionary<Type, IAssetFactory> m_factories;

        public AssetManager(ILogger<AssetManager> log, ICollection<IAssetFactory> assetFactories)
        {
            m_log = log;
            m_factories = assetFactories.ToDictionary(f => f.ForType, f => f);
        }

        public void Dispose()
        {
            m_lock.Dispose();
            GC.SuppressFinalize(this);
        }

        //private void CollectGarbage()
        //{
        //    var toRemove = new List<string>();

        //    using var mx = m_lock.Lock();

        //    foreach (var kvp in m_assets)
        //    {
        //        if (kvp.Value.ReferenceCounter.Value < 1)
        //            toRemove.Add(kvp.Key);
        //    }

        //    foreach (var key in toRemove)
        //        m_assets.Remove(key);
        //}

        private AssetReference<TAsset>? BuildNoLock<TAsset>(string path)
            where TAsset : Asset
        {
            // Called inside of Find() which acquires needed lock.

            var assetType = typeof(TAsset);

            if (!m_factories.TryGetValue(assetType, out var factory))
            {
                //m_log.Warn("No factory method registered for asset type {assetType}", assetType);
                m_log.Warn("No factory method registered for asset type {0}", assetType);
                m_assets.Remove(path);
                return null;
            }

            //m_log.Debug("Loading {assetType} asset: {assetPath}", assetType, path);
            m_log.Debug("Loading {assetType} asset: {0}", assetType, path);
            var result = (TAsset)factory.Build();

            if (result == null)
            {
                //m_log.Warn("Factory for asset type {assetType} returned null.", assetType);
                m_log.Warn("Factory for asset type {0} returned null.", assetType);
                m_assets.Remove(path);
                return null;
            }

            result.ID = path;

            if (!m_assets.TryGetValue(path, out var info))
                m_assets[path] = info = new AssetInfo();

            info.Asset = result;

            return new AssetReference<TAsset>(info.ReferenceCounter, result);
        }

        /// <summary>
        /// Register an asset with the given path.
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="path">The path to the asset.</param>
        /// <param name="asset">The asset to be registered.</param>
        /// <param name="overwrite">If set and an asset already exists at the given path, then the old one is overwritten.</param>
        /// <returns>An asset reference to the register asset, or null if the asset could not be registered.</returns>
        public AssetReference<TAsset>? RegisterAsset<TAsset>(string path, TAsset asset, bool overwrite = true)
            where TAsset : Asset
        {
            using var mx = m_lock.Lock();

            if (!m_assets.TryGetValue(path, out var info))
                m_assets[path] = info = new AssetInfo();
            else
            {
                int refCount = info.ReferenceCounter.Value;

                if (refCount > 0)
                {
                    if (!overwrite)
                        return null;

                    //m_log.Warn("New asset over writing asset at: {assetPath}", path);
                    m_log.Warn("New asset over writing asset at: {0}", path);
                    m_assets[path] = info = new AssetInfo();
                }
            }

            //m_log.Debug("Setting {assetType} asset: {assetPath}", typeof(TAsset), path);
            m_log.Debug("Setting {0} asset: {1}", typeof(TAsset).Name, path);

            info.Asset = asset;
            asset.ID = path;

            return new AssetReference<TAsset>(info.ReferenceCounter, asset);
        }

        /// <summary>
        /// Search for an asset at the given path.
        /// </summary>
        /// <typeparam name="TAsset">The type of asset we're searching for.</typeparam>
        /// <param name="path">The path for the asset.</param>
        /// <param name="create">Set if the asset should be loaded/created if not found.</param>
        /// <returns>An <see cref="AssetReference"/> object if found or created.  Otherwise return null if not.</returns>
        public AssetReference<TAsset>? Find<TAsset>(string path, bool create = true)
            where TAsset : Asset
        {
            using var mx = m_lock.Lock();

            if (m_assets.TryGetValue(path, out var info))
            {
                int res = info.ReferenceCounter.Increment();

                Debug.Assert(res >= 1, "Asset disposed multiple times?");

                if (res <= 1)
                {
                    if (!create)
                    {
                        m_assets.Remove(path);
                        return null;
                    }

                    // Asset disposed of while we we're grabbing the record, recreate.
                    return create ? BuildNoLock<TAsset>(path) : null;
                }

                return new AssetReference<TAsset>(info.ReferenceCounter, (TAsset)info.Asset);
            }
            else if (create)
                return BuildNoLock<TAsset>(path);

            return null;
        }
    }
}
