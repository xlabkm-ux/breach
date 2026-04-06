using UnityEngine;
using Breach.Core;

namespace Breach.Combat
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class DamageFlashFeedback : MonoBehaviour
    {
        [SerializeField] private Color flashColor = new Color(1f, 1f, 1f, 1f);
        [SerializeField] private float flashDuration = 0.1f;

        private HealthComponent health;
        private SpriteRenderer spriteRenderer;
        private Color baseColor;
        private float flashUntil;

        private void Awake()
        {
            health = GetComponent<HealthComponent>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            baseColor = spriteRenderer.color;
        }

        private void OnEnable()
        {
            if (health != null)
            {
                health.Damaged += OnDamaged;
            }
        }

        private void OnDisable()
        {
            if (health != null)
            {
                health.Damaged -= OnDamaged;
            }
        }

        private void Update()
        {
            if (spriteRenderer == null)
            {
                return;
            }

            if (Time.time <= flashUntil)
            {
                spriteRenderer.color = flashColor;
                return;
            }

            spriteRenderer.color = baseColor;
        }

        private void OnDamaged(int damage, int hpLeft)
        {
            if (spriteRenderer == null)
            {
                return;
            }

            baseColor = spriteRenderer.color;
            flashUntil = Time.time + flashDuration * VisualQualityProfileRuntime.EffectIntensity;
        }
    }
}
