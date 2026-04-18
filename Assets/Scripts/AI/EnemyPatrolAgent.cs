using TacticalBreach.Combat;
using UnityEngine;

namespace TacticalBreach.AI
{
    public sealed class EnemyPatrolAgent : MonoBehaviour
    {
        [SerializeField] private float patrolRadius = 1.5f;
        [SerializeField] private float patrolSpeed = 1.2f;

        private Vector3 origin;
        private HealthComponent health;

        private void Awake()
        {
            origin = transform.position;
            health = GetComponent<HealthComponent>();
            if (health != null)
            {
                health.SetTeam(TeamId.Enemy);
            }
        }

        private void Update()
        {
            var x = Mathf.Sin(Time.time * patrolSpeed) * patrolRadius;
            transform.position = new Vector3(origin.x + x, origin.y, transform.position.z);
        }
    }
}
