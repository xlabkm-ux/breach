using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace TacticalBreach.Editor
{
    public static class BreachTestLaunchMenu
    {
        private const string BootstrapScenePath = "Assets/Scenes/VerticalSlice/Bootstrap.unity";
        private const string TestScenePath = "Assets/Scenes/VerticalSlice/VS01_Rescue.unity";
        private const string LastTestScenePath = "Assets/Scenes/VerticalSlice/VS01_Rescue.unity";

        [MenuItem("Breach/Testing/Open Bootstrap")]
        private static void OpenBootstrap()
        {
            OpenScene(BootstrapScenePath);
        }

        [MenuItem("Breach/Testing/Open Bootstrap", true)]
        private static bool CanOpenBootstrap()
        {
            return CanOpenScene(BootstrapScenePath);
        }

        [MenuItem("Breach/Testing/Open VS01 Rescue")]
        private static void OpenVs01Rescue()
        {
            OpenScene(TestScenePath);
        }

        [MenuItem("Breach/Testing/Open VS01 Rescue", true)]
        private static bool CanOpenVs01Rescue()
        {
            return CanOpenScene(TestScenePath);
        }

        [MenuItem("Breach/Testing/Open Last Test Scene")]
        private static void OpenLastTestScene()
        {
            OpenScene(LastTestScenePath);
        }

        [MenuItem("Breach/Testing/Open Last Test Scene", true)]
        private static bool CanOpenLastTestScene()
        {
            return CanOpenScene(LastTestScenePath);
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
