using Breach.Squad;
using UnityEngine;
using Breach.AI;
using Breach.Core;

namespace Breach.Combat
{
    public sealed class SimpleShooter : MonoBehaviour
    {
        [SerializeField] private int damagePerShot = 34;
        [SerializeField] private float maxDistance = 20f;
        [SerializeField] private float selfIgnoreDistance = 0.25f;

        private Camera cachedCamera;
        private HealthComponent selfHealth;
        private OperativeMember operativeMember;
        private CombatResolver resolver;
        private NoiseEmitter noiseEmitter;

        public float MaxDistance => maxDistance;
        public HealthComponent SelfHealth => selfHealth;

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

            if (!InputCompat.GetMouseButtonDown(0))
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
            var cursor = InputCompat.MousePosition;
            var world = cachedCamera.ScreenToWorldPoint(new Vector3(cursor.x, cursor.y, Mathf.Abs(cachedCamera.transform.position.z)));
            var dir = (world - from);
            dir.z = 0f;
            if (dir.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            var direction = dir.normalized;
            var castOrigin = from + direction * selfIgnoreDistance;
            var castDistance = Mathf.Max(0.1f, maxDistance - selfIgnoreDistance);
            var hits = Physics2D.RaycastAll(castOrigin, direction, castDistance);
            if (hits == null || hits.Length == 0)
            {
                return;
            }

            HealthComponent target = null;
            for (var i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                if (hit.collider == null)
                {
                    continue;
                }

                var health = hit.collider.GetComponent<HealthComponent>();
                if (health == null)
                {
                    // Non-health collider (wall/obstacle) blocks shot.
                    return;
                }

                if (health == selfHealth)
                {
                    continue;
                }

                target = health;
                break;
            }

            if (target == null)
            {
                return;
            }

            noiseEmitter?.EmitGunshotNoise();
            resolver.TryResolveHit(selfHealth, target, damagePerShot);
        }
    }
}

