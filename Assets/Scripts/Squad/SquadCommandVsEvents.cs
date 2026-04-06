using Unity.VisualScripting;
using UnityEngine;

namespace Breach.Squad
{
    public static class SquadCommandVsEvents
    {
        public const string MoveIssued = "breach.squad.command.move";
        public const string HoldIssued = "breach.squad.command.hold";
        public const string FollowIssued = "breach.squad.command.follow";
        public const string AttackIssued = "breach.squad.command.attack";

        public static void RaiseMove(GameObject sender, Vector3 worldPoint)
        {
            EventBus.Trigger(EventHooks.Custom, sender, new SquadCommandPayload(MoveIssued, worldPoint, null));
        }

        public static void RaiseHold(GameObject sender)
        {
            EventBus.Trigger(EventHooks.Custom, sender, new SquadCommandPayload(HoldIssued, Vector3.zero, null));
        }

        public static void RaiseFollow(GameObject sender, Transform target)
        {
            EventBus.Trigger(EventHooks.Custom, sender, new SquadCommandPayload(FollowIssued, target != null ? target.position : Vector3.zero, target));
        }

        public static void RaiseAttack(GameObject sender, Transform target)
        {
            EventBus.Trigger(EventHooks.Custom, sender, new SquadCommandPayload(AttackIssued, target != null ? target.position : Vector3.zero, target));
        }
    }

    public readonly struct SquadCommandPayload
    {
        public SquadCommandPayload(string eventKey, Vector3 worldPosition, Transform target)
        {
            EventKey = eventKey;
            WorldPosition = worldPosition;
            Target = target;
        }

        public string EventKey { get; }
        public Vector3 WorldPosition { get; }
        public Transform Target { get; }
    }
}
