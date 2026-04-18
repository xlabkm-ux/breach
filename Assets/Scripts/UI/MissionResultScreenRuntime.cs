using TacticalBreach.Localization;
using TacticalBreach.Mission;
using UnityEngine;

namespace TacticalBreach.UI
{
    public sealed class MissionResultScreenRuntime : MonoBehaviour
    {
        [SerializeField] private Color panelColor = new Color(0f, 0f, 0f, 0.65f);
        [SerializeField] private int titleFontSize = 28;
        [SerializeField] private int bodyFontSize = 18;
        private const float MinPanelWidth = 320f;
        private const float MaxPanelWidth = 620f;

        private MissionStateService missionStateService;
        private ResultScreenController resultScreenController;
        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureRuntimeInstance()
        {
            if (FindAnyObjectByType<MissionResultScreenRuntime>() != null)
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
                missionStateService = FindAnyObjectByType<MissionStateService>();
            }

            if (resultScreenController == null)
            {
                resultScreenController = FindAnyObjectByType<ResultScreenController>();
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
            var width = GetPanelWidth(Screen.width);
            var titleKey = resultScreenController != null
                ? resultScreenController.GetTitleKey(success)
                : (success ? "ui.result.success.title" : "ui.result.fail.title");

            var bodyKey = resultScreenController != null
                ? resultScreenController.GetBodyKey(success)
                : (success ? "ui.result.success.body" : "ui.result.fail.body");

            var title = LocalizationService.Resolve(titleKey);
            var body = LocalizationService.Resolve(bodyKey);
            var hint = LocalizationService.Resolve("ui.result.hint.restart");

            var contentWidth = width - 40f;
            var titleHeight = Mathf.Max(48f, titleStyle.CalcHeight(new GUIContent(title), contentWidth));
            var bodyHeight = Mathf.Max(48f, bodyStyle.CalcHeight(new GUIContent(body), contentWidth));
            var hintHeight = Mathf.Max(32f, bodyStyle.CalcHeight(new GUIContent(hint), contentWidth));
            var height = 24f + titleHeight + 16f + bodyHeight + 12f + hintHeight + 20f;
            var x = (Screen.width - width) * 0.5f;
            var y = (Screen.height - height) * 0.5f;

            var previousColor = GUI.color;
            GUI.color = panelColor;
            GUI.Box(new Rect(x, y, width, height), GUIContent.none);
            GUI.color = previousColor;

            var cursorY = y + 24f;
            GUI.Label(new Rect(x + 20f, cursorY, contentWidth, titleHeight), title, titleStyle);
            cursorY += titleHeight + 16f;
            GUI.Label(new Rect(x + 20f, cursorY, contentWidth, bodyHeight), body, bodyStyle);
            cursorY += bodyHeight + 12f;
            GUI.Label(new Rect(x + 20f, cursorY, contentWidth, hintHeight), hint, bodyStyle);
        }

        public static float GetPanelWidth(float screenWidth)
        {
            return Mathf.Clamp(screenWidth - 40f, MinPanelWidth, MaxPanelWidth);
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
