using UnityEngine;

namespace TacticalBreach.Squad
{
    public enum OperativeCommandMode
    {
        None = 0,
        Hold = 1,
        MoveTo = 2,
        Follow = 3,
        AttackTarget = 4
    }

    public sealed class OperativeMember : MonoBehaviour
    {
        [SerializeField] private string operativeId = "operative";
        [SerializeField] private bool isSelected;
        [SerializeField] private float moveSpeed = 3f;

        private OperativeCommandMode commandMode = OperativeCommandMode.Hold;
        private Vector3 moveTarget;
        private Transform followTarget;
        private Transform attackTarget;

        public string OperativeId => operativeId;
        public bool IsSelected => isSelected;
        public OperativeCommandMode CommandMode => commandMode;

        private void Awake()
        {
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
                coll.radius = 0.5f;
            }
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
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
            }

            if (commandMode == OperativeCommandMode.Hold)
            {
                return;
            }

            if (commandMode == OperativeCommandMode.Follow && followTarget != null)
            {
                moveTarget = followTarget.position;
            }
            else if (commandMode == OperativeCommandMode.AttackTarget && attackTarget != null)
            {
                moveTarget = attackTarget.position;
            }

            var distance = Vector3.Distance(transform.position, moveTarget);
            var isFollowing = commandMode == OperativeCommandMode.Follow;
            var stopDist = isFollowing ? 1.25f : 0.05f;

            bool blockedByAlly = false;
            var othersColl = Physics2D.OverlapCircleAll(transform.position, 0.6f);
            foreach (var c in othersColl)
            {
                if (c.gameObject != this.gameObject && (c.GetComponent<OperativeMember>() != null || c.gameObject.name.Contains("Hostage")))
                {
                    var toTarget = (moveTarget - transform.position).normalized;
                    var toAlly = (c.transform.position - transform.position).normalized;
                    if (Vector3.Dot(toTarget, toAlly) > 0.4f)
                    {
                        blockedByAlly = true;
                        break;
                    }
                }
            }

            var rb = GetComponent<Rigidbody2D>();
            if (distance > stopDist && !blockedByAlly)
            {
                var next = Vector3.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);
                if (rb != null)
                {
                    var dir = (next - transform.position).normalized;
                    rb.linearVelocity = dir * moveSpeed;
                }
                else
                {
                    transform.position = new Vector3(next.x, next.y, transform.position.z);
                }
            }
            else
            {
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }
        }

        public void SetSelectedState(bool selected)
        {
            isSelected = selected;
        }

        public void IssueHold()
        {
            commandMode = OperativeCommandMode.Hold;
        }

        public void IssueMove(Vector3 worldTarget)
        {
            moveTarget = new Vector3(worldTarget.x, worldTarget.y, transform.position.z);
            commandMode = OperativeCommandMode.MoveTo;
            followTarget = null;
            attackTarget = null;
        }

        public void IssueFollow(Transform target)
        {
            if (target == null)
            {
                return;
            }

            followTarget = target;
            moveTarget = target.position;
            commandMode = OperativeCommandMode.Follow;
            attackTarget = null;
        }

        public void IssueAttackTarget(Transform target)
        {
            if (target == null)
            {
                return;
            }

            attackTarget = target;
            moveTarget = target.position;
            commandMode = OperativeCommandMode.AttackTarget;
            followTarget = null;
        }
    }
}
