using System.Collections.Generic;
using Breach.Data.AI;
using Breach.Data.Combat;
using Breach.Data.Localization;
using Breach.Data.Mission;
using Breach.Localization;
using Breach.Mission;
using Breach.Save;
using Breach.Squad;
using Breach.UI;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Breach.Editor
{
    public static class ProjectBootstrapAssetsGenerator
    {
        [InitializeOnLoadMethod]
        private static void EnsureProjectAssets()
        {
            EnsureFolder("Assets/Data");
            EnsureFolder("Assets/Data/Mission");
            EnsureFolder("Assets/Data/Combat");
            EnsureFolder("Assets/Data/AI");
            EnsureFolder("Assets/Data/Localization");
            EnsureFolder("Assets/Data/Save");
            EnsureFolder("Assets/Resources");
            EnsureFolder("Assets/Resources/Localization");
            EnsureFolder("Assets/Prefabs");
            EnsureFolder("Assets/Prefabs/Gameplay");
            EnsureFolder("Assets/Prefabs/Gameplay/Zones");
            EnsureFolder("Assets/Prefabs/Gameplay/Mission");
            EnsureFolder("Assets/VisualScripting");
            EnsureFolder("Assets/VisualScripting/Mission");
            EnsureFolder("Assets/VisualScripting/Squad");
            EnsureFolder("Assets/VisualScripting/AI");
            EnsureFolder("Assets/VisualScripting/Hostage");
            EnsureFolder("Assets/VisualScripting/UI");

            var localizationTable = EnsureAsset<LocalizationTableAsset>("Assets/Resources/Localization/DefaultLocalizationTable.asset");
            EnsureLocalizationDefaults(localizationTable);

            EnsureAsset<MissionConfig>("Assets/Data/Mission/MissionConfig_VS01.asset");
            EnsureAsset<WeaponConfig>("Assets/Data/Combat/WeaponConfig_Rifle.asset");
            EnsureAsset<DamageRulesConfig>("Assets/Data/Combat/DamageRules.asset");
            EnsureAsset<PerceptionConfig>("Assets/Data/AI/PerceptionConfig.asset");
            EnsureAsset<SaveSchemaConfig>("Assets/Data/Save/SaveSchemaConfig.asset");

            var refs = EnsureAsset<LocalizationTableRefs>("Assets/Data/Localization/LocalizationTableRefs.asset");
            if (refs.defaultTable == null)
            {
                refs.defaultTable = localizationTable;
                EditorUtility.SetDirty(refs);
            }

            EnsureExtractionZonePrefab();
            EnsureMissionDirectorPrefab();
            EnsureGraphPlaceholder("Assets/VisualScripting/Mission/MissionFlow.asset");
            EnsureGraphPlaceholder("Assets/VisualScripting/Squad/SquadCommandFlow.asset");
            EnsureGraphPlaceholder("Assets/VisualScripting/AI/EnemyAlertFlow.asset");
            EnsureGraphPlaceholder("Assets/VisualScripting/Hostage/HostageFlow.asset");
            EnsureGraphPlaceholder("Assets/VisualScripting/UI/ResultScreenFlow.asset");

            AssetDatabase.SaveAssets();
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var parent = System.IO.Path.GetDirectoryName(path)?.Replace('\\', '/');
            var name = System.IO.Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent) && AssetDatabase.IsValidFolder(parent))
            {
                AssetDatabase.CreateFolder(parent, name);
            }
        }

        private static T EnsureAsset<T>(string path) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null)
            {
                return existing;
            }

            var created = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(created, path);
            return created;
        }

        private static void EnsureLocalizationDefaults(LocalizationTableAsset table)
        {
            if (table == null)
            {
                return;
            }

            var entries = new List<LocalizationTableAsset.Entry>
            {
                new LocalizationTableAsset.Entry { key = "ui.result.success.title", value = "Mission Success" },
                new LocalizationTableAsset.Entry { key = "ui.result.success.body", value = "Hostage secured and extracted." },
                new LocalizationTableAsset.Entry { key = "ui.result.fail.title", value = "Mission Failed" },
                new LocalizationTableAsset.Entry { key = "ui.result.fail.body", value = "Hostage or squad lost." },
                new LocalizationTableAsset.Entry { key = "ui.result.hint.restart", value = "Press Play again to restart the mission." },
                new LocalizationTableAsset.Entry { key = "ui.warning.friendly_fire_risk", value = "Friendly Fire Risk" }
            };
            table.SetEntries(entries);
            EditorUtility.SetDirty(table);
        }

        private static void EnsureExtractionZonePrefab()
        {
            const string path = "Assets/Prefabs/Gameplay/Zones/ExtractionZone.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
            {
                return;
            }

            var go = new GameObject("ExtractionZone");
            go.AddComponent<ExtractionZoneTrigger>();
            var circle = go.AddComponent<CircleCollider2D>();
            circle.isTrigger = true;
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static void EnsureMissionDirectorPrefab()
        {
            const string path = "Assets/Prefabs/Gameplay/Mission/MissionDirector.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
            {
                return;
            }

            var go = new GameObject("MissionDirector");
            go.AddComponent<ActiveOperativeSwitchService>();
            go.AddComponent<SquadCommandService>();
            go.AddComponent<ObjectiveService>();
            go.AddComponent<MissionStateService>();
            go.AddComponent<MissionResultResolver>();
            go.AddComponent<SaveService>();
            go.AddComponent<ResultScreenController>();
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static void EnsureGraphPlaceholder(string path)
        {
            if (AssetDatabase.LoadAssetAtPath<ScriptGraphAsset>(path) != null)
            {
                return;
            }

            var graph = ScriptableObject.CreateInstance<ScriptGraphAsset>();
            AssetDatabase.CreateAsset(graph, path);
        }
    }
}
