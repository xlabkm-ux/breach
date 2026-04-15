using Breach.Core;
using Breach.Localization;
using Breach.Squad;
using UnityEngine;

namespace Breach.Combat
{
    public sealed class FriendlyFireAimOverlay : MonoBehaviour
    {
        [SerializeField] private Color safeLineColor = new Color(0.35f, 1f, 1f, 0.9f);
        [SerializeField] private Color riskyLineColor = new Color(1f, 0.3f, 0.3f, 0.95f);
        [SerializeField] private float lineWidth = 0.05f;
        [SerializeField] private int warningFontSize = 18;
        [SerializeField] private string warningLabelKey = "ui.warning.friendly_fire_risk";
        [SerializeField] private bool showOnlyWhileAimHeld = true;

        private Camera cachedCamera;
        private LineRenderer lineRenderer;
        private Material runtimeMaterial;
        private SimpleShooter[] shooters;
        private float nextRefreshTime;
        private bool hasRiskWarning;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureRuntimeInstance()
        {
            if (FindAnyObjectByType<FriendlyFireAimOverlay>() != null)
            {
                return;
            }

            var overlayObject = new GameObject("FriendlyFireAimOverlay_Runtime");
            overlayObject.AddComponent<FriendlyFireAimOverlay>();
            DontDestroyOnLoad(overlayObject);
        }

        private void Awake()
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            runtimeMaterial = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.material = runtimeMaterial;
            lineRenderer.positionCount = 2;
            lineRenderer.widthMultiplier = lineWidth;
            lineRenderer.enabled = false;
            lineRenderer.numCapVertices = 2;
        }

        private void OnDestroy()
        {
            if (runtimeMaterial != null)
            {
                Destroy(runtimeMaterial);
            }
        }

        private void Update()
        {
            hasRiskWarning = false;
            if (cachedCamera == null)
            {
                cachedCamera = Camera.main;
            }
            if (cachedCamera == null)
            {
                lineRenderer.enabled = false;
                return;
            }

            if (!VisualQualityProfileRuntime.EnableAimOverlay)
            {
                lineRenderer.enabled = false;
                return;
            }

            if (Time.unscaledTime >= nextRefreshTime)
            {
                shooters = FindObjectsByType<SimpleShooter>(UnityEngine.FindObjectsInactive.Exclude);
                nextRefreshTime = Time.unscaledTime + 0.5f;
            }

            var activeShooter = GetSelectedShooter();
            if (activeShooter == null || activeShooter.SelfHealth == null)
            {
                lineRenderer.enabled = false;
                return;
            }

            var isAiming = InputCompat.GetMouseButton(1) || InputCompat.GetMouseButton(0);
            if (showOnlyWhileAimHeld && !isAiming)
            {
                lineRenderer.enabled = false;
                hasRiskWarning = false;
                return;
            }

            var from = activeShooter.transform.position;
            var screen = InputCompat.MousePosition;
            var world = cachedCamera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, Mathf.Abs(cachedCamera.transform.position.z)));
            var direction = world - from;
            direction.z = 0f;
            if (direction.sqrMagnitude <= 0.0001f)
            {
                lineRenderer.enabled = false;
                return;
            }

            var aimDistance = Mathf.Min(activeShooter.MaxDistance, direction.magnitude);
            var normalizedDirection = direction.normalized;
            var end = from + normalizedDirection * aimDistance;
            var hasFriendlyRisk = DetectFriendlyRisk(from, normalizedDirection, aimDistance, activeShooter.SelfHealth);
            hasRiskWarning = hasFriendlyRisk;

            lineRenderer.enabled = true;
            lineRenderer.startColor = hasFriendlyRisk ? riskyLineColor : safeLineColor;
            lineRenderer.endColor = hasFriendlyRisk ? riskyLineColor : safeLineColor;
            lineRenderer.SetPosition(0, from);
            lineRenderer.SetPosition(1, end);
        }

        private void OnGUI()
        {
            if (!hasRiskWarning)
            {
                return;
            }

            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = warningFontSize,
                fontStyle = FontStyle.Bold
            };
            style.normal.textColor = riskyLineColor;
            var rect = new Rect((Screen.width - 320f) * 0.5f, 26f, 320f, 28f);
            GUI.Label(rect, LocalizationService.Resolve(warningLabelKey), style);
        }

        private SimpleShooter GetSelectedShooter()
        {
            if (shooters == null)
            {
                return null;
            }

            foreach (var shooter in shooters)
            {
                if (shooter == null)
                {
                    continue;
                }

                var operative = shooter.GetComponent<OperativeMember>();
                if (operative != null && operative.IsSelected)
                {
                    return shooter;
                }
            }

            return null;
        }

        private static bool DetectFriendlyRisk(Vector3 from, Vector3 dir, float distance, HealthComponent attacker)
        {
            var hits = Physics2D.RaycastAll(from, dir, distance);
            foreach (var hit in hits)
            {
                if (hit.collider == null)
                {
                    continue;
                }

                var health = hit.collider.GetComponent<HealthComponent>();
                if (health == null || health == attacker)
                {
                    continue;
                }

                if (health.Team == attacker.Team)
                {
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}
