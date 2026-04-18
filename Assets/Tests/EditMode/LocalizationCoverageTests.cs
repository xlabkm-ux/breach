#if UNITY_EDITOR && UNITY_INCLUDE_TESTS && BREACH_ENABLE_EDITMODE_TESTS
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
            var tableKeys = FindTableKeys("Assets/Resources/Localization/DefaultLocalizationTable.asset");
            var codeKeys = FindRuntimeLocalizationKeys("Assets/Scripts");

            Assert.That(codeKeys, Is.Not.Empty, "No runtime localization keys were found to validate.");
            Assert.That(tableKeys, Is.Not.Empty, "Default localization table asset was not found or contains no keys.");

            var missingKeys = codeKeys.Where(key => !tableKeys.Contains(key)).OrderBy(key => key).ToArray();
            Assert.That(missingKeys, Is.Empty, $"Missing localization entries: {string.Join(", ", missingKeys)}");
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

        private static HashSet<string> FindTableKeys(string assetPath)
        {
            var keys = new HashSet<string>();
            foreach (var line in File.ReadAllLines(assetPath))
            {
                var trimmed = line.TrimStart();
                if (!trimmed.StartsWith("- key: ", System.StringComparison.Ordinal))
                {
                    continue;
                }

                var key = trimmed.Substring("- key: ".Length).Trim();
                if (!string.IsNullOrWhiteSpace(key))
                {
                    keys.Add(key);
                }
            }

            return keys;
        }
    }
}
#endif
