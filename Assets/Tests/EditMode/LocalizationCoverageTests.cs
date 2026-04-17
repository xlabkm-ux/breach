#if UNITY_EDITOR && UNITY_INCLUDE_TESTS && BREACH_ENABLE_EDITMODE_TESTS
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Breach.Localization;
using NUnit.Framework;
using UnityEditor;

namespace Breach.Tests.EditMode
{
    public sealed class LocalizationCoverageTests
    {
        private static readonly Regex LocalizationKeyRegex = new Regex(
            @"LocalizationService\.(?:Resolve|ResolveFormat)\(""([^""]+)""",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        [Test]
        public void DefaultLocalizationTable_ShouldCoverAllRuntimeLocalizationKeys()
        {
            var table = AssetDatabase.LoadAssetAtPath<LocalizationTableAsset>(
                "Assets/Resources/Localization/DefaultLocalizationTable.asset");

            Assert.That(table, Is.Not.Null, "Default localization table asset was not found.");

            var tableEntries = GetEntries(table);
            var tableKeys = tableEntries.Select(entry => entry.key).Where(key => !string.IsNullOrWhiteSpace(key)).ToHashSet();
            var codeKeys = FindRuntimeLocalizationKeys("Assets/Scripts");

            Assert.That(codeKeys, Is.Not.Empty, "No runtime localization keys were found to validate.");

            var missingKeys = codeKeys.Where(key => !tableKeys.Contains(key)).OrderBy(key => key).ToArray();
            Assert.That(missingKeys, Is.Empty, $"Missing localization entries: {string.Join(", ", missingKeys)}");

            foreach (var key in codeKeys)
            {
                Assert.That(table.Resolve(key, "en"), Is.Not.EqualTo(key), $"English fallback missing for '{key}'.");
                Assert.That(table.Resolve(key, "ru"), Is.Not.EqualTo(key), $"Russian fallback missing for '{key}'.");
            }

            foreach (var entry in tableEntries)
            {
                if (string.IsNullOrWhiteSpace(entry.key))
                {
                    continue;
                }

                Assert.That(table.Resolve(entry.key, "en"), Is.Not.EqualTo(entry.key), $"English fallback missing for table key '{entry.key}'.");
                Assert.That(table.Resolve(entry.key, "ru"), Is.Not.EqualTo(entry.key), $"Russian fallback missing for table key '{entry.key}'.");
            }
        }

        private static HashSet<string> FindRuntimeLocalizationKeys(string rootFolder)
        {
            var keys = new HashSet<string>();

            foreach (var path in Directory.GetFiles(rootFolder, "*.cs", SearchOption.AllDirectories))
            {
                var content = File.ReadAllText(path);
                foreach (Match match in LocalizationKeyRegex.Matches(content))
                {
                    keys.Add(match.Groups[1].Value);
                }
            }

            return keys;
        }

        private static IReadOnlyList<LocalizationTableAsset.Entry> GetEntries(LocalizationTableAsset table)
        {
            var field = typeof(LocalizationTableAsset).GetField("entries", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, "LocalizationTableAsset.entries field is missing.");

            return field!.GetValue(table) as IReadOnlyList<LocalizationTableAsset.Entry>
                   ?? ((field.GetValue(table) as IEnumerable<LocalizationTableAsset.Entry>)?.ToList()
                       ?? new List<LocalizationTableAsset.Entry>());
        }
    }
}
#endif
