using UnityEngine;

namespace Breach.Mission
{
    public sealed class MissionPlayBootstrap : MonoBehaviour
    {
        [SerializeField] private bool forceFreshMissionOnPlay = true;
        [SerializeField] private bool ensureInfiltrationOnPlay = true;
        [SerializeField] private MissionStateService missionStateService;
        [SerializeField] private ObjectiveService objectiveService;

        private bool bootstrapped;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureRuntimeBootstrap()
        {
            if (FindAnyObjectByType<MissionPlayBootstrap>() != null)
            {
                return;
            }

            var runtimeObject = new GameObject("MissionPlayBootstrap_Runtime");
            runtimeObject.AddComponent<MissionPlayBootstrap>();
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
        }

        private void Start()
        {
            BootstrapMission();
        }

        private void BootstrapMission()
        {
            if (bootstrapped)
            {
                return;
            }

            bootstrapped = true;
            if (missionStateService == null || objectiveService == null)
            {
                return;
            }

            if (forceFreshMissionOnPlay)
            {
                objectiveService.ResetForNewMission();
                missionStateService.ForceStateForLoad(MissionState.NotStarted);
            }

            if (ensureInfiltrationOnPlay && missionStateService.CurrentState == MissionState.NotStarted)
            {
                missionStateService.TryTransition(MissionState.Infiltration, out _);
            }
        }
    }
}
