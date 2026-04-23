using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TacticalBreach.Editor
{
    public static class Day1VisualPolisher
    {
        [MenuItem("TacticalBreach/Final Visual Polish")]
        public static void Polish()
        {
            TacticalTransparencyRemover.CleanAll();
            
            FixTextureImport("Assets/Resources/Graphics/Textures/Tex_Door_Tactical.png", true);
            FixTextureImport("Assets/Resources/Graphics/Textures/Tex_Cover_Tactical.png", true);
            FixTextureImport("Assets/Resources/Graphics/Textures/Tex_Operative_Tactical.png", true);
            FixTextureImport("Assets/Resources/Graphics/Textures/Tex_Enemy_Tactical.png", true);
            FixTextureImport("Assets/Resources/Graphics/Textures/Tex_Floor_Tactical.png", false);
            FixTextureImport("Assets/Resources/Graphics/Textures/Tex_Wall_Tactical.png", false);

            AdjustTilemapVisuals();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Visual Polish Complete. Transparency fixed and contrast adjusted.");
        }

        private static void FixTextureImport(string path, bool alphaIsTransparency)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return;
            if (path.Contains("Approved")) return;

            bool changed = false;
            if (importer.textureType != TextureImporterType.Sprite) { importer.textureType = TextureImporterType.Sprite; changed = true; }
            if (importer.spritePixelsPerUnit != 128f) { importer.spritePixelsPerUnit = 128f; changed = true; }
            if (importer.alphaIsTransparency != alphaIsTransparency) { importer.alphaIsTransparency = alphaIsTransparency; changed = true; }
            if (importer.alphaSource != TextureImporterAlphaSource.FromInput) { importer.alphaSource = TextureImporterAlphaSource.FromInput; changed = true; }
            if (importer.filterMode != FilterMode.Bilinear) { importer.filterMode = FilterMode.Bilinear; changed = true; }
            
            if (importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                changed = true;
            }

            if (importer.spritePivot != new Vector2(0.5f, 0.5f))
            {
                importer.spritePivot = new Vector2(0.5f, 0.5f);
                changed = true;
            }

            if (changed) importer.SaveAndReimport();
        }

        private static void AdjustTilemapVisuals()
        {
            var collisionMap = GameObject.Find("World_Collision")?.GetComponent<Tilemap>();
            if (collisionMap != null)
            {
                // Make walls slightly blue-ish or darker to distinguish from floor
                collisionMap.color = new Color(0.7f, 0.7f, 1f, 1f); 
            }

            var baseMap = GameObject.Find("World_Base")?.GetComponent<Tilemap>();
            if (baseMap != null)
            {
                baseMap.color = new Color(0.8f, 0.8f, 0.8f, 1f); // Slightly dim floor
            }
        }
    }
}
