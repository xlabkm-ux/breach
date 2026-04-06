using Breach.Mission;
using Breach.Squad;
using UnityEngine;

namespace Breach.UI
{
    public sealed class MissionRuntimeHud : MonoBehaviour
    {
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
            EnsureStyle();

            var x = 16f;
            var y = 16f;
            GUI.Label(new Rect(x, y, 760f, 24f), $"scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}", labelStyle);
            y += 22f;

            var state = missionStateService != null ? missionStateService.CurrentState.ToString() : "missing_mission_state_service";
            GUI.Label(new Rect(x, y, 760f, 24f), $"mission_state: {state}", labelStyle);
            y += 22f;

            if (objectiveService != null)
            {
                GUI.Label(new Rect(x, y, 760f, 24f), $"obj_infiltration: {objectiveService.InfiltrationComplete} | obj_hostage_freed: {objectiveService.HostageFreed} | obj_hostage_extracted: {objectiveService.HostageExtracted}", labelStyle);
                y += 22f;
                GUI.Label(new Rect(x, y, 760f, 24f), $"fail_squad_alive: {objectiveService.SquadAlive} | fail_hostage_alive: {objectiveService.HostageAlive}", labelStyle);
                y += 22f;
            }

            var activeOperativeId = switchService != null && switchService.ActiveOperative != null
                ? switchService.ActiveOperative.OperativeId
                : "none";
            GUI.Label(new Rect(x, y, 760f, 24f), $"active_operative: {activeOperativeId}", labelStyle);
            y += 28f;

            GUI.Label(new Rect(x, y, 900f, 24f), "controls: WASD/Arrows move active | Tab switch | E free hostage | LMB shoot | H hold | F follow | M move order | T attack order", labelStyle);
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
