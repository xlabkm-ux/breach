using System.Collections.Generic;
using UnityEngine;

namespace Breach.Squad
{
    public sealed class ActiveOperativeSwitchService : MonoBehaviour
    {
        [SerializeField] private List<OperativeMember> operatives = new();
        [SerializeField] private int activeIndex;

        public int ActiveIndex => activeIndex;
        public OperativeMember ActiveOperative => IsValidIndex(activeIndex) ? operatives[activeIndex] : null;

        private void Awake()
        {
            if (operatives.Count == 0)
            {
                operatives.AddRange(FindObjectsByType<OperativeMember>(FindObjectsSortMode.None));
            }

            if (operatives.Count == 0)
            {
                return;
            }

            if (activeIndex < 0 || activeIndex >= operatives.Count)
            {
                activeIndex = 0;
            }

            ApplyActiveIndex(activeIndex);
        }

        private void Update()
        {
            if (operatives.Count < 2)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SwitchToNext();
            }
        }

        public void SwitchToNext()
        {
            if (operatives.Count == 0)
            {
                return;
            }

            activeIndex = (activeIndex + 1) % operatives.Count;
            ApplyActiveIndex(activeIndex);
        }

        public OperativeMember GetSecondaryOperative()
        {
            if (operatives.Count < 2)
            {
                return null;
            }

            var secondaryIndex = activeIndex == 0 ? 1 : 0;
            return IsValidIndex(secondaryIndex) ? operatives[secondaryIndex] : null;
        }

        private void ApplyActiveIndex(int index)
        {
            for (var i = 0; i < operatives.Count; i++)
            {
                if (operatives[i] != null)
                {
                    operatives[i].SetSelectedState(i == index);
                }
            }
        }

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < operatives.Count;
        }
    }
}
