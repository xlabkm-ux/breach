using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TacticalBreach.Tools
{
#if UNITY_EDITOR
    public static class PackageManifestTools
    {
        private static readonly Regex DependencyEntryRegex = new Regex(
            "\"(?<key>[^\"]+)\"\\s*:\\s*\"(?<value>[^\"]*)\"",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static bool TryRemoveDependency(string manifestPath, string packageName)
        {
            if (string.IsNullOrWhiteSpace(manifestPath))
            {
                throw new ArgumentException("Manifest path must be provided.", nameof(manifestPath));
            }

            if (string.IsNullOrWhiteSpace(packageName))
            {
                throw new ArgumentException("Package name must be provided.", nameof(packageName));
            }

            if (!TryLoadManifest(manifestPath, out var manifestText, out var bodyStart, out var bodyEnd))
            {
                return false;
            }

            var entries = ParseDependencyEntries(manifestText.AsSpan(bodyStart, bodyEnd - bodyStart).ToString());
            if (!entries.Remove(packageName))
            {
                return false;
            }

            SaveManifest(manifestPath, entries);
            return true;
        }

        public static bool TryUpsertFileDependency(string manifestPath, string packageName, string relativePackagePath)
        {
            if (string.IsNullOrWhiteSpace(manifestPath))
            {
                throw new ArgumentException("Manifest path must be provided.", nameof(manifestPath));
            }

            if (string.IsNullOrWhiteSpace(packageName))
            {
                throw new ArgumentException("Package name must be provided.", nameof(packageName));
            }

            if (string.IsNullOrWhiteSpace(relativePackagePath))
            {
                throw new ArgumentException("Package path must be provided.", nameof(relativePackagePath));
            }

            if (!TryLoadManifest(manifestPath, out var manifestText, out var bodyStart, out var bodyEnd))
            {
                return false;
            }

            var entries = ParseDependencyEntries(manifestText.AsSpan(bodyStart, bodyEnd - bodyStart).ToString());
            entries[packageName] = $"file:{NormalizePath(relativePackagePath)}";
            SaveManifest(manifestPath, entries);
            return true;
        }

        public static bool TryReadDependency(string manifestPath, string packageName, out string dependencyValue)
        {
            dependencyValue = string.Empty;

            if (string.IsNullOrWhiteSpace(manifestPath))
            {
                throw new ArgumentException("Manifest path must be provided.", nameof(manifestPath));
            }

            if (string.IsNullOrWhiteSpace(packageName))
            {
                throw new ArgumentException("Package name must be provided.", nameof(packageName));
            }

            if (!TryLoadManifest(manifestPath, out var manifestText, out var bodyStart, out var bodyEnd))
            {
                return false;
            }

            var entries = ParseDependencyEntries(manifestText.AsSpan(bodyStart, bodyEnd - bodyStart).ToString());
            if (!entries.TryGetValue(packageName, out var value))
            {
                return false;
            }

            dependencyValue = value;
            return true;
        }

        private static bool TryLoadManifest(string manifestPath, out string manifestText, out int bodyStart, out int bodyEnd)
        {
            manifestText = string.Empty;
            bodyStart = -1;
            bodyEnd = -1;

            if (!File.Exists(manifestPath))
            {
                return false;
            }

            manifestText = File.ReadAllText(manifestPath);
            var dependenciesIndex = manifestText.IndexOf("\"dependencies\"", StringComparison.Ordinal);
            if (dependenciesIndex < 0)
            {
                return false;
            }

            var openBrace = manifestText.IndexOf('{', dependenciesIndex);
            if (openBrace < 0)
            {
                return false;
            }

            var closeBrace = FindMatchingBrace(manifestText, openBrace);
            if (closeBrace < 0)
            {
                return false;
            }

            bodyStart = openBrace + 1;
            bodyEnd = closeBrace;
            return true;
        }

        private static Dictionary<string, string> ParseDependencyEntries(string bodyText)
        {
            var entries = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (Match match in DependencyEntryRegex.Matches(bodyText))
            {
                entries[match.Groups["key"].Value] = match.Groups["value"].Value;
            }

            return entries;
        }

        private static void SaveManifest(string manifestPath, Dictionary<string, string> entries)
        {
            var builder = new StringBuilder();
            builder.AppendLine("{");
            builder.AppendLine("  \"dependencies\": {");

            var index = 0;
            foreach (var entry in entries)
            {
                builder.Append("    ");
                builder.Append('"');
                builder.Append(entry.Key);
                builder.Append("\": \"");
                builder.Append(entry.Value);
                builder.Append('"');
                index++;
                if (index < entries.Count)
                {
                    builder.Append(',');
                }
                builder.AppendLine();
            }

            builder.AppendLine("  }");
            builder.AppendLine("}");
            File.WriteAllText(manifestPath, builder.ToString());
        }

        private static string NormalizePath(string value) => value.Replace('\\', '/').Trim();

        private static int FindMatchingBrace(string text, int openBraceIndex)
        {
            var depth = 0;
            for (var i = openBraceIndex; i < text.Length; i++)
            {
                if (text[i] == '{')
                {
                    depth++;
                }
                else if (text[i] == '}')
                {
                    depth--;
                    if (depth == 0)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }
    }
#endif
}
