using Breach.Hostage;
using UnityEngine;

namespace Breach.Mission
{
    public sealed class ExtractionZoneTrigger : MonoBehaviour
    {
        [SerializeField] private float radius = 1.8f;
        [SerializeField] private ObjectiveService objectiveService;

        private bool alreadyMarked;

        private void Awake()
        {
            if (objectiveService == null)
            {
                objectiveService = FindFirstObjectByType<ObjectiveService>();
            }
        }

        private void Update()
        {
            if (alreadyMarked)
            {
                return;
            }

            var hostage = FindFirstObjectByType<HostageController>();
            if (hostage == null || !hostage.IsFreed)
            {
                return;
            }

            var distance = Vector2.Distance(transform.position, hostage.transform.position);
            if (distance <= radius)
            {
                objectiveService?.MarkHostageExtracted();
                alreadyMarked = true;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
