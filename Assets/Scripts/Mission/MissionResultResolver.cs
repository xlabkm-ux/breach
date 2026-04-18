using UnityEngine;

namespace TacticalBreach.Mission
{
    public sealed class MissionResultResolver : MonoBehaviour
    {
        [SerializeField] private MissionStateService missionStateService;
        [SerializeField] private ObjectiveService objectiveService;

        private bool resolved;

        private void Awake()
        {
            if (missionStateService == null)
            {
                missionStateService = GetComponent<MissionStateService>();
            }
            if (objectiveService == null)
            {
                objectiveService = GetComponent<ObjectiveService>();
            }
        }

        private void Update()
        {
            if (resolved || missionStateService == null || objectiveService == null)
            {
                return;
            }

            if (objectiveService.IsMissionFailed)
            {
                missionStateService.TryFail(out _);
                resolved = true;
                return;
            }

            TrySyncProgressState();

            if (objectiveService.IsMissionSuccessCandidate)
            {
                resolved = TryResolveSuccessChain();
            }
        }

        private void TrySyncProgressState()
        {
            if (!objectiveService.HostageFreed)
            {
                return;
            }

            if (missionStateService.CurrentState == MissionState.Infiltration ||
                missionStateService.CurrentState == MissionState.Engagement)
            {
                missionStateService.TryTransition(MissionState.HostageSecured, out _);
            }
        }

        private bool TryResolveSuccessChain()
        {
            if (missionStateService.CurrentState == MissionState.NotStarted)
            {
                missionStateService.TryTransition(MissionState.Infiltration, out _);
            }

            if (missionStateService.CurrentState == MissionState.Infiltration ||
                missionStateService.CurrentState == MissionState.Engagement)
            {
                missionStateService.TryTransition(MissionState.HostageSecured, out _);
            }

            if (missionStateService.CurrentState == MissionState.HostageSecured)
            {
                missionStateService.TryTransition(MissionState.Extraction, out _);
            }

            if (missionStateService.CurrentState == MissionState.Extraction)
            {
                missionStateService.TryTransition(MissionState.Success, out _);
            }

            return missionStateService.CurrentState == MissionState.Success;
        }
    }
}
