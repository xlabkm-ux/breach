using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace TacticalBreach.Editor
{
    public static class TacticalBreachTestLaunchMenu
    {
        private const string BootstrapScenePath = "Assets/Scenes/VerticalSlice/Bootstrap.unity";
        private const string TestScenePath = "Assets/Scenes/VerticalSlice/VS01_Rescue.unity";

        // [MenuItem("TacticalBreach/Testing/Open Bootstrap")]
        private static void OpenBootstrap()
        {
            OpenScene(BootstrapScenePath);
        }

        // [MenuItem("TacticalBreach/Testing/Open Bootstrap", true)]
        private static bool CanOpenBootstrap()
        {
            return CanOpenScene(BootstrapScenePath);
        }

        // [MenuItem("TacticalBreach/Testing/Open VS01 Rescue")]
        private static void OpenVs01Rescue()
        {
            OpenScene(TestScenePath);
        }

        // [MenuItem("TacticalBreach/Testing/Open VS01 Rescue", true)]
        private static bool CanOpenVs01Rescue()
        {
            return CanOpenScene(TestScenePath);
        }

        private static void OpenScene(string scenePath)
        {
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }

        private static bool CanOpenScene(string scenePath)
        {
            return AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) != null;
        }
    }
}
