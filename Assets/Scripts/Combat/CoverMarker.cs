using UnityEngine;

namespace Breach.Combat
{
    public enum CoverType
    {
        Half = 0,
        Full = 1
    }

    public sealed class CoverMarker : MonoBehaviour
    {
        [SerializeField] private CoverType coverType = CoverType.Half;
        [SerializeField] private float protectionValue = 0.5f;

        public CoverType Type => coverType;
        public float ProtectionValue => protectionValue;
    }
}
