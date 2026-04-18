using UnityEngine;

namespace TacticalBreach.Data.AI
{
    [CreateAssetMenu(menuName = "Breach/AI/Perception Config", fileName = "PerceptionConfig")]
    public sealed class PerceptionConfig : ScriptableObject
    {
        public float visionRadius = 6f;
        public float visionAngle = 70f;
        public float gunshotNoiseRadius = 8f;
        public float suspiciousDuration = 4f;
    }
}
