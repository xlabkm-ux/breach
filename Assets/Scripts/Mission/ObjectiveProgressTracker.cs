using TacticalBreach.Combat;
using TacticalBreach.Squad;
using UnityEngine;

namespace TacticalBreach.Mission
{
    public sealed class ObjectiveProgressTracker : MonoBehaviour
    {
        [SerializeField] private float infiltrationMoveThreshold = 0.45f;
        [SerializeField] private MissionStateService missionStateService;
        [SerializeField] private ObjectiveService objectiveService;

        private OperativeMember[] operatives;
        private Vector3[] initialPositions;
        private HealthComponent[] operativeHealth;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureRuntimeInstance()
        {
            if (FindAnyObjectByType<ObjectiveProgressTracker>() != null)
            {
                return;
            }

            var runtimeObject = new GameObject("ObjectiveProgressTracker_Runtime");
            runtimeObject.AddComponent<ObjectiveProgressTracker>();
        }

        private void Awake()
        {
            if (missionStateService == null)
            {
                missionStateService = FindAnyObjectByType<MissionStateService>();
            }
            if (objectiveService == null)
            {
                objectiveService = FindAnyObjectByType<ObjectiveService>();
            }

            operatives = FindObjectsByType<OperativeMember>(UnityEngine.FindObjectsInactive.Exclude);
            initialPositions = new Vector3[operatives.Length];
            operativeHealth = new HealthComponent[operatives.Length];

            for (var i = 0; i < operatives.Length; i++)
            {
                initialPositions[i] = operatives[i] != null ? operatives[i].transform.position : Vector3.zero;
                operativeHealth[i] = operatives[i] != null ? operatives[i].GetComponent<HealthComponent>() : null;
            }
        }

        private void Update()
        {
            if (objectiveService == null)
            {
                return;
            }

            TryMarkInfiltrationProgress();
            TryMarkSquadWiped();
        }

        private void TryMarkInfiltrationProgress()
        {
            if (objectiveService.InfiltrationComplete || missionStateService == null)
            {
                return;
            }

            if (missionStateService.CurrentState != MissionState.Infiltration)
            {
                return;
            }

            for (var i = 0; i < operatives.Length; i++)
            {
                var operative = operatives[i];
                if (operative == null)
                {
                    continue;
                }

                var moved = Vector2.Distance(operative.transform.position, initialPositions[i]);
                if (moved < infiltrationMoveThreshold)
                {
                    continue;
                }

                objectiveService.MarkInfiltrationComplete();
                missionStateService.TryTransition(MissionState.Engagement, out _);
                return;
            }
        }

        private void TryMarkSquadWiped()
        {
            if (!objectiveService.SquadAlive)
            {
                return;
            }

            if (operativeHealth == null || operativeHealth.Length == 0)
            {
                return;
            }

            for (var i = 0; i < operativeHealth.Length; i++)
            {
                var health = operativeHealth[i];
                if (health != null && !health.IsDead)
                {
                    return;
                }
            }

            objectiveService.MarkSquadWiped();
        }
    }
}
