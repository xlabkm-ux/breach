using UnityEngine;

namespace TacticalBreach.AI
{
    public sealed class NoiseEmitter : MonoBehaviour
    {
        [SerializeField] private float gunshotNoiseRadius = 8f;

        public void EmitGunshotNoise()
        {
            NoiseEventBus.Raise(transform.position, gunshotNoiseRadius);
        }
    }
}
