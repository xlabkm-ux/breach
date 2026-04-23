using UnityEngine;
using UnityEngine.AI;
using TacticalBreach.Combat;

namespace TacticalBreach.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class EnemyPatrolAgent : MonoBehaviour
    {
        [Header("Patrol Settings")]
        [SerializeField] private float patrolRadius = 8f;
        [SerializeField] private float waitTime = 2f;
        
        private NavMeshAgent agent;
        private HealthComponent health;
        private float nextPatrolTime;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<HealthComponent>();
            
            // Configure agent for 2D top-down (XY plane)
            agent.updateRotation = false;
            agent.updateUpAxis = false;

            if (health != null)
            {
                health.SetTeam(TeamId.Enemy);
            }
        }

        private void Start()
        {
            SetRandomDestination();
        }

        private void Update()
        {
            if (!agent.isOnNavMesh) return;

            // Check if arrived
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                if (Time.time >= nextPatrolTime)
                {
                    SetRandomDestination();
                }
            }
        }

        private void SetRandomDestination()
        {
            if (!agent.isOnNavMesh) return;

            Vector2 randomDir = Random.insideUnitCircle * patrolRadius;
            Vector3 targetPos = transform.position + new Vector3(randomDir.x, randomDir.y, 0);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPos, out hit, patrolRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                nextPatrolTime = Time.time + waitTime;
            }
        }
    }
}
