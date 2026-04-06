using UnityEngine;

namespace Breach.Combat
{
    public sealed class CombatResolver : MonoBehaviour
    {
        [SerializeField] private bool friendlyFireEnabled = true;

        public bool TryResolveHit(HealthComponent attacker, HealthComponent target, int damage)
        {
            if (target == null || damage <= 0 || target.IsDead)
            {
                return false;
            }

            if (!friendlyFireEnabled && attacker != null && attacker.Team == target.Team)
            {
                return false;
            }

            return target.ApplyDamage(damage);
        }
    }
}
