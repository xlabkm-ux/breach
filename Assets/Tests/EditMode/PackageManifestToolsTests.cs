#if UNITY_EDITOR && UNITY_INCLUDE_TESTS && BREACH_ENABLE_EDITMODE_TESTS
using System;
using System.IO;
using Breach.Tools;
using NUnit.Framework;

namespace Breach.Tests.EditMode
{
    public sealed class PackageManifestToolsTests
    {
        [Test]
        public void TryRemoveDependency_RemovesPackageEntry()
        {
            var manifestPath = CreateTempManifest(@"{""dependencies"":{""com.example.package"":""1.2.3"",""com.example.other"":""4.5.6""}}");

            try
            {
                var removed = PackageManifestTools.TryRemoveDependency(manifestPath, "com.example.package");

                Assert.That(removed, Is.True);
                Assert.That(File.ReadAllText(manifestPath), Does.Not.Contain("com.example.package"));
                Assert.That(File.ReadAllText(manifestPath), Does.Contain("com.example.other"));
            }
            finally
            {
                SafeDelete(manifestPath);
            }
        }

        [Test]
        public void TryUpsertFileDependency_WritesFileReference()
        {
            var manifestPath = CreateTempManifest(@"{""dependencies"":{}}");

            try
            {
                var upserted = PackageManifestTools.TryUpsertFileDependency(manifestPath, "com.example.package", @"..\local\package");

                Assert.That(upserted, Is.True);
                Assert.That(File.ReadAllText(manifestPath), Does.Contain(@"""com.example.package"": ""file:../local/package"""));
            }
            finally
            {
                SafeDelete(manifestPath);
            }
        }

        [Test]
        public void TryReadDependency_ReturnsStoredValue()
        {
            var manifestPath = CreateTempManifest(@"{""dependencies"":{""com.example.package"":""file:../local/package""}}");

            try
            {
                var found = PackageManifestTools.TryReadDependency(manifestPath, "com.example.package", out var value);

                Assert.That(found, Is.True);
                Assert.That(value, Is.EqualTo("file:../local/package"));
            }
            finally
            {
                SafeDelete(manifestPath);
            }
        }

        private static string CreateTempManifest(string json)
        {
            var path = Path.Combine(Path.GetTempPath(), $"manifest-{Guid.NewGuid():N}.json");
            File.WriteAllText(path, json);
            return path;
        }

        private static void SafeDelete(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
#endif
