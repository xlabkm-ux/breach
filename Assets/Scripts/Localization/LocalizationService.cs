using UnityEngine;
using System.Globalization;

namespace Breach.Localization
{
    public sealed class LocalizationService : MonoBehaviour
    {
        [SerializeField] private string resourcePath = "Localization/DefaultLocalizationTable";
        [SerializeField] private string fallbackLanguageCode = "en";
        [SerializeField] private string runtimeLanguageCode = "en";

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

        public static string ResolveFormat(string key, params object[] args)
        {
            var raw = Resolve(key);
            if (args == null || args.Length == 0)
            {
                return raw;
            }

            try
            {
                return string.Format(CultureInfo.InvariantCulture, raw, args);
            }
            catch
            {
                return raw;
            }
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
            runtimeLanguageCode = ResolveRuntimeLanguageCode();
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

            var resolved = tableAsset.Resolve(key, runtimeLanguageCode);
            if (resolved == key && !string.Equals(runtimeLanguageCode, fallbackLanguageCode, System.StringComparison.OrdinalIgnoreCase))
            {
                resolved = tableAsset.Resolve(key, fallbackLanguageCode);
            }

            return resolved;
        }

        private static string ResolveRuntimeLanguageCode()
        {
            return Application.systemLanguage switch
            {
                SystemLanguage.Russian => "ru",
                _ => "en"
            };
        }
    }
}
