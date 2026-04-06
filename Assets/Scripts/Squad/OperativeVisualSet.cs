using UnityEngine;

namespace Breach.Squad
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class OperativeVisualSet : MonoBehaviour
    {
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private Color selectedTint = new Color(0.85f, 1f, 0.85f, 1f);
        [SerializeField] private Color idleTint = Color.white;

        private SpriteRenderer spriteRenderer;
        private OperativeMember operativeMember;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            operativeMember = GetComponent<OperativeMember>();
            if (defaultSprite != null)
            {
                spriteRenderer.sprite = defaultSprite;
            }
        }

        private void Update()
        {
            if (spriteRenderer == null || operativeMember == null)
            {
                return;
            }

            spriteRenderer.color = operativeMember.IsSelected ? selectedTint : idleTint;
        }
    }
}
