using Breach.Combat;
using Breach.Mission;
using Breach.Squad;
using UnityEngine;

namespace Breach.Hostage
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
            var next = Vector3.MoveTowards(current, target, followSpeed * Mathf.Max(0f, deltaTime));
            transform.position = new Vector3(next.x, next.y, current.z);
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
        }
    }
}
