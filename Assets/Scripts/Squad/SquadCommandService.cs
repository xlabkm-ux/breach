using UnityEngine;

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

            if (Input.GetKeyDown(KeyCode.H))
            {
                IssueHoldToSecondary();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                IssueFollowToSecondary();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                IssueMoveToSecondaryAtCursor();
            }

            if (Input.GetKeyDown(KeyCode.T))
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

            var screen = Input.mousePosition;
            var world = cachedCamera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, Mathf.Abs(cachedCamera.transform.position.z)));
            secondary.IssueMove(world);
        }

        public void IssueHoldToSecondary()
        {
            var secondary = switchService.GetSecondaryOperative();
            secondary?.IssueHold();
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

            var screen = Input.mousePosition;
            var world = cachedCamera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, Mathf.Abs(cachedCamera.transform.position.z)));
            var hit = Physics2D.OverlapPoint(new Vector2(world.x, world.y));
            if (hit == null)
            {
                return;
            }

            secondary.IssueAttackTarget(hit.transform);
        }
    }
}
