using TacticalBreach.Mission;
using UnityEngine;

namespace TacticalBreach.Save
{
    public sealed class SaveVisualRestoreBridge : MonoBehaviour
    {
        private SaveService saveService;
        private MissionRuntimeStabilizer runtimeStabilizer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureRuntimeInstance()
        {
            if (FindAnyObjectByType<SaveVisualRestoreBridge>() != null)
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
                saveService = FindAnyObjectByType<SaveService>();
                if (saveService != null)
                {
                    saveService.Loaded += OnSaveLoaded;
                }
            }

            if (runtimeStabilizer == null)
            {
                runtimeStabilizer = FindAnyObjectByType<MissionRuntimeStabilizer>();
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
                runtimeStabilizer = FindAnyObjectByType<MissionRuntimeStabilizer>();
            }

            runtimeStabilizer?.ReinitializeForLoadedState();
        }
    }
}
