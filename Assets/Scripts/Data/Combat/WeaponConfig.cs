using UnityEngine;

namespace Breach.Data.Combat
{
    [CreateAssetMenu(menuName = "Breach/Combat/Weapon Config", fileName = "WeaponConfig")]
    public sealed class WeaponConfig : ScriptableObject
    {
        public string weaponId = "rifle";
        public int damagePerShot = 34;
        public float maxDistance = 20f;
        public float noiseRadius = 8f;
    }
}
