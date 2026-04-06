using UnityEngine;

namespace Breach.Localization
{
    public sealed class LocalizationService : MonoBehaviour
    {
        [SerializeField] private string resourcePath = "Localization/DefaultLocalizationTable";
        [SerializeField] private string fallbackLanguageCode = "en";

        private static LocalizationService instance;
        private LocalizationTableAsset tableAsset;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureRuntimeInstance()
        {
            if (instance != null)
            {
                return;
            }

            var serviceObject = new GameObject("LocalizationService_Runtime");
            instance = serviceObject.AddComponent<LocalizationService>();
            DontDestroyOnLoad(serviceObject);
        }

        public static string Resolve(string key)
        {
            if (instance == null)
            {
                EnsureRuntimeInstance();
            }

            return instance != null ? instance.ResolveInternal(key) : key;
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            tableAsset = Resources.Load<LocalizationTableAsset>(resourcePath);
        }

        private string ResolveInternal(string key)
        {
            if (tableAsset == null)
            {
                tableAsset = Resources.Load<LocalizationTableAsset>(resourcePath);
            }

            if (tableAsset == null)
            {
                return key;
            }

            return tableAsset.Resolve(key);
        }
    }
}
