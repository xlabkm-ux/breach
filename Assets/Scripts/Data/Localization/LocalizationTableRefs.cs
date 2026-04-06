using Breach.Localization;
using UnityEngine;

namespace Breach.Data.Localization
{
    [CreateAssetMenu(menuName = "Breach/Localization/Table Refs", fileName = "LocalizationTableRefs")]
    public sealed class LocalizationTableRefs : ScriptableObject
    {
        public LocalizationTableAsset defaultTable;
        public string fallbackLanguage = "en";
    }
}
