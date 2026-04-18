using System;
using UnityEngine;

namespace TacticalBreach.Mission
{
    public sealed class ObjectiveService : MonoBehaviour
    {
        public event Action MilestoneReached;
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
        public bool IsMissionSuccessCandidate =>
            infiltrationComplete &&
            hostageFreed &&
            hostageExtracted &&
            hostageAlive &&
            squadAlive;

        public void MarkInfiltrationComplete()
        {
            infiltrationComplete = true;
            MilestoneReached?.Invoke();
        }

        public void MarkHostageFreed()
        {
            hostageFreed = true;
            MilestoneReached?.Invoke();
        }

        public void MarkHostageExtracted()
        {
            hostageExtracted = true;
            MilestoneReached?.Invoke();
        }

        public void MarkSquadWiped()
        {
            squadAlive = false;
        }

        public void MarkHostageKilled()
        {
            hostageAlive = false;
        }

        public void ResetForNewMission()
        {
            infiltrationComplete = false;
            hostageFreed = false;
            hostageExtracted = false;
            squadAlive = true;
            hostageAlive = true;
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
