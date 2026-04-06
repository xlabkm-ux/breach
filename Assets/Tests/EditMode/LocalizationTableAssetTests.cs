using Breach.Localization;
using NUnit.Framework;
using UnityEngine;

public sealed class LocalizationTableAssetTests
{
    [Test]
    public void Resolve_UsesLanguageSpecificValue_WhenPresent()
    {
        var table = ScriptableObject.CreateInstance<LocalizationTableAsset>();
        table.SetEntries(new[]
        {
            new LocalizationTableAsset.Entry
            {
                key = "ui.result.success.title",
                enValue = "Mission Success",
                ruValue = "Миссия выполнена"
            }
        });

        Assert.That(table.Resolve("ui.result.success.title", "ru"), Is.EqualTo("Миссия выполнена"));
        Assert.That(table.Resolve("ui.result.success.title", "en"), Is.EqualTo("Mission Success"));

        Object.DestroyImmediate(table);
    }

    [Test]
    public void Resolve_FallsBackToLegacyValue_WhenLanguageSpecificMissing()
    {
        var table = ScriptableObject.CreateInstance<LocalizationTableAsset>();
        table.SetEntries(new[]
        {
            new LocalizationTableAsset.Entry
            {
                key = "ui.warning.friendly_fire_risk",
                value = "Friendly Fire Risk"
            }
        });

        Assert.That(table.Resolve("ui.warning.friendly_fire_risk", "ru"), Is.EqualTo("Friendly Fire Risk"));
        Assert.That(table.Resolve("missing.key", "en"), Is.EqualTo("missing.key"));

        Object.DestroyImmediate(table);
    }
}
