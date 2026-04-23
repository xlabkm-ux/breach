using UnityEngine;
using System.Collections.Generic;

namespace TacticalBreach.AI
{
    public sealed class AISensor : MonoBehaviour
    {
        [Header("Vision Settings")]
        public float viewRadius = 10f;
        [Range(0, 360)] public float viewAngle = 90f;
        public LayerMask targetMask;
        public LayerMask obstacleMask;

        [Header("Hearing Settings")]
        public float hearingRadius = 12f;

        public List<Transform> visibleTargets = new List<Transform>();

        private void Start()
        {
            // Regularly scan for targets to save performance
            InvokeRepeating(nameof(FindVisibleTargets), 0.2f, 0.2f);
        }

        private void FindVisibleTargets()
        {
            visibleTargets.Clear();
            Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.up, dirToTarget) < viewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, target.position);

                    // Raycast for obstacles (Walls)
                    if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                    {
                        visibleTargets.Add(target);
                        Debug.Log($"Target {target.name} detected by {name}!");
                    }
                }
            }
        }

        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.z;
            }
            return new Vector3(-Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
        }
    }
}
