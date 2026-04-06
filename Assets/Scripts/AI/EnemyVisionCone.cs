using Breach.Squad;
using UnityEngine;

namespace Breach.AI
{
    public sealed class EnemyVisionCone : MonoBehaviour
    {
        [SerializeField] private float radius = 6f;
        [SerializeField] private float angleDeg = 70f;
        [SerializeField] private LayerMask targetMask = ~0;

        private OperativeMember detectedTarget;
        private float lastDetectionTime;

        public bool HasTarget => detectedTarget != null;
        public OperativeMember DetectedTarget => detectedTarget;
        public float LastDetectionTime => lastDetectionTime;

        private void Update()
        {
            detectedTarget = null;
            var colliders = Physics2D.OverlapCircleAll(transform.position, radius, targetMask);
            var forward = Vector2.right;

            foreach (var col in colliders)
            {
                var candidate = col.GetComponent<OperativeMember>();
                if (candidate == null)
                {
                    continue;
                }

                var toTarget = (Vector2)(candidate.transform.position - transform.position);
                var delta = Vector2.Angle(forward, toTarget);
                if (delta <= angleDeg * 0.5f)
                {
                    detectedTarget = candidate;
                    lastDetectionTime = Time.time;
                    break;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = HasTarget ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
