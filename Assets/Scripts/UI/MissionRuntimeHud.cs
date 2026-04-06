using Breach.Mission;
using Breach.Squad;
using Breach.Localization;
using UnityEngine;

namespace Breach.UI
{
    public sealed class MissionRuntimeHud : MonoBehaviour
    {
        [SerializeField] private bool showDebugHud = false;

        private GUIStyle labelStyle;
        private MissionStateService missionStateService;
        private ObjectiveService objectiveService;
        private ActiveOperativeSwitchService switchService;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureRuntimeHud()
        {
            if (FindFirstObjectByType<MissionRuntimeHud>() != null)
            {
                return;
            }

            var hudObject = new GameObject("MissionRuntimeHud_Runtime");
            hudObject.AddComponent<MissionRuntimeHud>();
            DontDestroyOnLoad(hudObject);
        }

        private void Update()
        {
            if (Breach.Core.InputCompat.GetKeyDown(KeyCode.F1))
            {
                showDebugHud = !showDebugHud;
            }

            if (missionStateService == null)
            {
                missionStateService = FindFirstObjectByType<MissionStateService>();
            }
            if (objectiveService == null)
            {
                objectiveService = FindFirstObjectByType<ObjectiveService>();
            }
            if (switchService == null)
            {
                switchService = FindFirstObjectByType<ActiveOperativeSwitchService>();
            }
        }

        private void OnGUI()
        {
            if (!showDebugHud)
            {
                return;
            }

            EnsureStyle();

            var x = 16f;
            var y = 16f;
            GUI.Label(new Rect(x, y, 900f, 24f),
                LocalizationService.ResolveFormat(
                    "hud.scene",
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name),
                labelStyle);
            y += 22f;

            var state = missionStateService != null ? missionStateService.CurrentState.ToString() : "missing_mission_state_service";
            GUI.Label(new Rect(x, y, 900f, 24f), LocalizationService.ResolveFormat("hud.mission_state", state), labelStyle);
            y += 22f;

            if (objectiveService != null)
            {
                GUI.Label(
                    new Rect(x, y, 900f, 24f),
                    LocalizationService.ResolveFormat(
                        "hud.objectives",
                        objectiveService.InfiltrationComplete,
                        objectiveService.HostageFreed,
                        objectiveService.HostageExtracted),
                    labelStyle);
                y += 22f;
                GUI.Label(
                    new Rect(x, y, 900f, 24f),
                    LocalizationService.ResolveFormat("hud.fail_flags", objectiveService.SquadAlive, objectiveService.HostageAlive),
                    labelStyle);
                y += 22f;
            }

            var activeOperativeId = switchService != null && switchService.ActiveOperative != null
                ? switchService.ActiveOperative.OperativeId
                : "none";
            GUI.Label(new Rect(x, y, 900f, 24f), LocalizationService.ResolveFormat("hud.active_operative", activeOperativeId), labelStyle);
            y += 28f;

            GUI.Label(new Rect(x, y, 1100f, 24f), LocalizationService.Resolve("hud.controls"), labelStyle);
        }

        private void EnsureStyle()
        {
            if (labelStyle != null)
            {
                return;
            }

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            labelStyle.normal.textColor = new Color(0.9f, 0.95f, 1f);
        }
    }
}
