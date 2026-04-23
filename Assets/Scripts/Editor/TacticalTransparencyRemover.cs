using UnityEditor;
using UnityEngine;
using System.IO;

namespace TacticalBreach.Editor
{
    public static class TacticalTransparencyRemover
    {
        [MenuItem("TacticalBreach/Clean Texture Transparency")]
        public static void CleanAll()
        {
            CleanTexture("Assets/Resources/Graphics/Textures/Tex_Door_Tactical.png");
            CleanTexture("Assets/Resources/Graphics/Textures/Tex_Cover_Tactical.png");
            CleanTexture("Assets/Resources/Graphics/Textures/Tex_Operative_Tactical.png");
            CleanTexture("Assets/Resources/Graphics/Textures/Tex_Enemy_Tactical.png");
            
            AssetDatabase.Refresh();
            Debug.Log("Transparency Cleaning Complete.");
        }

        private static void CleanTexture(string assetPath)
        {
            // 1. Make texture readable
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null) return;

            bool wasReadable = importer.isReadable;
            var oldSettings = importer.textureCompression;
            
            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();

            // 2. Process pixels
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture == null) return;

            var pixels = texture.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                // If pixel is light (background), make it transparent
                if (pixels[i].r > 0.85f && pixels[i].g > 0.85f && pixels[i].b > 0.85f)
                {
                    pixels[i] = new Color(0, 0, 0, 0);
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            // 3. Save as PNG
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(Path.GetFullPath(assetPath), bytes);

            // 4. Restore importer settings and ensure alpha
            importer.isReadable = wasReadable;
            importer.textureCompression = oldSettings;
            importer.alphaIsTransparency = true;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.SaveAndReimport();
        }
    }
}
