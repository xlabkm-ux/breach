using UnityEngine;

namespace TacticalBreach.Core
{
    public sealed class VisualQualityProfileRuntime : MonoBehaviour
    {
        public static bool EnableVisionConeOverlay { get; private set; } = true;
        public static bool EnableNoiseOverlay { get; private set; } = true;
        public static bool EnableAimOverlay { get; private set; } = true;
        public static float EffectIntensity { get; private set; } = 1f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureRuntimeInstance()
        {
            if (FindAnyObjectByType<VisualQualityProfileRuntime>() != null)
            {
                return;
            }

            var runtimeObject = new GameObject("VisualQualityProfileRuntime");
            runtimeObject.AddComponent<VisualQualityProfileRuntime>();
            DontDestroyOnLoad(runtimeObject);
        }

        private void Awake()
        {
            RefreshFromCurrentQuality();
        }

        public static void RefreshFromCurrentQuality()
        {
            ApplyByCurrentProfile();
        }

        private static void ApplyByCurrentProfile()
        {
            var qualityName = QualitySettings.names[QualitySettings.GetQualityLevel()];
            var normalized = qualityName.Trim().ToLowerInvariant();

            if (normalized == "android_low")
            {
                EnableVisionConeOverlay = false;
                EnableNoiseOverlay = false;
                EnableAimOverlay = true;
                EffectIntensity = 0.6f;
                return;
            }

            if (normalized == "android_default")
            {
                EnableVisionConeOverlay = true;
                EnableNoiseOverlay = true;
                EnableAimOverlay = true;
                EffectIntensity = 0.8f;
                return;
            }

            EnableVisionConeOverlay = true;
            EnableNoiseOverlay = true;
            EnableAimOverlay = true;
            EffectIntensity = 1f;
        }
    }
}
