using UnityEngine;

namespace Breach.Mission
{
    public sealed class ZoneVisualMarker : MonoBehaviour
    {
        [SerializeField] private Color markerColor = Color.cyan;
        [SerializeField] private Vector2 size = new Vector2(1.2f, 1.2f);

        private void OnDrawGizmos()
        {
            Gizmos.color = markerColor;
            Gizmos.DrawWireCube(transform.position, new Vector3(size.x, size.y, 0f));
        }
    }
}
