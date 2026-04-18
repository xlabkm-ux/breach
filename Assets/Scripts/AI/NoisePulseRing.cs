using UnityEngine;

namespace TacticalBreach.AI
{
    [RequireComponent(typeof(LineRenderer))]
    public sealed class NoisePulseRing : MonoBehaviour
    {
        private const int Segments = 48;

        private LineRenderer lineRenderer;
        private float targetRadius;
        private float duration;
        private float elapsed;
        private Color baseColor;
        private Material runtimeMaterial;

        public void Configure(float radius, float pulseDuration, float width, Color color)
        {
            targetRadius = Mathf.Max(0.1f, radius);
            duration = Mathf.Max(0.1f, pulseDuration);
            baseColor = color;

            EnsureRenderer();
            lineRenderer.widthMultiplier = Mathf.Max(0.01f, width);
            UpdateRing(0.01f, color);
        }

        private void Awake()
        {
            EnsureRenderer();
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            var t = Mathf.Clamp01(elapsed / duration);
            var currentRadius = Mathf.Lerp(0.01f, targetRadius, t);

            var alpha = Mathf.Lerp(baseColor.a, 0f, t);
            var color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            UpdateRing(currentRadius, color);

            if (t >= 1f)
            {
                Destroy(gameObject);
            }
        }

        private void EnsureRenderer()
        {
            if (lineRenderer != null)
            {
                return;
            }

            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.loop = true;
            lineRenderer.useWorldSpace = false;
            lineRenderer.positionCount = Segments;
            runtimeMaterial = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.material = runtimeMaterial;
            lineRenderer.numCapVertices = 2;
        }

        private void OnDestroy()
        {
            if (runtimeMaterial != null)
            {
                Destroy(runtimeMaterial);
            }
        }

        private void UpdateRing(float radius, Color color)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            for (var i = 0; i < Segments; i++)
            {
                var angle = (i / (float)Segments) * Mathf.PI * 2f;
                var position = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
                lineRenderer.SetPosition(i, position);
            }
        }
    }
}
