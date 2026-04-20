using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class AutoBaker
{
    static AutoBaker()
    {
        EditorApplication.update += RunOnce;
    }

    private static void RunOnce()
    {
        EditorApplication.update -= RunOnce;
        
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;

        TileTextureGenerator.Generate();
        TextureTransparencyFixer.FixSettings();
        AssetDatabase.Refresh();
        Debug.Log("[CI] AutoBaker dynamically baked all textures.");
    }
}
