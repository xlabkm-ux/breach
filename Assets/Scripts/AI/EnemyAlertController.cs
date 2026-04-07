using UnityEngine;

namespace Breach.AI
{
    public enum EnemyAlertState
    {
        Idle = 0,
        Suspicious = 1,
        Alert = 2
    }

    public sealed class EnemyAlertController : MonoBehaviour
    {
        [SerializeField] private EnemyVisionCone visionCone;
        [SerializeField] private float suspiciousDuration = 4f;

        private EnemyAlertState currentState = EnemyAlertState.Idle;
        private float suspiciousUntil;
        private EnemyAlertState lastBroadcastState = (EnemyAlertState)(-1);

        public EnemyAlertState CurrentState => currentState;

        private void OnEnable()
        {
            NoiseEventBus.NoiseRaised += OnNoiseRaised;
            BroadcastStateIfNeeded();
        }

        private void OnDisable()
        {
            NoiseEventBus.NoiseRaised -= OnNoiseRaised;
        }

        private void Awake()
        {
            if (visionCone == null)
            {
                visionCone = GetComponent<EnemyVisionCone>();
            }
        }

        private void Update()
        {
            if (visionCone != null && visionCone.HasTarget)
            {
                currentState = EnemyAlertState.Alert;
            }
            else if (currentState == EnemyAlertState.Alert && (visionCone == null || !visionCone.HasTarget))
            {
                currentState = EnemyAlertState.Suspicious;
                suspiciousUntil = Time.time + suspiciousDuration;
            }
            else if (currentState == EnemyAlertState.Suspicious && Time.time >= suspiciousUntil)
            {
                currentState = EnemyAlertState.Idle;
            }

            BroadcastStateIfNeeded();
        }

        private void OnNoiseRaised(NoiseEvent noise)
        {
            var distance = Vector2.Distance(transform.position, noise.Position);
            if (distance > noise.Radius)
            {
                return;
            }

            if (currentState == EnemyAlertState.Idle)
            {
                currentState = EnemyAlertState.Suspicious;
            }

            suspiciousUntil = Time.time + suspiciousDuration;
            BroadcastStateIfNeeded();
        }

        private void BroadcastStateIfNeeded()
        {
            if (lastBroadcastState == currentState)
            {
                return;
            }

            lastBroadcastState = currentState;
            EnemyAlertVsEvents.Raise(currentState, gameObject);
        }
    }
}
