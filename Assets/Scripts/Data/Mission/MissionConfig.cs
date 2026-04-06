using UnityEngine;

namespace Breach.Data.Mission
{
    [CreateAssetMenu(menuName = "Breach/Mission/Mission Config", fileName = "MissionConfig")]
    public sealed class MissionConfig : ScriptableObject
    {
        public string missionId = "VS01_Rescue";
        public int squadMin = 2;
        public int squadMax = 4;
        public int enemiesMin = 2;
        public int enemiesMax = 4;
    }
}
