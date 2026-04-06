using UnityEngine;

namespace Breach.Mission
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

            if (objectiveService.IsMissionSuccessCandidate)
            {
                missionStateService.TryTransition(MissionState.Extraction, out _);
                missionStateService.TryTransition(MissionState.Success, out _);
                resolved = true;
            }
        }
    }
}
