#if UNITY_EDITOR && UNITY_INCLUDE_TESTS && BREACH_ENABLE_EDITMODE_TESTS
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Breach.AI;
using Breach.Combat;
using Breach.Core;
using Breach.Localization;
using Breach.Hostage;
using Breach.Mission;
using Breach.Squad;
using Breach.Save;
using Breach.UI;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if UNITY_VISUAL_SCRIPTING
using Unity.VisualScripting;
#endif

namespace Breach.Tests.EditMode
{
    public sealed class BreachVerificationTests
    {
        [Test]
        public void SceneAndPrefabAssets_ShouldNotContainMissingReferences()
        {
            var originalScenePath = SceneManager.GetActiveScene().path;

            try
            {
                foreach (var scenePath in FindAssetPaths("t:SceneAsset", "Assets/Scenes"))
                {
                    AssertAssetHasNoMissingReferences(scenePath, LoadSceneAndInspect);
                }

                foreach (var prefabPath in FindAssetPaths("t:Prefab", "Assets/Prefabs"))
                {
                    AssertAssetHasNoMissingReferences(prefabPath, InspectPrefabContents);
                }
            }
            finally
            {
                RestoreScene(originalScenePath);
            }
        }

        [Test]
        public void SaveService_ShouldAutosaveAndRecoverFromBackup()
        {
            var root = new GameObject("SaveServiceVerification");
            var savePath = Path.Combine(Application.persistentDataPath, "mission_save_v1.json");
            var backupPath = Path.Combine(Application.persistentDataPath, "mission_save_v1.backup.json");
            var tempPath = Path.Combine(Application.persistentDataPath, "mission_save_v1.tmp");
            var hadPrimary = File.Exists(savePath);
            var hadBackup = File.Exists(backupPath);
            var originalPrimary = hadPrimary ? File.ReadAllText(savePath) : null;
            var originalBackup = hadBackup ? File.ReadAllText(backupPath) : null;

            try
            {
                var missionState = root.AddComponent<MissionStateService>();
                var objectives = root.AddComponent<ObjectiveService>();
                var saveService = root.AddComponent<SaveService>();

                SetPrivateField(saveService, "schemaVersion", 1);
                SetPrivateField(saveService, "missionStateService", missionState);
                SetPrivateField(saveService, "objectiveService", objectives);
                InvokePrivate(saveService, "OnEnable");

                objectives.MarkInfiltrationComplete();
                objectives.MarkHostageFreed();
                objectives.MarkHostageExtracted();

                Assert.IsTrue(missionState.TryTransition(MissionState.Infiltration, out _));
                Assert.That(File.Exists(savePath), Is.True, "Primary save was not written after mission state transition.");
                Assert.That(File.Exists(backupPath), Is.True, "Backup save was not written alongside the primary save.");

                File.WriteAllText(savePath, "{ broken json");

                Assert.IsTrue(saveService.TryLoad(), "SaveService should recover from a broken primary save by using the backup.");
                Assert.AreEqual(MissionState.Infiltration, missionState.CurrentState);
                Assert.IsTrue(objectives.InfiltrationComplete);
                Assert.IsTrue(objectives.HostageFreed);
                Assert.IsTrue(objectives.HostageExtracted);
                Assert.That(File.ReadAllText(savePath), Does.Contain("\"schemaVersion\""), "Primary save was not restored from backup.");
            }
            finally
            {
                InvokePrivate(root.GetComponent<SaveService>(), "OnDisable");
                Object.DestroyImmediate(root);
                RestoreFile(savePath, hadPrimary, originalPrimary);
                RestoreFile(backupPath, hadBackup, originalBackup);
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }

        [Test]
        public void SaveService_ShouldLoadCompatibleBackupAfterPrimarySchemaMismatch()
        {
            var root = new GameObject("SaveServiceSchemaFallbackVerification");
            var savePath = Path.Combine(Application.persistentDataPath, "mission_save_v1.json");
            var backupPath = Path.Combine(Application.persistentDataPath, "mission_save_v1.backup.json");
            var tempPath = Path.Combine(Application.persistentDataPath, "mission_save_v1.tmp");
            var hadPrimary = File.Exists(savePath);
            var hadBackup = File.Exists(backupPath);
            var originalPrimary = hadPrimary ? File.ReadAllText(savePath) : null;
            var originalBackup = hadBackup ? File.ReadAllText(backupPath) : null;

            try
            {
                var missionState = root.AddComponent<MissionStateService>();
                var objectives = root.AddComponent<ObjectiveService>();
                var saveService = root.AddComponent<SaveService>();

                SetPrivateField(saveService, "schemaVersion", 1);
                SetPrivateField(saveService, "missionStateService", missionState);
                SetPrivateField(saveService, "objectiveService", objectives);
                InvokePrivate(saveService, "OnEnable");

                missionState.ForceStateForLoad(MissionState.Failed);
                objectives.ResetForNewMission();

                var incompatiblePrimary = new MissionSaveSnapshot
                {
                    schemaVersion = 2,
                    missionState = MissionState.Failed,
                    infiltrationComplete = false,
                    hostageFreed = false,
                    hostageExtracted = false,
                    squadAlive = false,
                    hostageAlive = false
                };

                var compatibleBackup = new MissionSaveSnapshot
                {
                    schemaVersion = 1,
                    missionState = MissionState.Extraction,
                    infiltrationComplete = true,
                    hostageFreed = true,
                    hostageExtracted = true,
                    squadAlive = true,
                    hostageAlive = true
                };

                File.WriteAllText(savePath, JsonUtility.ToJson(incompatiblePrimary, true));
                File.WriteAllText(backupPath, JsonUtility.ToJson(compatibleBackup, true));

                Assert.IsTrue(saveService.TryLoad(), "SaveService should fall back to the compatible backup when the primary schema is unsupported.");
                Assert.AreEqual(MissionState.Extraction, missionState.CurrentState);
                Assert.IsTrue(objectives.InfiltrationComplete);
                Assert.IsTrue(objectives.HostageFreed);
                Assert.IsTrue(objectives.HostageExtracted);
                Assert.IsTrue(objectives.SquadAlive);
                Assert.IsTrue(objectives.HostageAlive);
                Assert.That(File.ReadAllText(savePath), Does.Contain("\"schemaVersion\": 1"), "Primary save was not restored from the compatible backup.");
            }
            finally
            {
                InvokePrivate(root.GetComponent<SaveService>(), "OnDisable");
                Object.DestroyImmediate(root);
                RestoreFile(savePath, hadPrimary, originalPrimary);
                RestoreFile(backupPath, hadBackup, originalBackup);
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }

        [Test]
        public void SaveService_ShouldPreserveLiveStateWhenPrimaryAndBackupAreInvalid()
        {
            var root = new GameObject("SaveServiceInvalidFallbackVerification");
            var savePath = Path.Combine(Application.persistentDataPath, "mission_save_v1.json");
            var backupPath = Path.Combine(Application.persistentDataPath, "mission_save_v1.backup.json");
            var tempPath = Path.Combine(Application.persistentDataPath, "mission_save_v1.tmp");
            var hadPrimary = File.Exists(savePath);
            var hadBackup = File.Exists(backupPath);
            var originalPrimary = hadPrimary ? File.ReadAllText(savePath) : null;
            var originalBackup = hadBackup ? File.ReadAllText(backupPath) : null;

            try
            {
                var missionState = root.AddComponent<MissionStateService>();
                var objectives = root.AddComponent<ObjectiveService>();
                var saveService = root.AddComponent<SaveService>();

                SetPrivateField(saveService, "schemaVersion", 1);
                SetPrivateField(saveService, "missionStateService", missionState);
                SetPrivateField(saveService, "objectiveService", objectives);
                InvokePrivate(saveService, "OnEnable");

                missionState.ForceStateForLoad(MissionState.HostageSecured);
                objectives.MarkInfiltrationComplete();
                objectives.MarkHostageFreed();

                File.WriteAllText(savePath, "{ broken json");
                File.WriteAllText(backupPath, JsonUtility.ToJson(new MissionSaveSnapshot
                {
                    schemaVersion = 2,
                    missionState = MissionState.Success,
                    infiltrationComplete = true,
                    hostageFreed = true,
                    hostageExtracted = true,
                    squadAlive = false,
                    hostageAlive = true
                }, true));

                Assert.IsFalse(saveService.TryLoad(), "SaveService should fail cleanly when both the primary and backup saves are unusable.");
                Assert.AreEqual(MissionState.HostageSecured, missionState.CurrentState);
                Assert.IsTrue(objectives.InfiltrationComplete);
                Assert.IsTrue(objectives.HostageFreed);
                Assert.IsFalse(objectives.HostageExtracted);
                Assert.IsTrue(objectives.SquadAlive);
                Assert.IsTrue(objectives.HostageAlive);
            }
            finally
            {
                InvokePrivate(root.GetComponent<SaveService>(), "OnDisable");
                Object.DestroyImmediate(root);
                RestoreFile(savePath, hadPrimary, originalPrimary);
                RestoreFile(backupPath, hadBackup, originalBackup);
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }

        [Test]
        public void LocalizationTable_ShouldFallbackToEnglishAndKey()
        {
            var table = ScriptableObject.CreateInstance<LocalizationTableAsset>();

            try
            {
                table.SetEntries(new[]
                {
                    new LocalizationTableAsset.Entry
                    {
                        key = "test.fallback",
                        enValue = "English value",
                        ruValue = string.Empty
                    },
                    new LocalizationTableAsset.Entry
                    {
                        key = "test.ru",
                        enValue = "English value",
                        ruValue = "Русское значение"
                    }
                });

                Assert.AreEqual("English value", table.Resolve("test.fallback", "ru"));
                Assert.AreEqual("Русское значение", table.Resolve("test.ru", "ru"));
                Assert.AreEqual("missing.key", table.Resolve("missing.key", "en"));
                Assert.AreEqual(string.Empty, table.Resolve(string.Empty, "en"));
            }
            finally
            {
                Object.DestroyImmediate(table);
            }
        }

        [Test]
        public void DefaultLocalizationTable_ShouldCoverCoreMissionUiKeys()
        {
            var table = Resources.Load<LocalizationTableAsset>("Localization/DefaultLocalizationTable");
            Assert.NotNull(table, "Default localization table resource is missing.");

            var requiredKeys = new[]
            {
                "ui.result.success.title",
                "ui.result.success.body",
                "ui.result.fail.title",
                "ui.result.fail.body",
                "ui.result.hint.restart",
                "ui.warning.friendly_fire_risk",
                "ui.alert.idle",
                "ui.alert.suspicious",
                "ui.alert.alert",
                "hud.scene",
                "hud.mission_state",
                "hud.objectives",
                "hud.fail_flags",
                "hud.active_operative",
                "hud.controls"
            };

            foreach (var key in requiredKeys)
            {
                var resolved = table.Resolve(key, "en");
                Assert.That(resolved, Is.Not.Null.And.Not.Empty.And.Not.EqualTo(key), $"Localization key '{key}' is missing from the default table.");
            }
        }

        [Test]
        public void SquadCommandEvents_ShouldUseCanonicalEventNames()
        {
            Assert.AreEqual("breach.squad.command.move", SquadCommandVsEvents.MoveIssued);
            Assert.AreEqual("breach.squad.command.hold", SquadCommandVsEvents.HoldIssued);
            Assert.AreEqual("breach.squad.command.follow", SquadCommandVsEvents.FollowIssued);
            Assert.AreEqual("breach.squad.command.attack", SquadCommandVsEvents.AttackIssued);
        }

        [Test]
        public void MissionResultResolver_ShouldResolveSuccessAndFailureChains()
        {
            var successRoot = new GameObject("MissionSuccessVerification");
            var failureRoot = new GameObject("MissionFailureVerification");

            try
            {
                var successState = successRoot.AddComponent<MissionStateService>();
                var successObjectives = successRoot.AddComponent<ObjectiveService>();
                var successResolver = successRoot.AddComponent<MissionResultResolver>();
                InvokePrivate(successResolver, "Awake");

                successObjectives.MarkInfiltrationComplete();
                successObjectives.MarkHostageFreed();
                successObjectives.MarkHostageExtracted();
                InvokePrivate(successResolver, "Update");

                Assert.AreEqual(MissionState.Success, successState.CurrentState);

                var failureState = failureRoot.AddComponent<MissionStateService>();
                var failureObjectives = failureRoot.AddComponent<ObjectiveService>();
                var failureResolver = failureRoot.AddComponent<MissionResultResolver>();
                InvokePrivate(failureResolver, "Awake");

                failureObjectives.MarkSquadWiped();
                InvokePrivate(failureResolver, "Update");

                Assert.AreEqual(MissionState.Failed, failureState.CurrentState);
            }
            finally
            {
                Object.DestroyImmediate(successRoot);
                Object.DestroyImmediate(failureRoot);
            }
        }

        [Test]
        public void MissionRuntimeHud_ShouldMapMissionStatesToLocalizationKeys()
        {
            var method = typeof(MissionRuntimeHud).GetMethod("GetMissionStateKey", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method, "MissionRuntimeHud.GetMissionStateKey is missing.");

            Assert.AreEqual("mission_state.not_started", method.Invoke(null, new object[] { MissionState.NotStarted }));
            Assert.AreEqual("mission_state.infiltration", method.Invoke(null, new object[] { MissionState.Infiltration }));
            Assert.AreEqual("mission_state.engagement", method.Invoke(null, new object[] { MissionState.Engagement }));
            Assert.AreEqual("mission_state.hostage_secured", method.Invoke(null, new object[] { MissionState.HostageSecured }));
            Assert.AreEqual("mission_state.extraction", method.Invoke(null, new object[] { MissionState.Extraction }));
            Assert.AreEqual("mission_state.success", method.Invoke(null, new object[] { MissionState.Success }));
            Assert.AreEqual("mission_state.failed", method.Invoke(null, new object[] { MissionState.Failed }));

            var unknownKey = method.Invoke(null, new object[] { (MissionState)123 });
            Assert.AreEqual("mission_state.unknown", unknownKey);
        }

        [Test]
        public void CombatResolver_ShouldRespectFriendlyFireAndDamageTargets()
        {
            var root = new GameObject("CombatResolverVerification");

            try
            {
                var resolver = root.AddComponent<CombatResolver>();
                var attacker = new GameObject("Attacker").AddComponent<HealthComponent>();
                var target = new GameObject("Target").AddComponent<HealthComponent>();

                attacker.SetTeam(TeamId.Squad);
                target.SetTeam(TeamId.Squad);

                Assert.IsFalse(resolver.TryResolveHit(attacker, target, 25), "Friendly fire should be blocked when disabled.");
                Assert.AreEqual(100, target.CurrentHealth);

                resolver.SetFriendlyFireEnabled(true);
                Assert.IsTrue(resolver.TryResolveHit(attacker, target, 25), "Friendly fire should apply when enabled.");
                Assert.AreEqual(75, target.CurrentHealth);
            }
            finally
            {
                Object.DestroyImmediate(root);
                Object.DestroyImmediate(GameObject.Find("Attacker"));
                Object.DestroyImmediate(GameObject.Find("Target"));
            }
        }

        [Test]
        public void EnemyAlertController_ShouldEscalateAndReactToNoise()
        {
            var enemy = new GameObject("EnemyAlertVerification");
            var target = new GameObject("EnemyAlertTarget");

            try
            {
                enemy.transform.position = Vector3.zero;
                target.transform.position = new Vector3(2f, 0f, 0f);
                target.AddComponent<CircleCollider2D>();
                target.AddComponent<OperativeMember>();

                var vision = enemy.AddComponent<EnemyVisionCone>();
                var controller = enemy.AddComponent<EnemyAlertController>();

                Physics2D.SyncTransforms();
                NoiseEventBus.Raise(new Vector3(0f, 0f, 0f), 8f);
                InvokePrivate(controller, "Update");
                Assert.AreEqual(EnemyAlertState.Suspicious, controller.CurrentState, "Noise inside range should make the enemy suspicious.");

                InvokePrivate(vision, "Update");
                InvokePrivate(controller, "Update");
                Assert.AreEqual(EnemyAlertState.Alert, controller.CurrentState, "Detected target should escalate the enemy to alert.");
            }
            finally
            {
                Object.DestroyImmediate(enemy);
                Object.DestroyImmediate(target);
            }
        }

        [Test]
        public void EnemyVisionCone_ShouldDetectTargetsInForwardArc()
        {
            var enemy = new GameObject("EnemyVisionConeVerification");
            var target = new GameObject("TargetVisionConeVerification");

            try
            {
                enemy.transform.position = Vector3.zero;
                target.transform.position = new Vector3(3f, 0f, 0f);

                target.AddComponent<CircleCollider2D>();
                target.AddComponent<OperativeMember>();

                var vision = enemy.AddComponent<EnemyVisionCone>();
                enemy.AddComponent<CircleCollider2D>();

                Physics2D.SyncTransforms();
                InvokePrivate(vision, "Update");

                Assert.IsTrue(vision.HasTarget, "The target in front of the cone should be detected.");
                Assert.AreEqual(target.GetComponent<OperativeMember>(), vision.DetectedTarget);
            }
            finally
            {
                Object.DestroyImmediate(enemy);
                Object.DestroyImmediate(target);
            }
        }

        [Test]
        public void HostageController_ShouldFreeAndEscortActiveOperative()
        {
            var hostageRoot = new GameObject("HostageVerification");
            var operative = new GameObject("ActiveOperativeVerification");

            try
            {
                hostageRoot.transform.position = Vector3.zero;
                operative.transform.position = new Vector3(1.2f, 0f, 0f);

                var objectiveService = hostageRoot.AddComponent<ObjectiveService>();
                hostageRoot.AddComponent<HealthComponent>();
                var hostage = hostageRoot.AddComponent<HostageController>();

                Assert.IsTrue(hostage.TryFree(operative.transform), "Hostage should free when an operative is within interaction distance.");
                Assert.IsTrue(hostage.IsFreed);
                Assert.IsTrue(objectiveService.HostageFreed);

                var beforeEscort = hostageRoot.transform.position;
                operative.transform.position = new Vector3(5f, 0f, 0f);
                hostage.TickEscort(operative.transform, 1f);

                Assert.Greater(hostageRoot.transform.position.x, beforeEscort.x, "Freed hostage should move toward the escort target.");
            }
            finally
            {
                Object.DestroyImmediate(hostageRoot);
                Object.DestroyImmediate(operative);
            }
        }

        [Test]
        public void HostageFailureFlow_ShouldFailMissionWhenHostageDies()
        {
            var root = new GameObject("HostageFailureVerification");

            try
            {
                var stateService = root.AddComponent<MissionStateService>();
                var objectiveService = root.AddComponent<ObjectiveService>();
                var resolver = root.AddComponent<MissionResultResolver>();

                InvokePrivate(resolver, "Awake");
                objectiveService.MarkHostageKilled();
                InvokePrivate(resolver, "Update");

                Assert.AreEqual(MissionState.Failed, stateService.CurrentState);
                Assert.IsTrue(objectiveService.IsMissionFailed);
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void HostageFailureFlow_ShouldPreferFailureOverLateSuccessCandidate()
        {
            var root = new GameObject("HostageLateFailureVerification");

            try
            {
                var stateService = root.AddComponent<MissionStateService>();
                var objectiveService = root.AddComponent<ObjectiveService>();
                var resolver = root.AddComponent<MissionResultResolver>();

                InvokePrivate(resolver, "Awake");

                objectiveService.MarkInfiltrationComplete();
                objectiveService.MarkHostageFreed();
                objectiveService.MarkHostageExtracted();
                objectiveService.MarkHostageKilled();

                InvokePrivate(resolver, "Update");

                Assert.AreEqual(MissionState.Failed, stateService.CurrentState);
                Assert.IsFalse(stateService.CurrentState == MissionState.Success, "Mission should not resolve to success when the hostage is dead.");
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        [Test]
        public void ExtractionZoneTrigger_ShouldMarkFreedHostageAsExtracted()
        {
            var root = new GameObject("ExtractionVerification");
            var hostage = new GameObject("ExtractionHostage");
            var operative = new GameObject("ExtractionOperative");

            try
            {
                root.transform.position = Vector3.zero;
                hostage.transform.position = new Vector3(0.8f, 0f, 0f);
                operative.transform.position = new Vector3(1f, 0f, 0f);

                var objectiveService = root.AddComponent<ObjectiveService>();
                var zone = root.AddComponent<ExtractionZoneTrigger>();
                hostage.AddComponent<HealthComponent>();
                var hostageController = hostage.AddComponent<HostageController>();

                Assert.IsTrue(hostageController.TryFree(operative.transform));
                Physics2D.SyncTransforms();
                InvokePrivate(zone, "Awake");
                InvokePrivate(zone, "Update");

                Assert.IsTrue(objectiveService.HostageExtracted, "Freed hostage inside the extraction zone should mark extraction complete.");
            }
            finally
            {
                Object.DestroyImmediate(root);
                Object.DestroyImmediate(hostage);
                Object.DestroyImmediate(operative);
            }
        }

        [Test]
        public void ExtractionZoneTrigger_ShouldIgnoreUnfreedHostage()
        {
            var root = new GameObject("ExtractionUnfreedVerification");
            var hostage = new GameObject("ExtractionUnfreedHostage");

            try
            {
                root.transform.position = Vector3.zero;
                hostage.transform.position = new Vector3(0.8f, 0f, 0f);

                var objectiveService = root.AddComponent<ObjectiveService>();
                var zone = root.AddComponent<ExtractionZoneTrigger>();
                hostage.AddComponent<HealthComponent>();
                hostage.AddComponent<HostageController>();

                Physics2D.SyncTransforms();
                InvokePrivate(zone, "Awake");
                InvokePrivate(zone, "Update");

                Assert.IsFalse(objectiveService.HostageExtracted, "A hostage must be freed before extraction can complete.");
            }
            finally
            {
                Object.DestroyImmediate(root);
                Object.DestroyImmediate(hostage);
            }
        }

        [Test]
        public void QualitySettings_ShouldExposeCanonicalPlatformProfiles()
        {
            var names = QualitySettings.names;
            CollectionAssert.Contains(names, "PC_Default");
            CollectionAssert.Contains(names, "Android_Default");
            CollectionAssert.Contains(names, "Android_Low");
        }

        [Test]
        public void VisualQualityProfileRuntime_ShouldMapProfilesToOverlayBudget()
        {
            var runtimeObject = new GameObject("VisualQualityProfileVerification");
            var originalQuality = QualitySettings.GetQualityLevel();

            try
            {
                runtimeObject.AddComponent<VisualQualityProfileRuntime>();

                ApplyQualityProfile("Android_Low");
                VisualQualityProfileRuntime.RefreshFromCurrentQuality();
                Assert.IsFalse(VisualQualityProfileRuntime.EnableVisionConeOverlay);
                Assert.IsFalse(VisualQualityProfileRuntime.EnableNoiseOverlay);
                Assert.IsTrue(VisualQualityProfileRuntime.EnableAimOverlay);
                Assert.AreEqual(0.6f, VisualQualityProfileRuntime.EffectIntensity, 0.0001f);

                ApplyQualityProfile("Android_Default");
                VisualQualityProfileRuntime.RefreshFromCurrentQuality();
                Assert.IsTrue(VisualQualityProfileRuntime.EnableVisionConeOverlay);
                Assert.IsTrue(VisualQualityProfileRuntime.EnableNoiseOverlay);
                Assert.IsTrue(VisualQualityProfileRuntime.EnableAimOverlay);
                Assert.AreEqual(0.8f, VisualQualityProfileRuntime.EffectIntensity, 0.0001f);

                ApplyQualityProfile("PC_Default");
                VisualQualityProfileRuntime.RefreshFromCurrentQuality();
                Assert.IsTrue(VisualQualityProfileRuntime.EnableVisionConeOverlay);
                Assert.IsTrue(VisualQualityProfileRuntime.EnableNoiseOverlay);
                Assert.IsTrue(VisualQualityProfileRuntime.EnableAimOverlay);
                Assert.AreEqual(1f, VisualQualityProfileRuntime.EffectIntensity, 0.0001f);
            }
            finally
            {
                if (originalQuality >= 0 && originalQuality < QualitySettings.names.Length)
                {
                    QualitySettings.SetQualityLevel(originalQuality, true);
                }

                VisualQualityProfileRuntime.RefreshFromCurrentQuality();
                Object.DestroyImmediate(runtimeObject);
            }
        }

        [Test]
        public void MissionHudAndResultScreen_ShouldClampWidthsForMobileReadability()
        {
            Assert.AreEqual(320f, MissionRuntimeHud.GetHudLineWidth(240f));
            Assert.AreEqual(1100f, MissionRuntimeHud.GetHudLineWidth(1920f));
            Assert.AreEqual(320f, MissionRuntimeHud.GetControlLineWidth(240f));
            Assert.AreEqual(1100f, MissionRuntimeHud.GetControlLineWidth(2560f));

            Assert.AreEqual(320f, MissionResultScreenRuntime.GetPanelWidth(320f));
            Assert.AreEqual(620f, MissionResultScreenRuntime.GetPanelWidth(1920f));
        }

        [Test]
        public void AndroidBuildSettings_ShouldStayAlignedWithMobileReleaseSanity()
        {
            Assert.AreEqual(ScriptingImplementation.IL2CPP, PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android));
            Assert.AreEqual(AndroidArchitecture.ARM64, PlayerSettings.Android.targetArchitectures);
        }

        [Test]
        public void InputCompat_ShouldSupportCanonicalGameplayHotkeys()
        {
            CollectionAssert.AreEquivalent(
                new[] { KeyCode.Tab, KeyCode.H, KeyCode.F, KeyCode.M, KeyCode.T, KeyCode.E, KeyCode.F1 },
                new[]
                {
                    KeyCode.Tab,
                    KeyCode.H,
                    KeyCode.F,
                    KeyCode.M,
                    KeyCode.T,
                    KeyCode.E,
                    KeyCode.F1
                }.Where(InputCompat.IsSupportedKeyCode).ToArray());
        }

#if UNITY_VISUAL_SCRIPTING
        [Test]
        public void VisualScriptingGraphs_ShouldContainCanonicalCommandAndAlertBindings()
        {
            var squadFlow = AssetDatabase.LoadAssetAtPath<ScriptGraphAsset>("Assets/VisualScripting/Squad/SquadCommandFlow.asset");
            var enemyFlow = AssetDatabase.LoadAssetAtPath<ScriptGraphAsset>("Assets/VisualScripting/AI/EnemyAlertFlow.asset");
            var missionDirector = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Gameplay/Mission/MissionDirector.prefab");
            var enemyGrunt = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Gameplay/Enemies/Enemy_Grunt.prefab");

            Assert.NotNull(squadFlow, "Squad command graph asset is missing.");
            Assert.NotNull(enemyFlow, "Enemy alert graph asset is missing.");
            Assert.NotNull(missionDirector, "MissionDirector prefab is missing.");
            Assert.NotNull(enemyGrunt, "Enemy_Grunt prefab is missing.");

            AssertGraphContainsRelay(squadFlow.graph, "breach.squad.command.move", "breach.squad.command.move.graph");
            AssertGraphContainsRelay(squadFlow.graph, "breach.squad.command.hold", "breach.squad.command.hold.graph");
            AssertGraphContainsRelay(squadFlow.graph, "breach.squad.command.follow", "breach.squad.command.follow.graph");
            AssertGraphContainsRelay(squadFlow.graph, "breach.squad.command.attack", "breach.squad.command.attack.graph");

            AssertGraphContainsRelay(enemyFlow.graph, EnemyAlertVsEvents.Idle, "breach.enemy.alert.idle.graph");
            AssertGraphContainsRelay(enemyFlow.graph, EnemyAlertVsEvents.Suspicious, "breach.enemy.alert.suspicious.graph");
            AssertGraphContainsRelay(enemyFlow.graph, EnemyAlertVsEvents.Alert, "breach.enemy.alert.alert.graph");

            var missionMachine = missionDirector.GetComponent<ScriptMachine>();
            var enemyMachine = enemyGrunt.GetComponent<ScriptMachine>();

            Assert.NotNull(missionMachine, "MissionDirector prefab is missing its ScriptMachine bridge.");
            Assert.NotNull(enemyMachine, "Enemy_Grunt prefab is missing its ScriptMachine bridge.");
            Assert.AreEqual(squadFlow, missionMachine.nest.macro, "MissionDirector should use SquadCommandFlow for command parity.");
            Assert.AreEqual(enemyFlow, enemyMachine.nest.macro, "Enemy_Grunt should use EnemyAlertFlow for alert parity.");
        }
#endif

        private static void AssertAssetHasNoMissingReferences(string assetPath, Action<string> inspector)
        {
            try
            {
                inspector(assetPath);
            }
            catch (AssertionException exception)
            {
                Assert.Fail($"{assetPath}: {exception.Message}");
            }
        }

        private static IEnumerable<string> FindAssetPaths(string query, params string[] folders)
        {
            return AssetDatabase.FindAssets(query, folders)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .OrderBy(path => path);
        }

        private static void LoadSceneAndInspect(string scenePath)
        {
            var originalScenePath = SceneManager.GetActiveScene().path;

            try
            {
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                InspectSceneHierarchy(scene, scenePath);
            }
            finally
            {
                RestoreScene(originalScenePath);
            }
        }

        private static void InspectPrefabContents(string prefabPath)
        {
            var root = PrefabUtility.LoadPrefabContents(prefabPath);
            try
            {
                InspectGameObjectHierarchy(root, prefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static void InspectSceneHierarchy(Scene scene, string context)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                InspectGameObjectHierarchy(root, context);
            }
        }

        private static void InspectGameObjectHierarchy(GameObject root, string context)
        {
            foreach (var missingScriptRoot in root.GetComponentsInChildren<Transform>(true))
            {
                if (missingScriptRoot == null)
                {
                    continue;
                }

                var gameObject = missingScriptRoot.gameObject;
                var missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObject);
                Assert.AreEqual(0, missingCount, $"{context}: missing script found on '{GetHierarchyPath(gameObject.transform)}'.");

                var components = gameObject.GetComponents<Component>();
                foreach (var component in components)
                {
                    if (component == null)
                    {
                        Assert.Fail($"{context}: null component found on '{GetHierarchyPath(gameObject.transform)}'.");
                    }

                    InspectSerializedReferences(component, context);
                }
            }
        }

        private static void InspectSerializedReferences(Component component, string context)
        {
            var serializedObject = new SerializedObject(component);
            var iterator = serializedObject.GetIterator();
            var enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (iterator.propertyType != SerializedPropertyType.ObjectReference)
                {
                    continue;
                }

                if (iterator.name == "m_Script")
                {
                    continue;
                }

                if (iterator.objectReferenceValue == null && iterator.objectReferenceInstanceIDValue != 0)
                {
                    Assert.Fail($"{context}: missing reference on '{GetHierarchyPath(component.transform)}' component '{component.GetType().Name}' field '{iterator.propertyPath}'.");
                }
            }
        }

        private static string GetHierarchyPath(Transform transform)
        {
            var segments = new Stack<string>();
            var current = transform;
            while (current != null)
            {
                segments.Push(current.name);
                current = current.parent;
            }

            return string.Join("/", segments);
        }

        private static void RestoreScene(string scenePath)
        {
            if (string.IsNullOrWhiteSpace(scenePath))
            {
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) == null)
            {
                return;
            }

            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }

        private static void RestoreFile(string path, bool existedBefore, string originalContents)
        {
            if (existedBefore)
            {
                File.WriteAllText(path, originalContents ?? string.Empty);
                return;
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private static void InvokePrivate(object target, string methodName)
        {
            if (target == null)
            {
                return;
            }

            var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (method == null)
            {
                Assert.Fail($"Method '{methodName}' was not found on '{target.GetType().Name}'.");
            }

            method.Invoke(target, null);
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                Assert.Fail($"Field '{fieldName}' was not found on '{target.GetType().Name}'.");
            }

            field.SetValue(target, value);
        }

        private static void ApplyQualityProfile(string profileName)
        {
            var names = QualitySettings.names;
            var index = Array.FindIndex(names, candidate => string.Equals(candidate, profileName, StringComparison.Ordinal));
            Assert.That(index, Is.GreaterThanOrEqualTo(0), $"Quality profile '{profileName}' was not found.");
            QualitySettings.SetQualityLevel(index, true);
        }

#if UNITY_VISUAL_SCRIPTING
        private static void AssertGraphContainsRelay(FlowGraph graph, string listenEvent, string relayEvent)
        {
            Assert.NotNull(graph, "Expected a valid Visual Scripting graph.");

            var hasRelay = false;
            foreach (var unit in graph.units)
            {
                if (unit is not CustomEvent listener)
                {
                    continue;
                }

                if (!listener.defaultValues.TryGetValue(listener.name.key, out var listenValue))
                {
                    continue;
                }

                if (!string.Equals(listenValue?.ToString(), listenEvent, StringComparison.Ordinal))
                {
                    continue;
                }

                foreach (var output in listener.trigger.validConnectedPorts)
                {
                    if (output is not ControlInput input || input.unit is not TriggerCustomEvent trigger)
                    {
                        continue;
                    }

                    if (!trigger.defaultValues.TryGetValue(trigger.name.key, out var triggerValue))
                    {
                        continue;
                    }

                    if (string.Equals(triggerValue?.ToString(), relayEvent, StringComparison.Ordinal))
                    {
                        hasRelay = true;
                        break;
                    }
                }

                if (hasRelay)
                {
                    break;
                }
            }

            Assert.IsTrue(hasRelay, $"Graph is missing relay '{listenEvent}' -> '{relayEvent}'.");
        }
#endif
    }
}
#endif

