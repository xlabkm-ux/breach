using UnityEngine;

namespace Breach.Squad
{
    public enum OperativeCommandMode
    {
        None = 0,
        Hold = 1,
        MoveTo = 2,
        Follow = 3,
        AttackTarget = 4
    }

    public sealed class OperativeMember : MonoBehaviour
    {
        [SerializeField] private string operativeId = "operative";
        [SerializeField] private bool isSelected;
        [SerializeField] private float moveSpeed = 3f;

        private OperativeCommandMode commandMode = OperativeCommandMode.Hold;
        private Vector3 moveTarget;
        private Transform followTarget;
        private Transform attackTarget;

        public string OperativeId => operativeId;
        public bool IsSelected => isSelected;
        public OperativeCommandMode CommandMode => commandMode;

        private void Update()
        {
            if (commandMode == OperativeCommandMode.Hold)
            {
                return;
            }

            if (commandMode == OperativeCommandMode.Follow && followTarget != null)
            {
                moveTarget = followTarget.position;
            }
            else if (commandMode == OperativeCommandMode.AttackTarget && attackTarget != null)
            {
                moveTarget = attackTarget.position;
            }

            var next = Vector3.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);
            transform.position = new Vector3(next.x, next.y, transform.position.z);
        }

        public void SetSelectedState(bool selected)
        {
            isSelected = selected;
        }

        public void IssueHold()
        {
            commandMode = OperativeCommandMode.Hold;
        }

        public void IssueMove(Vector3 worldTarget)
        {
            moveTarget = new Vector3(worldTarget.x, worldTarget.y, transform.position.z);
            commandMode = OperativeCommandMode.MoveTo;
            followTarget = null;
            attackTarget = null;
        }

        public void IssueFollow(Transform target)
        {
            if (target == null)
            {
                return;
            }

            followTarget = target;
            moveTarget = target.position;
            commandMode = OperativeCommandMode.Follow;
            attackTarget = null;
        }

        public void IssueAttackTarget(Transform target)
        {
            if (target == null)
            {
                return;
            }

            attackTarget = target;
            moveTarget = target.position;
            commandMode = OperativeCommandMode.AttackTarget;
            followTarget = null;
        }
    }
}
