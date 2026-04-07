using System.Collections.Generic;
using System.Linq;
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
            var squadFlow = EnsureAsset<ScriptGraphAsset>("Assets/VisualScripting/Squad/SquadCommandFlow.asset");
            var enemyFlow = EnsureAsset<ScriptGraphAsset>("Assets/VisualScripting/AI/EnemyAlertFlow.asset");
            EnsureGraphPlaceholder("Assets/VisualScripting/Mission/MissionFlow.asset");
            EnsureGraphPlaceholder("Assets/VisualScripting/Hostage/HostageFlow.asset");
            EnsureGraphPlaceholder("Assets/VisualScripting/UI/ResultScreenFlow.asset");

            EnsureSquadCommandFlow(squadFlow);
            EnsureEnemyAlertFlow(enemyFlow);
            EnsurePrefabScriptMachine("Assets/Prefabs/Gameplay/Mission/MissionDirector.prefab", squadFlow);
            EnsurePrefabScriptMachine("Assets/Prefabs/Gameplay/Enemies/Enemy_Grunt.prefab", enemyFlow);

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
                new LocalizationTableAsset.Entry { key = "ui.result.success.title", enValue = "Mission Success", ruValue = "Миссия выполнена" },
                new LocalizationTableAsset.Entry { key = "ui.result.success.body", enValue = "Hostage secured and extracted.", ruValue = "Заложник освобожден и эвакуирован." },
                new LocalizationTableAsset.Entry { key = "ui.result.fail.title", enValue = "Mission Failed", ruValue = "Миссия провалена" },
                new LocalizationTableAsset.Entry { key = "ui.result.fail.body", enValue = "Hostage or squad lost.", ruValue = "Потерян заложник или весь отряд." },
                new LocalizationTableAsset.Entry { key = "ui.result.hint.restart", enValue = "Press Play again to restart the mission.", ruValue = "Нажмите Play заново, чтобы перезапустить миссию." },
                new LocalizationTableAsset.Entry { key = "ui.warning.friendly_fire_risk", enValue = "Friendly Fire Risk", ruValue = "Риск огня по своим" },
                new LocalizationTableAsset.Entry { key = "ui.alert.idle", enValue = "Idle", ruValue = "Спокоен" },
                new LocalizationTableAsset.Entry { key = "ui.alert.suspicious", enValue = "Suspicious", ruValue = "Подозрение" },
                new LocalizationTableAsset.Entry { key = "ui.alert.alert", enValue = "Alert", ruValue = "Тревога" },
                new LocalizationTableAsset.Entry { key = "hud.scene", enValue = "scene: {0}", ruValue = "сцена: {0}" },
                new LocalizationTableAsset.Entry { key = "hud.mission_state", enValue = "mission_state: {0}", ruValue = "состояние_миссии: {0}" },
                new LocalizationTableAsset.Entry { key = "hud.objectives", enValue = "obj_infiltration: {0} | obj_hostage_freed: {1} | obj_hostage_extracted: {2}", ruValue = "цели_инфильтрация: {0} | цели_заложник_освобожден: {1} | цели_заложник_эвакуирован: {2}" },
                new LocalizationTableAsset.Entry { key = "hud.fail_flags", enValue = "fail_squad_alive: {0} | fail_hostage_alive: {1}", ruValue = "провал_отряд_жив: {0} | провал_заложник_жив: {1}" },
                new LocalizationTableAsset.Entry { key = "hud.active_operative", enValue = "active_operative: {0}", ruValue = "активный_оперативник: {0}" },
                new LocalizationTableAsset.Entry { key = "hud.controls", enValue = "controls: WASD move | Tab switch | E free hostage | LMB shoot | RMB aim preview | F1 hide hud", ruValue = "управление: WASD движение | Tab переключение | E освободить заложника | ЛКМ выстрел | ПКМ линия прицела | F1 скрыть hud" }
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

        private static void EnsureSquadCommandFlow(ScriptGraphAsset flow)
        {
            if (flow == null)
            {
                return;
            }

            var relays = new[]
            {
                ("breach.squad.command.move", "breach.squad.command.move.graph"),
                ("breach.squad.command.hold", "breach.squad.command.hold.graph"),
                ("breach.squad.command.follow", "breach.squad.command.follow.graph"),
                ("breach.squad.command.attack", "breach.squad.command.attack.graph")
            };

            if (HasAllEventRelays(flow.graph, relays))
            {
                return;
            }

            flow.graph = BuildRelayGraph(relays);
            EditorUtility.SetDirty(flow);
        }

        private static void EnsureEnemyAlertFlow(ScriptGraphAsset flow)
        {
            if (flow == null)
            {
                return;
            }

            var relays = new[]
            {
                (EnemyAlertVsEvents.Idle, "breach.enemy.alert.idle.graph"),
                (EnemyAlertVsEvents.Suspicious, "breach.enemy.alert.suspicious.graph"),
                (EnemyAlertVsEvents.Alert, "breach.enemy.alert.alert.graph")
            };

            if (HasAllEventRelays(flow.graph, relays))
            {
                return;
            }

            flow.graph = BuildRelayGraph(relays);
            EditorUtility.SetDirty(flow);
        }

        private static FlowGraph BuildRelayGraph(IEnumerable<(string listenEvent, string relayEvent)> relays)
        {
            var graph = new FlowGraph();
            var offset = 0f;

            foreach (var relay in relays)
            {
                AddEventRelay(graph, relay.listenEvent, relay.relayEvent, new Vector2(-420f, offset), new Vector2(-90f, offset));
                offset -= 140f;
            }

            return graph;
        }

        private static void AddEventRelay(FlowGraph graph, string listenEvent, string relayEvent, Vector2 listenPosition, Vector2 relayPosition)
        {
            var listener = new CustomEvent
            {
                argumentCount = 0,
                position = listenPosition
            };

            var trigger = new TriggerCustomEvent
            {
                argumentCount = 0,
                position = relayPosition
            };

            graph.units.Add(listener);
            graph.units.Add(trigger);

            listener.name.SetDefaultValue(listenEvent);
            trigger.name.SetDefaultValue(relayEvent);
            listener.trigger.ValidlyConnectTo(trigger.enter);
        }

        private static bool HasAllEventRelays(FlowGraph graph, IEnumerable<(string listenEvent, string relayEvent)> relays)
        {
            return relays.All(relay => HasEventRelay(graph, relay.listenEvent, relay.relayEvent));
        }

        private static bool HasEventRelay(FlowGraph graph, string listenEvent, string relayEvent)
        {
            if (graph == null)
            {
                return false;
            }

            foreach (var unit in graph.units)
            {
                if (unit is not CustomEvent customEvent)
                {
                    continue;
                }

                if (!customEvent.defaultValues.TryGetValue(customEvent.name.key, out var listenValue))
                {
                    continue;
                }

                if (!string.Equals(listenValue?.ToString(), listenEvent, System.StringComparison.Ordinal))
                {
                    continue;
                }

                var listenerOutput = customEvent.trigger;
                foreach (var connected in listenerOutput.validConnectedPorts)
                {
                    if (connected is not ControlInput controlInput)
                    {
                        continue;
                    }

                    if (controlInput.unit is not TriggerCustomEvent trigger)
                    {
                        continue;
                    }

                    if (!trigger.defaultValues.TryGetValue(trigger.name.key, out var triggerValue))
                    {
                        continue;
                    }

                    var triggerName = triggerValue?.ToString();
                    if (string.Equals(triggerName, relayEvent, System.StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void EnsurePrefabScriptMachine(string prefabPath, ScriptGraphAsset graphAsset)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                return;
            }

            var root = PrefabUtility.LoadPrefabContents(prefabPath);
            try
            {
                var variables = root.GetComponent<Variables>();
                if (variables == null)
                {
                    variables = root.AddComponent<Variables>();
                }

                var machine = root.GetComponent<ScriptMachine>();
                if (machine == null)
                {
                    machine = root.AddComponent<ScriptMachine>();
                }

                if (machine.nest.macro != graphAsset)
                {
                    machine.nest.SwitchToMacro(graphAsset);
                }

                EditorUtility.SetDirty(root);
                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
    }
}
