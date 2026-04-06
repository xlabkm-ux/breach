using UnityEngine;

namespace Breach.AI
{
    [RequireComponent(typeof(EnemyVisionCone))]
    public sealed class EnemyVisionConeOverlay : MonoBehaviour
    {
        [SerializeField] private int segments = 24;
        [SerializeField] private float zOffset = 0.05f;
        [SerializeField] private Color idleColor = new Color(1f, 0.95f, 0.2f, 0.2f);
        [SerializeField] private Color alertColor = new Color(1f, 0.2f, 0.2f, 0.3f);

        private EnemyVisionCone visionCone;
        private Mesh mesh;
        private Material material;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AttachOverlaysAtRuntime()
        {
            var cones = FindObjectsByType<EnemyVisionCone>(FindObjectsSortMode.None);
            foreach (var cone in cones)
            {
                if (cone.GetComponent<EnemyVisionConeOverlay>() == null)
                {
                    cone.gameObject.AddComponent<EnemyVisionConeOverlay>();
                }
            }
        }

        private void Awake()
        {
            visionCone = GetComponent<EnemyVisionCone>();
            mesh = new Mesh { name = "EnemyVisionConeOverlayMesh" };
            material = new Material(Shader.Find("Sprites/Default"));
        }

        private void OnDestroy()
        {
            if (mesh != null)
            {
                Destroy(mesh);
            }
            if (material != null)
            {
                Destroy(material);
            }
        }

        private void LateUpdate()
        {
            if (visionCone == null || mesh == null)
            {
                return;
            }

            BuildConeMesh();
        }

        private void OnRenderObject()
        {
            if (mesh == null || material == null || visionCone == null)
            {
                return;
            }

            material.color = visionCone.HasTarget ? alertColor : idleColor;
            material.SetPass(0);
            Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
        }

        private void BuildConeMesh()
        {
            var safeSegments = Mathf.Max(6, segments);
            var radius = Mathf.Max(0.1f, visionCone.Radius);
            var halfAngle = visionCone.AngleDeg * 0.5f;

            var vertices = new Vector3[safeSegments + 2];
            var triangles = new int[safeSegments * 3];

            vertices[0] = transform.position + new Vector3(0f, 0f, zOffset);

            for (var i = 0; i <= safeSegments; i++)
            {
                var t = i / (float)safeSegments;
                var angle = Mathf.Lerp(-halfAngle, halfAngle, t);
                var direction = Quaternion.Euler(0f, 0f, angle) * transform.right;
                var point = transform.position + direction * radius;
                vertices[i + 1] = new Vector3(point.x, point.y, point.z + zOffset);
            }

            var triangleIndex = 0;
            for (var i = 0; i < safeSegments; i++)
            {
                triangles[triangleIndex++] = 0;
                triangles[triangleIndex++] = i + 1;
                triangles[triangleIndex++] = i + 2;
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
        }
    }
}
