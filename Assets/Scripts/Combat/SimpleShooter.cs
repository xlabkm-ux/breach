using Breach.Squad;
using UnityEngine;
using Breach.AI;

namespace Breach.Combat
{
    public sealed class SimpleShooter : MonoBehaviour
    {
        [SerializeField] private int damagePerShot = 34;
        [SerializeField] private float maxDistance = 20f;

        private Camera cachedCamera;
        private HealthComponent selfHealth;
        private OperativeMember operativeMember;
        private CombatResolver resolver;
        private NoiseEmitter noiseEmitter;

        private void Awake()
        {
            cachedCamera = Camera.main;
            selfHealth = GetComponent<HealthComponent>();
            operativeMember = GetComponent<OperativeMember>();
            resolver = FindFirstObjectByType<CombatResolver>();
            noiseEmitter = GetComponent<NoiseEmitter>();
        }

        private void Update()
        {
            if (operativeMember == null || !operativeMember.IsSelected)
            {
                return;
            }

            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            if (cachedCamera == null)
            {
                cachedCamera = Camera.main;
                if (cachedCamera == null)
                {
                    return;
                }
            }

            if (resolver == null)
            {
                resolver = FindFirstObjectByType<CombatResolver>();
                if (resolver == null)
                {
                    return;
                }
            }

            var from = transform.position;
            var cursor = Input.mousePosition;
            var world = cachedCamera.ScreenToWorldPoint(new Vector3(cursor.x, cursor.y, Mathf.Abs(cachedCamera.transform.position.z)));
            var dir = (world - from);
            dir.z = 0f;
            if (dir.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            var hit = Physics2D.Raycast(from, dir.normalized, maxDistance);
            if (hit.collider == null)
            {
                return;
            }

            var target = hit.collider.GetComponent<HealthComponent>();
            if (target == null)
            {
                return;
            }

            noiseEmitter?.EmitGunshotNoise();
            resolver.TryResolveHit(selfHealth, target, damagePerShot);
        }
    }
}
