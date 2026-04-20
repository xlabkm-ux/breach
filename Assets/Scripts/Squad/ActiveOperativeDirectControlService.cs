using TacticalBreach.Core;
using UnityEngine;

namespace TacticalBreach.Squad
{
    public sealed class ActiveOperativeDirectControlService : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4.2f;
        [SerializeField] private ActiveOperativeSwitchService switchService;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureRuntimeController()
        {
            if (FindAnyObjectByType<ActiveOperativeDirectControlService>() != null)
            {
                return;
            }

            var runtimeObject = new GameObject("ActiveOperativeDirectControl_Runtime");
            runtimeObject.AddComponent<ActiveOperativeDirectControlService>();
        }

        private void Awake()
        {
            if (switchService == null)
            {
                switchService = FindAnyObjectByType<ActiveOperativeSwitchService>();
            }
        }

        private void Update()
        {
            if (switchService == null)
            {
                return;
            }

            var activeOperative = switchService.ActiveOperative;
            if (activeOperative == null)
            {
                return;
            }

            var input = InputCompat.MoveVector;
            var normalized = input.normalized;

            var rb = activeOperative.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = activeOperative.gameObject.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 0f;
                rb.freezeRotation = true;
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

                var coll = activeOperative.gameObject.AddComponent<CircleCollider2D>();
                coll.radius = 0.4f;
            }

            if (input.sqrMagnitude <= 0.0001f)
            {
                rb.linearVelocity = Vector2.zero;
            }
            else
            {
                rb.linearVelocity = normalized * moveSpeed;
            }
        }
    }
}
