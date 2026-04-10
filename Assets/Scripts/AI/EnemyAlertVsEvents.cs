using UnityEngine;
#if UNITY_VISUAL_SCRIPTING
using Unity.VisualScripting;
#endif

namespace Breach.AI
{
    public static class EnemyAlertVsEvents
    {
        public const string Idle = "breach.enemy.alert.idle";
        public const string Suspicious = "breach.enemy.alert.suspicious";
        public const string Alert = "breach.enemy.alert.alert";

        public static void Raise(EnemyAlertState state, GameObject sender)
        {
            var eventName = state switch
            {
                EnemyAlertState.Alert => Alert,
                EnemyAlertState.Suspicious => Suspicious,
                _ => Idle
            };

#if UNITY_VISUAL_SCRIPTING
            EventBus.Trigger(EventHooks.Custom, sender, new CustomEventArgs(eventName));
#endif
        }
    }
}
