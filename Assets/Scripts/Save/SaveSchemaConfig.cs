using UnityEngine;

namespace TacticalBreach.Save
{
    [CreateAssetMenu(menuName = "Breach/Save/Schema Config", fileName = "SaveSchemaConfig")]
    public sealed class SaveSchemaConfig : ScriptableObject
    {
        public int schemaVersion = 1;
    }
}
