using System;
using System.Collections.Generic;
using UnityEngine;

namespace TacticalBreach.Localization
{
    [CreateAssetMenu(menuName = "Breach/Localization/Table", fileName = "LocalizationTable")]
    public sealed class LocalizationTableAsset : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public string key;
            public string value;
            public string enValue;
            public string ruValue;
        }

        [SerializeField] private List<Entry> entries = new();

        private Dictionary<string, Entry> cache;

        public string Resolve(string key, string languageCode)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            EnsureCache();
            if (!cache.TryGetValue(key, out var entry))
            {
                return key;
            }

            return ResolveEntryValue(entry, languageCode, key);
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

            cache = new Dictionary<string, Entry>(StringComparer.Ordinal);
            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (string.IsNullOrWhiteSpace(entry.key))
                {
                    continue;
                }

                cache[entry.key] = entry;
            }
        }

        private static string ResolveEntryValue(Entry entry, string languageCode, string fallbackKey)
        {
            if (string.Equals(languageCode, "ru", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(entry.ruValue))
            {
                return entry.ruValue;
            }

            if (!string.IsNullOrWhiteSpace(entry.enValue))
            {
                return entry.enValue;
            }

            if (!string.IsNullOrWhiteSpace(entry.value))
            {
                return entry.value;
            }

            return fallbackKey;
        }
    }
}
