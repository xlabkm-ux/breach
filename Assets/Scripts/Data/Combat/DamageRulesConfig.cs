using UnityEngine;

namespace Breach.Data.Combat
{
    [CreateAssetMenu(menuName = "Breach/Combat/Damage Rules", fileName = "DamageRules")]
    public sealed class DamageRulesConfig : ScriptableObject
    {
        public bool friendlyFireEnabled = true;
        public float headshotMultiplier = 1.5f;
        public float bodyshotMultiplier = 1f;
    }
}
