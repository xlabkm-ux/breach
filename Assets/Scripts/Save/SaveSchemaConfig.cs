using UnityEngine;

namespace Breach.Save
{
    [CreateAssetMenu(menuName = "Breach/Save/Schema Config", fileName = "SaveSchemaConfig")]
    public sealed class SaveSchemaConfig : ScriptableObject
    {
        public int schemaVersion = 1;
    }
}
