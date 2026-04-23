using UnityEditor;
using UnityEngine;
using System.IO;

public static class ImageResizer
{
    [MenuItem("TacticalBreach/Resize Assets to 128")]
    public static void ResizeAll()
    {
        string[] textures = {
            "Assets/Resources/Graphics/Textures/Tex_Floor_Tactical.png",
            "Assets/Resources/Graphics/Textures/Tex_Wall_Tactical.png",
            "Assets/Resources/Graphics/Textures/Tex_Door_Tactical.png",
            "Assets/Resources/Graphics/Textures/Tex_Cover_Tactical.png",
            "Assets/Resources/Graphics/Icons/Icon_Operative_Tactical.png",
            "Assets/Resources/Graphics/Icons/Icon_Enemy_Tactical.png",
            "Assets/Resources/Graphics/Icons/Icon_Hostage_Tactical.png"
        };

        foreach (var path in textures)
        {
            ResizeTexture(path, 128, 128);
        }
        
        AssetDatabase.Refresh();
        Debug.Log("All tactical assets resized to 128x128.");
    }

    private static void ResizeTexture(string path, int width, int height)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        if (!tex.LoadImage(fileData)) return;

        // Create temporary RenderTexture
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        RenderTexture.active = rt;
        
        // Copy and resize
        Graphics.Blit(tex, rt);
        Texture2D newTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        newTex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        newTex.Apply();
        
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        // Save back to disk
        byte[] bytes = newTex.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        
        Object.DestroyImmediate(tex);
        Object.DestroyImmediate(newTex);
        
        Debug.Log($"Resized: {path}");
    }
}
