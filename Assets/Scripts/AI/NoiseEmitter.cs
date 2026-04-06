using UnityEngine;

namespace Breach.AI
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
