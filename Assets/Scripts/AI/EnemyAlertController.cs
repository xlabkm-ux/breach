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

        public EnemyAlertState CurrentState => currentState;

        private void OnEnable()
        {
            NoiseEventBus.NoiseRaised += OnNoiseRaised;
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
                return;
            }

            if (currentState == EnemyAlertState.Alert && (visionCone == null || !visionCone.HasTarget))
            {
                currentState = EnemyAlertState.Suspicious;
                suspiciousUntil = Time.time + suspiciousDuration;
                return;
            }

            if (currentState == EnemyAlertState.Suspicious && Time.time >= suspiciousUntil)
            {
                currentState = EnemyAlertState.Idle;
            }
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
        }
    }
}
