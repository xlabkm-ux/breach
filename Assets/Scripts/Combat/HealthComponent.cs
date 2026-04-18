using UnityEngine;
using System;

namespace TacticalBreach.Combat
{
    public sealed class HealthComponent : MonoBehaviour
    {
        [SerializeField] private TeamId team = TeamId.Neutral;
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth = 100;

        public TeamId Team => team;
        public int CurrentHealth => currentHealth;
        public bool IsDead => currentHealth <= 0;
        public event Action<int, int> Damaged;
        public event Action Died;

        private void Awake()
        {
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }

        public bool ApplyDamage(int damage)
        {
            if (IsDead || damage <= 0)
            {
                return false;
            }

            currentHealth = Mathf.Max(0, currentHealth - damage);
            Damaged?.Invoke(damage, currentHealth);
            if (currentHealth == 0)
            {
                Died?.Invoke();
                gameObject.SetActive(false);
            }

            return true;
        }

        public void SetTeam(TeamId value)
        {
            team = value;
        }
    }
}
