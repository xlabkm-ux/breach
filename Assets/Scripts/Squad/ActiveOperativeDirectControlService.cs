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
            if (input.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            var normalized = input.normalized;
            var delta = new Vector3(normalized.x, normalized.y, 0f) * (moveSpeed * Time.deltaTime);
            activeOperative.transform.position += delta;
        }
    }
}
