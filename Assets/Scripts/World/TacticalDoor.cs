using UnityEngine;

namespace TacticalBreach.World
{
    public sealed class TacticalDoor : MonoBehaviour
    {
        [SerializeField] private bool isOpen = false;
        [SerializeField] private float openAngle = 90f;
        
        private bool isTransitioning = false;

        public void Toggle()
        {
            isOpen = !isOpen;
            // Instead of rotation, we just hide the door (and its collider if needed)
            var renderer = GetComponentInChildren<SpriteRenderer>();
            if (renderer != null) renderer.enabled = !isOpen;
            
            // If the door has a collider on the same object, disable it too
            var coll = GetComponent<Collider2D>();
            if (coll != null) coll.enabled = !isOpen;
        }
    }
}
