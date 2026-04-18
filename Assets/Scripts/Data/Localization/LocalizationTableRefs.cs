using TacticalBreach.Localization;
using UnityEngine;

namespace TacticalBreach.Data.Localization
{
    [CreateAssetMenu(menuName = "Breach/Localization/Table Refs", fileName = "LocalizationTableRefs")]
    public sealed class LocalizationTableRefs : ScriptableObject
    {
        public LocalizationTableAsset defaultTable;
        public string fallbackLanguage = "en";
    }
}
