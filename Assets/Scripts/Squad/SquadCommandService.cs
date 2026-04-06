using UnityEngine;
using Breach.Core;

namespace Breach.Squad
{
    public sealed class SquadCommandService : MonoBehaviour
    {
        [SerializeField] private ActiveOperativeSwitchService switchService;

        private Camera cachedCamera;

        private void Awake()
        {
            cachedCamera = Camera.main;
            if (switchService == null)
            {
                switchService = GetComponent<ActiveOperativeSwitchService>();
            }
        }

        private void Update()
        {
            if (switchService == null)
            {
                return;
            }

            if (InputCompat.GetKeyDown(KeyCode.H))
            {
                IssueHoldToSecondary();
            }

            if (InputCompat.GetKeyDown(KeyCode.F))
            {
                IssueFollowToSecondary();
            }

            if (InputCompat.GetKeyDown(KeyCode.M))
            {
                IssueMoveToSecondaryAtCursor();
            }

            if (InputCompat.GetKeyDown(KeyCode.T))
            {
                IssueAttackTargetToSecondaryAtCursor();
            }
        }

        public void IssueMoveToSecondaryAtCursor()
        {
            var secondary = switchService.GetSecondaryOperative();
            if (secondary == null)
            {
                return;
            }

            if (cachedCamera == null)
            {
                cachedCamera = Camera.main;
                if (cachedCamera == null)
                {
                    return;
                }
            }

            var screen = InputCompat.MousePosition;
            var world = cachedCamera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, Mathf.Abs(cachedCamera.transform.position.z)));
            secondary.IssueMove(world);
            SquadCommandVsEvents.RaiseMove(gameObject, world);
        }

        public void IssueHoldToSecondary()
        {
            var secondary = switchService.GetSecondaryOperative();
            secondary?.IssueHold();
            SquadCommandVsEvents.RaiseHold(gameObject);
        }

        public void IssueFollowToSecondary()
        {
            var active = switchService.ActiveOperative;
            var secondary = switchService.GetSecondaryOperative();
            if (active == null || secondary == null)
            {
                return;
            }

            secondary.IssueFollow(active.transform);
            SquadCommandVsEvents.RaiseFollow(gameObject, active.transform);
        }

        public void IssueAttackTargetToSecondaryAtCursor()
        {
            var secondary = switchService.GetSecondaryOperative();
            if (secondary == null)
            {
                return;
            }

            if (cachedCamera == null)
            {
                cachedCamera = Camera.main;
                if (cachedCamera == null)
                {
                    return;
                }
            }

            var screen = InputCompat.MousePosition;
            var world = cachedCamera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, Mathf.Abs(cachedCamera.transform.position.z)));
            var hit = Physics2D.OverlapPoint(new Vector2(world.x, world.y));
            if (hit == null)
            {
                return;
            }

            secondary.IssueAttackTarget(hit.transform);
            SquadCommandVsEvents.RaiseAttack(gameObject, hit.transform);
        }
    }
}

