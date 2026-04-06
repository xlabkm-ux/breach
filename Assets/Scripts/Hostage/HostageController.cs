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
            health = GetComponent<HealthComponent>();
            if (switchService == null)
            {
                switchService = FindFirstObjectByType<ActiveOperativeSwitchService>();
            }
            if (objectiveService == null)
            {
                objectiveService = FindFirstObjectByType<ObjectiveService>();
            }
        }

        private void Update()
        {
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
                var distance = Vector2.Distance(transform.position, activeOperative.transform.position);
                if (distance <= interactionDistance && Input.GetKeyDown(KeyCode.E))
                {
                    isFreed = true;
                    objectiveService?.MarkHostageFreed();
                }
                return;
            }

            var current = transform.position;
            var target = activeOperative.transform.position;
            var next = Vector3.MoveTowards(current, target, followSpeed * Time.deltaTime);
            transform.position = new Vector3(next.x, next.y, current.z);
        }
    }
}
