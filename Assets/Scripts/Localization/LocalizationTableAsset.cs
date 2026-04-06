using System;
using System.Collections.Generic;
using UnityEngine;

namespace Breach.Localization
{
    [CreateAssetMenu(menuName = "Breach/Localization/Table", fileName = "LocalizationTable")]
    public sealed class LocalizationTableAsset : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public string key;
            public string value;
        }

        [SerializeField] private List<Entry> entries = new();

        private Dictionary<string, string> cache;

        public string Resolve(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            EnsureCache();
            return cache.TryGetValue(key, out var value) ? value : key;
        }

        public void SetEntries(IEnumerable<Entry> newEntries)
        {
            entries = new List<Entry>(newEntries);
            cache = null;
        }

        private void EnsureCache()
        {
            if (cache != null)
            {
                return;
            }

            cache = new Dictionary<string, string>(StringComparer.Ordinal);
            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (string.IsNullOrWhiteSpace(entry.key))
                {
                    continue;
                }

                cache[entry.key] = entry.value ?? string.Empty;
            }
        }
    }
}
