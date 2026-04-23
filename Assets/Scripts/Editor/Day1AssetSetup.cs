using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TacticalBreach.Editor
{
    public static class Day1AssetSetup
    {
        [MenuItem("TacticalBreach/Setup Day 1 Assets")]
        public static void SetupAssets()
        {
            string floorPath = "Assets/Resources/Graphics/Textures/Tex_Floor_Tactical.png";
            string wallPath = "Assets/Resources/Graphics/Textures/Tex_Wall_Tactical.png";
            string doorPath = "Assets/Resources/Graphics/Textures/Tex_Door_Tactical.png";
            string coverPath = "Assets/Resources/Graphics/Textures/Tex_Cover_Tactical.png";

            SetupTexture(ref floorPath);
            SetupTexture(ref wallPath);
            SetupTexture(ref doorPath);
            SetupTexture(ref coverPath);

            CreateTile(floorPath, "Assets/Resources/MapTiles/TacticalFloor.asset", Tile.ColliderType.None);
            CreateTile(wallPath, "Assets/Resources/MapTiles/TacticalWall.asset", Tile.ColliderType.Grid);

            // Update Door Prefab directly
            var doorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/World/TacticalDoor.prefab");
            if (doorPrefab != null)
            {
                var sr = doorPrefab.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(doorPath);
                    sr.color = Color.white;
                    EditorUtility.SetDirty(doorPrefab);
                }
            }

            var coverPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/World/TacticalCover.prefab");
            if (coverPrefab != null)
            {
                var sr = coverPrefab.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(coverPath);
                    sr.color = Color.white;
                    EditorUtility.SetDirty(coverPrefab);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Day 1 Assets Setup Complete. PPU set to 128, Tiles updated with new textures.");
        }

        private static void SetupTexture(ref string path)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                // Try to find it in Approved folder instead
                string approvedPath = path.Replace("Textures/", "Approved/");
                importer = AssetImporter.GetAtPath(approvedPath) as TextureImporter;
                if (importer != null)
                {
                    path = approvedPath; // Update path for subsequent logic
                }
                else
                {
                    Debug.LogWarning($"Texture not found at path: {path} (checked Approved as well)");
                    return;
                }
            }

            if (path.Contains("Approved"))
            {
                Debug.Log($"[Locked] Skipping approved asset: {path}");
                return;
            }

            bool changed = false;
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                changed = true;
            }
            if (importer.spritePixelsPerUnit != 128f)
            {
                importer.spritePixelsPerUnit = 128f;
                changed = true;
            }
            if (importer.filterMode != FilterMode.Bilinear)
            {
                importer.filterMode = FilterMode.Bilinear;
                changed = true;
            }

            // Ensure Single mode and Center Pivot
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

            if (changed)
            {
                importer.SaveAndReimport();
                Debug.Log($"Configured texture with Center Pivot: {path}");
            }
        }

        private static void CreateTile(string texPath, string tilePath, Tile.ColliderType colliderType)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(texPath);
            if (sprite == null)
            {
                Debug.LogError($"Sprite not found at {texPath}. Cannot create tile.");
                return;
            }

            var tile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
            if (tile == null)
            {
                tile = ScriptableObject.CreateInstance<Tile>();
                AssetDatabase.CreateAsset(tile, tilePath);
            }

            tile.sprite = sprite;
            tile.colliderType = colliderType;
            tile.color = Color.white;

            EditorUtility.SetDirty(tile);
            AssetDatabase.SaveAssets();
            Debug.Log($"Updated tile: {tilePath}");
        }
    }
}
