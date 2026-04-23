using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SceneHierarchyLogger
{
    // [MenuItem("TacticalBreach/Debug/Log Scene Hierarchy")]
    public static void LogHierarchy()
    {
        const string scenePath = "Assets/Scenes/VerticalSlice/VS01_Rescue.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        
        Debug.Log($"--- Hierarchy for {scene.name} ---");
        var allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
        foreach (var go in allObjects)
        {
            if (go.transform.parent == null)
            {
                LogRecursive(go, 0);
            }
        }
    }

    private static void LogRecursive(GameObject go, int depth)
    {
        var indent = new string('-', depth);
        var components = go.GetComponents<Component>();
        var compList = "";
        foreach (var c in components)
        {
            if (c == null) continue;
            compList += $" [{c.GetType().Name}]";
        }
        Debug.Log($"{indent} {go.name} {compList}");
        for (int i = 0; i < go.transform.childCount; i++)
        {
            LogRecursive(go.transform.GetChild(i).gameObject, depth + 1);
        }
    }
}
