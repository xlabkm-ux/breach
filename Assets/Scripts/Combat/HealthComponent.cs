using UnityEngine;

namespace Breach.Combat
{
    public sealed class HealthComponent : MonoBehaviour
    {
        [SerializeField] private TeamId team = TeamId.Neutral;
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth = 100;

        public TeamId Team => team;
        public int CurrentHealth => currentHealth;
        public bool IsDead => currentHealth <= 0;

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
            if (currentHealth == 0)
            {
                gameObject.SetActive(false);
            }

            return true;
        }
    }
}
