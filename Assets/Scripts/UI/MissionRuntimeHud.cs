using Breach.Mission;
using Breach.Squad;
using Breach.Localization;
using UnityEngine;

namespace Breach.UI
{
    public sealed class MissionRuntimeHud : MonoBehaviour
    {
        [SerializeField] private bool showDebugHud = false;
        private const float MinHudLineWidth = 320f;
        private const float MaxHudLineWidth = 1100f;

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
            var lineWidth = GetHudLineWidth(Screen.width);
            var controlWidth = GetControlLineWidth(Screen.width);

            y = DrawLine(x, y, lineWidth,
                LocalizationService.ResolveFormat(
                    "hud.scene",
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name));

            var state = missionStateService != null ? missionStateService.CurrentState.ToString() : "missing_mission_state_service";
            y = DrawLine(x, y, lineWidth, LocalizationService.ResolveFormat("hud.mission_state", state));

            if (objectiveService != null)
            {
                y = DrawLine(
                    x,
                    y,
                    lineWidth,
                    LocalizationService.ResolveFormat(
                        "hud.objectives",
                        objectiveService.InfiltrationComplete,
                        objectiveService.HostageFreed,
                        objectiveService.HostageExtracted));
                y = DrawLine(
                    x,
                    y,
                    lineWidth,
                    LocalizationService.ResolveFormat("hud.fail_flags", objectiveService.SquadAlive, objectiveService.HostageAlive));
            }

            var activeOperativeId = switchService != null && switchService.ActiveOperative != null
                ? switchService.ActiveOperative.OperativeId
                : "none";
            y = DrawLine(x, y, lineWidth, LocalizationService.ResolveFormat("hud.active_operative", activeOperativeId));

            DrawLine(x, y, controlWidth, LocalizationService.Resolve("hud.controls"));
        }

        public static float GetHudLineWidth(float screenWidth)
        {
            return Mathf.Clamp(screenWidth - 32f, MinHudLineWidth, MaxHudLineWidth);
        }

        public static float GetControlLineWidth(float screenWidth)
        {
            return Mathf.Clamp(screenWidth - 32f, MinHudLineWidth, MaxHudLineWidth);
        }

        private float DrawLine(float x, float y, float width, string text)
        {
            var content = new GUIContent(text);
            var height = Mathf.Max(24f, labelStyle.CalcHeight(content, width));
            GUI.Label(new Rect(x, y, width, height), content, labelStyle);
            return y + height + 4f;
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
                fontStyle = FontStyle.Bold,
                wordWrap = true
            };
            labelStyle.normal.textColor = new Color(0.9f, 0.95f, 1f);
        }
    }
}
