using UnityEngine;
using Breach.Core;

namespace Breach.AI
{
    public sealed class NoisePulseVisualizer : MonoBehaviour
    {
        [SerializeField] private float pulseDuration = 0.45f;
        [SerializeField] private float ringWidth = 0.06f;
        [SerializeField] private Color pulseColor = new Color(1f, 0.35f, 0.15f, 0.8f);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureRuntimeInstance()
        {
            if (FindAnyObjectByType<NoisePulseVisualizer>() != null)
            {
                return;
            }

            var visualizerObject = new GameObject("NoisePulseVisualizer_Runtime");
            visualizerObject.AddComponent<NoisePulseVisualizer>();
            DontDestroyOnLoad(visualizerObject);
        }

        private void OnEnable()
        {
            NoiseEventBus.NoiseRaised += OnNoiseRaised;
        }

        private void OnDisable()
        {
            NoiseEventBus.NoiseRaised -= OnNoiseRaised;
        }

        private void OnNoiseRaised(NoiseEvent noise)
        {
            if (!VisualQualityProfileRuntime.EnableNoiseOverlay)
            {
                return;
            }

            var ringObject = new GameObject("NoisePulseRing");
            ringObject.transform.position = new Vector3(noise.Position.x, noise.Position.y, noise.Position.z - 0.05f);

            var ring = ringObject.AddComponent<NoisePulseRing>();
            ring.Configure(
                noise.Radius,
                pulseDuration * VisualQualityProfileRuntime.EffectIntensity,
                ringWidth,
                pulseColor);
        }
    }
}
