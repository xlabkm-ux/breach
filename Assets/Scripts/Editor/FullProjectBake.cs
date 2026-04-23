using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class FullProjectBake
{
    // [MenuItem("TacticalBreach/CI/Full Bake")]
    public static void RunFullBake()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("[CI] Cannot run Full Bake while in Play Mode. Please stop the game first!");
            return;
        }

        Debug.Log("[CI] Starting Full Project Bake...");
        
        // 1. Fix textures
        TileTextureGenerator.Generate();
        TextureTransparencyFixer.FixSettings();
        
        // 2. Generate tiles
        AssetDatabase.Refresh();
        TacticalBreach.Editor.MapTileAssetBootstrap.GenerateApartmentTiles();
        
        // 3. Open Scene and Rebuild
        AssetDatabase.Refresh();
        var scenePath = "Assets/Scenes/VerticalSlice/VS01_Rescue.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        
        var builder = Object.FindAnyObjectByType<TacticalBreach.Mission.ApartmentLayoutBuilder>();
        if (builder != null)
        {
            builder.Rebuild();
            Debug.Log("[CI] Apartment layout rebuilt.");
        }
        else
        {
            Debug.LogError("[CI] ApartmentLayoutBuilder not found in scene!");
        }

        // 3.5 Bake NavMesh (Note: UnityEditor.AI.NavMeshBuilder is deprecated, moving to NavMeshSurface in the future)
        #pragma warning disable CS0618
        UnityEditor.AI.NavMeshBuilder.BuildNavMesh();
        #pragma warning restore CS0618
        Debug.Log("[CI] NavMesh baked.");

        // 4. Setup environment (Lights/Post)
        // We'll call the logic from GraphicsBaker if possible, or just copy it.
        // Since GraphicsBaker has errors, let's just do it here.
        SetupTacticalEnvironment();

        // 5. Save
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        
        Debug.Log("[CI] Full Project Bake COMPLETED.");
    }

    private static void SetupTacticalEnvironment()
    {
        // Lit material for tilemaps
        var litMatGuid = "51bc4a5b15d239941a796041609cd381"; 
        var litMatPath = AssetDatabase.GUIDToAssetPath(litMatGuid);
        var litMat = AssetDatabase.LoadAssetAtPath<Material>(litMatPath);

        if (litMat != null)
        {
            var renderers = Object.FindObjectsByType<UnityEngine.Tilemaps.TilemapRenderer>(FindObjectsInactive.Include);
            foreach (var r in renderers) { r.sharedMaterial = litMat; }
        }

        // Global Light
        var lights = Object.FindObjectsByType<UnityEngine.Rendering.Universal.Light2D>(FindObjectsInactive.Include);
        foreach (var l in lights)
        {
            if (l.lightType == UnityEngine.Rendering.Universal.Light2D.LightType.Global && l.gameObject.name != "GlobalLight2D")
            {
                Object.DestroyImmediate(l.gameObject);
            }
        }

        var lightGo = GameObject.Find("GlobalLight2D") ?? new GameObject("GlobalLight2D");
        var light = lightGo.GetComponent<UnityEngine.Rendering.Universal.Light2D>() ?? lightGo.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
        light.lightType = UnityEngine.Rendering.Universal.Light2D.LightType.Global;
        light.intensity = 1.25f;

        Debug.Log("[CI] Environment setup done.");
    }
}
