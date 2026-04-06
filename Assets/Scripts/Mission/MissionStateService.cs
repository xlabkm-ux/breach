using System;
using UnityEngine;

namespace Breach.Mission
{
    public enum MissionState
    {
        NotStarted = 0,
        Infiltration = 10,
        Engagement = 20,
        HostageSecured = 30,
        Extraction = 40,
        Success = 90,
        Failed = 99
    }

    public sealed class MissionStateService : MonoBehaviour
    {
        [SerializeField] private MissionState currentState = MissionState.NotStarted;

        public MissionState CurrentState => currentState;

        public event Action<MissionState, MissionState> StateChanged;

        public bool TryTransition(MissionState nextState, out string reason)
        {
            if (nextState == currentState)
            {
                reason = "State is already active.";
                return false;
            }

            if (!IsTransitionAllowed(currentState, nextState))
            {
                reason = $"Transition denied: {currentState} -> {nextState}.";
                return false;
            }

            var previous = currentState;
            currentState = nextState;
            StateChanged?.Invoke(previous, nextState);
            reason = string.Empty;
            return true;
        }

        public bool TryFail(out string reason)
        {
            return TryTransition(MissionState.Failed, out reason);
        }

        private static bool IsTransitionAllowed(MissionState from, MissionState to)
        {
            if (to == MissionState.Failed)
            {
                return from != MissionState.Success;
            }

            return from switch
            {
                MissionState.NotStarted => to == MissionState.Infiltration,
                MissionState.Infiltration => to == MissionState.Engagement || to == MissionState.HostageSecured,
                MissionState.Engagement => to == MissionState.HostageSecured || to == MissionState.Extraction,
                MissionState.HostageSecured => to == MissionState.Extraction,
                MissionState.Extraction => to == MissionState.Success,
                MissionState.Success => false,
                MissionState.Failed => false,
                _ => false
            };
        }
    }
}
