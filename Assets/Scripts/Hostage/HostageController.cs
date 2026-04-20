using TacticalBreach.Combat;
using TacticalBreach.Mission;
using TacticalBreach.Squad;
using UnityEngine;

namespace TacticalBreach.Hostage
{
    public sealed class HostageController : MonoBehaviour
    {
        [SerializeField] private float followSpeed = 2.3f;
        [SerializeField] private float interactionDistance = 1.8f;
        [SerializeField] private ActiveOperativeSwitchService switchService;
        [SerializeField] private ObjectiveService objectiveService;

        private HealthComponent health;
        private bool isFreed;

        public bool IsFreed => isFreed;

        private void Awake()
        {
            ResolveDependencies();
        }

        private void Start()
        {
            var myColl = GetComponent<CircleCollider2D>();
            if (myColl != null)
            {
                var others = FindObjectsByType<CircleCollider2D>(FindObjectsSortMode.None);
                foreach (var coll in others)
                {
                    if (coll != myColl && coll.gameObject != this.gameObject)
                        Physics2D.IgnoreCollision(myColl, coll);
                }
            }
        }

        private void Update()
        {
            ResolveDependencies();

            if (health != null && health.IsDead)
            {
                objectiveService?.MarkHostageKilled();
                return;
            }

            var activeOperative = switchService != null ? switchService.ActiveOperative : null;
            if (activeOperative == null)
            {
                return;
            }

            if (!isFreed)
            {
                TryFree(activeOperative.transform);
                return;
            }

            TickEscort(activeOperative.transform, Time.deltaTime);
        }

        public bool TryFree(Transform activeOperative)
        {
            ResolveDependencies();

            if (isFreed || activeOperative == null)
            {
                return false;
            }

            var distance = Vector2.Distance(transform.position, activeOperative.position);
            var canFree = distance <= interactionDistance;
            if (!canFree)
            {
                return false;
            }

            isFreed = true;
            objectiveService?.MarkHostageFreed();
            return true;
        }

        public void TickEscort(Transform activeOperative, float deltaTime)
        {
            if (!isFreed || activeOperative == null)
            {
                return;
            }

            var current = transform.position;
            var target = activeOperative.position;
            var distance = Vector3.Distance(current, target);
            
            bool blockedByAlly = false;
            var othersColl = Physics2D.OverlapCircleAll(transform.position, 0.6f);
            foreach (var c in othersColl)
            {
                if (c.gameObject != this.gameObject && (c.GetComponent<TacticalBreach.Squad.OperativeMember>() != null || c.gameObject.name.Contains("Hostage")))
                {
                    var toTarget = (target - current).normalized;
                    var toAlly = (c.transform.position - current).normalized;
                    if (Vector3.Dot(toTarget, toAlly) > 0.4f)
                    {
                        blockedByAlly = true;
                        break;
                    }
                }
            }

            if (distance > 1.25f && !blockedByAlly)
            {
                var next = Vector3.MoveTowards(current, target, followSpeed * Mathf.Max(0f, deltaTime));
                var rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    var dir = (next - current).normalized;
                    rb.linearVelocity = dir * followSpeed;
                }
                else
                {
                    transform.position = new Vector3(next.x, next.y, current.z);
                }
            }
            else
            {
                var rb = GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }
        }

        private void ResolveDependencies()
        {
            if (health == null)
            {
                health = GetComponent<HealthComponent>();
            }

            if (switchService == null)
            {
                switchService = FindAnyObjectByType<ActiveOperativeSwitchService>();
            }

            if (objectiveService == null)
            {
                objectiveService = FindAnyObjectByType<ObjectiveService>();
            }
            
            var rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.mass = 100f;
                rb.gravityScale = 0f;
                rb.freezeRotation = true;
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

                var coll = gameObject.AddComponent<CircleCollider2D>();
                coll.radius = 0.35f;
            }
            
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
            }
        }
    }
}
