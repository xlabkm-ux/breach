using UnityEditor;
using UnityEngine;

namespace TacticalBreach.Editor
{
    public static class AssetDatabaseFixer
    {
        [MenuItem("TacticalBreach/Tools/Force Reimport Localization")]
        public static void ForceReimport()
        {
            string path = "Assets/Resources/Localization/DefaultLocalizationTable.asset";
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
            Debug.Log($"[Fixer] Force reimported: {path}");
        }

        public static void ReimportAll()
        {
            AssetDatabase.Refresh();
            Debug.Log("[Fixer] AssetDatabase Refreshed.");
        }
    }
}
