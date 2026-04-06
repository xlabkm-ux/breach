using Breach.Mission;
using UnityEngine;

namespace Breach.Save
{
    public sealed class SaveVisualRestoreBridge : MonoBehaviour
    {
        private SaveService saveService;
        private MissionRuntimeStabilizer runtimeStabilizer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureRuntimeInstance()
        {
            if (FindFirstObjectByType<SaveVisualRestoreBridge>() != null)
            {
                return;
            }

            var bridgeObject = new GameObject("SaveVisualRestoreBridge_Runtime");
            bridgeObject.AddComponent<SaveVisualRestoreBridge>();
            DontDestroyOnLoad(bridgeObject);
        }

        private void Update()
        {
            if (saveService == null)
            {
                saveService = FindFirstObjectByType<SaveService>();
                if (saveService != null)
                {
                    saveService.Loaded += OnSaveLoaded;
                }
            }

            if (runtimeStabilizer == null)
            {
                runtimeStabilizer = FindFirstObjectByType<MissionRuntimeStabilizer>();
            }
        }

        private void OnDestroy()
        {
            if (saveService != null)
            {
                saveService.Loaded -= OnSaveLoaded;
            }
        }

        private void OnSaveLoaded(MissionSaveSnapshot snapshot)
        {
            if (runtimeStabilizer == null)
            {
                runtimeStabilizer = FindFirstObjectByType<MissionRuntimeStabilizer>();
            }

            runtimeStabilizer?.ReinitializeForLoadedState();
        }
    }
}
