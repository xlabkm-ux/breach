using UnityEngine;
using TacticalBreach.Localization;

namespace TacticalBreach.AI
{
    public sealed class EnemyAlertStateOverlay : MonoBehaviour
    {
        [SerializeField] private Vector2 screenOffset = new Vector2(0f, -36f);
        [SerializeField] private int fontSize = 13;

        private Camera cachedCamera;
        private GUIStyle style;
        private EnemyAlertController[] cachedEnemies;
        private float nextRefreshTime;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureRuntimeInstance()
        {
            if (FindAnyObjectByType<EnemyAlertStateOverlay>() != null)
            {
                return;
            }

            var overlayObject = new GameObject("EnemyAlertStateOverlay_Runtime");
            overlayObject.AddComponent<EnemyAlertStateOverlay>();
            DontDestroyOnLoad(overlayObject);
        }

        private void Update()
        {
            if (cachedCamera == null)
            {
                cachedCamera = Camera.main;
            }

            if (Time.unscaledTime >= nextRefreshTime)
            {
                cachedEnemies = FindObjectsByType<EnemyAlertController>(UnityEngine.FindObjectsInactive.Exclude);
                nextRefreshTime = Time.unscaledTime + 0.4f;
            }
        }

        private void OnGUI()
        {
            if (cachedCamera == null || cachedEnemies == null)
            {
                return;
            }

            EnsureStyle();
            foreach (var enemy in cachedEnemies)
            {
                if (enemy == null)
                {
                    continue;
                }

                var world = enemy.transform.position + new Vector3(0f, 0.7f, 0f);
                var screen = cachedCamera.WorldToScreenPoint(world);
                if (screen.z <= 0f)
                {
                    continue;
                }

                var guiPoint = new Vector2(screen.x, Screen.height - screen.y) + screenOffset;
                style.normal.textColor = GetStateColor(enemy.CurrentState);
                var rect = new Rect(guiPoint.x - 48f, guiPoint.y, 96f, 20f);
                GUI.Label(rect, ResolveStateLabel(enemy.CurrentState), style);
            }
        }

        private void EnsureStyle()
        {
            if (style != null)
            {
                return;
            }

            style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = fontSize,
                fontStyle = FontStyle.Bold
            };
        }

        private static Color GetStateColor(EnemyAlertState state)
        {
            return state switch
            {
                EnemyAlertState.Alert => new Color(1f, 0.25f, 0.25f),
                EnemyAlertState.Suspicious => new Color(1f, 0.75f, 0.2f),
                _ => new Color(0.6f, 1f, 0.6f)
            };
        }

        private static string ResolveStateLabel(EnemyAlertState state)
        {
            var key = state switch
            {
                EnemyAlertState.Alert => "ui.alert.alert",
                EnemyAlertState.Suspicious => "ui.alert.suspicious",
                _ => "ui.alert.idle"
            };

            return LocalizationService.Resolve(key);
        }
    }
}
