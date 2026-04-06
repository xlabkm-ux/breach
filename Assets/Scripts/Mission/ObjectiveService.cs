using UnityEngine;

namespace Breach.Mission
{
    public sealed class ObjectiveService : MonoBehaviour
    {
        [Header("Primary Objective Flags")]
        [SerializeField] private bool infiltrationComplete;
        [SerializeField] private bool hostageFreed;
        [SerializeField] private bool hostageExtracted;

        [Header("Fail Conditions")]
        [SerializeField] private bool squadAlive = true;
        [SerializeField] private bool hostageAlive = true;

        public bool InfiltrationComplete => infiltrationComplete;
        public bool HostageFreed => hostageFreed;
        public bool HostageExtracted => hostageExtracted;
        public bool SquadAlive => squadAlive;
        public bool HostageAlive => hostageAlive;

        public bool IsMissionFailed => !squadAlive || !hostageAlive;
        public bool IsMissionSuccessCandidate => hostageExtracted && hostageAlive && squadAlive;

        public void MarkInfiltrationComplete()
        {
            infiltrationComplete = true;
        }

        public void MarkHostageFreed()
        {
            hostageFreed = true;
        }

        public void MarkHostageExtracted()
        {
            hostageExtracted = true;
        }

        public void MarkSquadWiped()
        {
            squadAlive = false;
        }

        public void MarkHostageKilled()
        {
            hostageAlive = false;
        }

        public void ApplySnapshot(
            bool infiltrationCompleteValue,
            bool hostageFreedValue,
            bool hostageExtractedValue,
            bool squadAliveValue,
            bool hostageAliveValue)
        {
            infiltrationComplete = infiltrationCompleteValue;
            hostageFreed = hostageFreedValue;
            hostageExtracted = hostageExtractedValue;
            squadAlive = squadAliveValue;
            hostageAlive = hostageAliveValue;
        }
    }
}
