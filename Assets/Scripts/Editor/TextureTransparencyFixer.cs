using UnityEditor;
using UnityEngine;
using System.IO;

public static class TextureTransparencyFixer
{
    // [MenuItem("TacticalBreach/Graphics/Fix Texture Settings")]
    public static void FixSettings()
    {
        AssetDatabase.Refresh(); // Ensure files generated previously exist in DB

        string[] paths = {
            "Assets/Resources/Graphics/Icons/Icon_Operative_Tactical.png",
            "Assets/Resources/Graphics/Icons/Icon_Enemy_Tactical.png",
            "Assets/Resources/Graphics/Icons/Icon_Hostage_Tactical.png",
            "Assets/Resources/Graphics/Textures/Tex_Floor_Concrete_Dark.png",
            "Assets/Resources/Graphics/Textures/Tex_Floor_Room_Dark.png",
            "Assets/Resources/Graphics/Textures/Tex_Wall_Concrete_Dark.png",
            "Assets/Resources/Graphics/Textures/Tex_Door_Dark.png",
            "Assets/Resources/Graphics/Textures/Tex_Window_Dark.png",
            "Assets/Resources/Graphics/Textures/Tex_Extract_Dark.png"
        };

        foreach (var path in paths)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                Debug.LogWarning($"Importer not found for {path}");
                continue;
            }

            bool changed = false;
            
            if (importer.textureType != TextureImporterType.Sprite) { importer.textureType = TextureImporterType.Sprite; changed = true; }
            if (Mathf.Abs(importer.spritePixelsPerUnit - 1024f) > 0.1f) { importer.spritePixelsPerUnit = 1024f; changed = true; }
            if (!importer.isReadable) { importer.isReadable = true; changed = true; }
            if (!importer.alphaIsTransparency) { importer.alphaIsTransparency = true; changed = true; }
            if (importer.mipmapEnabled) { importer.mipmapEnabled = false; changed = true; }
            if (importer.textureCompression != TextureImporterCompression.Uncompressed) { importer.textureCompression = TextureImporterCompression.Uncompressed; changed = true; }
            if (importer.filterMode != FilterMode.Bilinear) { importer.filterMode = FilterMode.Bilinear; changed = true; }
            if (importer.maxTextureSize != 2048) { importer.maxTextureSize = 2048; changed = true; }

            if (changed)
            {
                importer.SaveAndReimport();
            }
        }
        
        ProcessIconsAlpha();
        
        Debug.Log("Texture settings fixed.");
    }

    private static void ProcessIconsAlpha()
    {
        string[] iconPaths = new string[]
        {
            "Assets/Resources/Graphics/Icons/Icon_Enemy_Tactical.png",
            "Assets/Resources/Graphics/Icons/Icon_Hostage_Tactical.png",
            "Assets/Resources/Graphics/Icons/Icon_Operative_Tactical.png"
        };

        foreach (var path in iconPaths)
        {
            string fullPath = Path.Combine(Application.dataPath, path.Substring(7));
            if (!File.Exists(fullPath)) continue;

            byte[] bytes = File.ReadAllBytes(fullPath);
            Texture2D modTex = new Texture2D(2, 2);
            modTex.LoadImage(bytes);

            int width = modTex.width;
            int height = modTex.height;
            float cx = width / 2f;
            float cy = height / 2f;
            // The circle drawn by generate_image usually isn't perfect edges, so let's cut a slightly smaller circle to be safe.
            float radius = width / 2f - 4f; 

            Color[] pixels = modTex.GetPixels();
            bool modified = false;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    // Check if outside circle
                    if (dx * dx + dy * dy > radius * radius)
                    {
                        int idx = y * width + x;
                        if (pixels[idx].a > 0.05f) // Has alpha
                        {
                            pixels[idx] = new Color(0, 0, 0, 0);
                            modified = true;
                        }
                    }
                }
            }

            if (modified)
            {
                modTex.SetPixels(pixels);
                modTex.Apply();
                byte[] outBytes = modTex.EncodeToPNG();
                File.WriteAllBytes(fullPath, outBytes);
                Debug.Log($"[CI] Processed icon alpha mask: {path}");
            }
        }
        
        UnityEditor.AssetDatabase.Refresh();
        foreach (var path in iconPaths)
        {
            var importer = UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.TextureImporter;
            if (importer != null)
            {
                bool changed = false;
                if (importer.textureType != UnityEditor.TextureImporterType.Sprite) { importer.textureType = UnityEditor.TextureImporterType.Sprite; changed = true; }
                if (Mathf.Abs(importer.spritePixelsPerUnit - 1024f) > 0.1f) { importer.spritePixelsPerUnit = 1024f; changed = true; }
                if (importer.textureCompression != UnityEditor.TextureImporterCompression.Uncompressed) { importer.textureCompression = UnityEditor.TextureImporterCompression.Uncompressed; changed = true; }
                if (importer.filterMode != UnityEngine.FilterMode.Bilinear) { importer.filterMode = UnityEngine.FilterMode.Bilinear; changed = true; }
                if (importer.mipmapEnabled) { importer.mipmapEnabled = false; changed = true; }
                if (importer.maxTextureSize != 2048) { importer.maxTextureSize = 2048; changed = true; }
                
                if (changed)
                {
                    importer.SaveAndReimport();
                }
            }
        }
    }
}
