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
        private bool isListeningToNoise;
        private int lastProcessedNoiseSequence;

        public EnemyAlertState CurrentState => currentState;

        private void OnEnable()
        {
            SubscribeToNoiseEvents();
            BroadcastStateIfNeeded();
        }

        private void OnDisable()
        {
            UnsubscribeFromNoiseEvents();
        }

        private void Awake()
        {
            EnsureVisionCone();
            lastProcessedNoiseSequence = NoiseEventBus.LatestSequence;
            SubscribeToNoiseEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromNoiseEvents();
        }

        private void Update()
        {
            EnsureVisionCone();
            TryConsumeLatestNoise();

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
            if (noise.Sequence <= lastProcessedNoiseSequence)
            {
                return;
            }

            lastProcessedNoiseSequence = noise.Sequence;
            ProcessNoise(noise);
        }

        private void ProcessNoise(NoiseEvent noise)
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

        private void TryConsumeLatestNoise()
        {
            if (!NoiseEventBus.TryGetLatestNoise(out var noise))
            {
                return;
            }

            if (noise.Sequence <= lastProcessedNoiseSequence)
            {
                return;
            }

            lastProcessedNoiseSequence = noise.Sequence;
            ProcessNoise(noise);
        }

        private void EnsureVisionCone()
        {
            if (visionCone != null)
            {
                return;
            }

            visionCone = GetComponent<EnemyVisionCone>();
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

        private void SubscribeToNoiseEvents()
        {
            if (isListeningToNoise || !isActiveAndEnabled)
            {
                return;
            }

            NoiseEventBus.NoiseRaised += OnNoiseRaised;
            isListeningToNoise = true;
        }

        private void UnsubscribeFromNoiseEvents()
        {
            if (!isListeningToNoise)
            {
                return;
            }

            NoiseEventBus.NoiseRaised -= OnNoiseRaised;
            isListeningToNoise = false;
        }
    }
}
