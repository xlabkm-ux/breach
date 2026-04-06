using Breach.Localization;
using Breach.Mission;
using UnityEngine;

namespace Breach.UI
{
    public sealed class MissionResultScreenRuntime : MonoBehaviour
    {
        [SerializeField] private Color panelColor = new Color(0f, 0f, 0f, 0.65f);
        [SerializeField] private int titleFontSize = 28;
        [SerializeField] private int bodyFontSize = 18;

        private MissionStateService missionStateService;
        private ResultScreenController resultScreenController;
        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureRuntimeInstance()
        {
            if (FindFirstObjectByType<MissionResultScreenRuntime>() != null)
            {
                return;
            }

            var screenObject = new GameObject("MissionResultScreen_Runtime");
            screenObject.AddComponent<MissionResultScreenRuntime>();
            DontDestroyOnLoad(screenObject);
        }

        private void Update()
        {
            if (missionStateService == null)
            {
                missionStateService = FindFirstObjectByType<MissionStateService>();
            }

            if (resultScreenController == null)
            {
                resultScreenController = FindFirstObjectByType<ResultScreenController>();
            }
        }

        private void OnGUI()
        {
            if (missionStateService == null)
            {
                return;
            }

            var success = missionStateService.CurrentState == MissionState.Success;
            var fail = missionStateService.CurrentState == MissionState.Failed;
            if (!success && !fail)
            {
                return;
            }

            EnsureStyles();
            var width = Mathf.Min(620f, Screen.width - 40f);
            var height = 220f;
            var x = (Screen.width - width) * 0.5f;
            var y = (Screen.height - height) * 0.5f;

            var previousColor = GUI.color;
            GUI.color = panelColor;
            GUI.Box(new Rect(x, y, width, height), GUIContent.none);
            GUI.color = previousColor;

            var titleKey = resultScreenController != null
                ? resultScreenController.GetTitleKey(success)
                : (success ? "ui.result.success.title" : "ui.result.fail.title");

            var bodyKey = resultScreenController != null
                ? resultScreenController.GetBodyKey(success)
                : (success ? "ui.result.success.body" : "ui.result.fail.body");

            var title = LocalizationService.Resolve(titleKey);
            var body = LocalizationService.Resolve(bodyKey);
            var hint = LocalizationService.Resolve("ui.result.hint.restart");

            GUI.Label(new Rect(x + 20f, y + 24f, width - 40f, 48f), title, titleStyle);
            GUI.Label(new Rect(x + 20f, y + 86f, width - 40f, 64f), body, bodyStyle);
            GUI.Label(new Rect(x + 20f, y + 162f, width - 40f, 32f), hint, bodyStyle);
        }

        private void EnsureStyles()
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = titleFontSize,
                    fontStyle = FontStyle.Bold,
                    wordWrap = true
                };
                titleStyle.normal.textColor = Color.white;
            }

            if (bodyStyle == null)
            {
                bodyStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = bodyFontSize,
                    fontStyle = FontStyle.Normal,
                    wordWrap = true
                };
                bodyStyle.normal.textColor = new Color(0.9f, 0.95f, 1f, 1f);
            }
        }
    }
}
