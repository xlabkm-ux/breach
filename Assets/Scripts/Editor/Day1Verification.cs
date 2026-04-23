using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

namespace TacticalBreach.Editor
{
    public static class Day1Verification
    {
        [MenuItem("TacticalBreach/Verify Day 1 Status")]
        public static void Verify()
        {
            Debug.Log("=== VERIFICATION: DAY 1 FULL STATUS ===");

            bool step1Ok = VerifyHierarchy();
            bool step2Ok = VerifyAssets();
            bool step3Ok = VerifyPrefabs();

            if (step1Ok && step2Ok && step3Ok)
            {
                Debug.Log("=== ALL DAY 1 REQUIREMENTS VERIFIED: OK ✅ ===");
            }
            else
            {
                Debug.LogWarning("=== VERIFICATION FAILED. PLEASE CHECK CONSOLE FOR ERRORS. ===");
            }
        }

        private static bool VerifyHierarchy()
        {
            bool ok = true;
            ok &= CheckObject("Grid", typeof(Grid));
            ok &= CheckObject("World_Base", typeof(Tilemap));
            ok &= CheckObject("World_Collision", typeof(Tilemap));
            ok &= CheckObject("NavMeshManager", null);
            ok &= CheckObject("GlobalLight2D", null);

            var collisionGo = GameObject.Find("World_Collision");
            if (collisionGo != null)
            {
                var composite = collisionGo.GetComponent<CompositeCollider2D>();
                if (composite == null) { Debug.LogError("[Hierarchy] World_Collision missing CompositeCollider2D."); ok = false; }
                else if (composite.geometryType != CompositeCollider2D.GeometryType.Polygons) { Debug.LogError("[Hierarchy] World_Collision CompositeCollider2D should use Polygons."); ok = false; }
            }

            if (ok) Debug.Log("[Step 1] Scene Hierarchy: OK ✅");
            return ok;
        }

        private static bool VerifyAssets()
        {
            bool ok = true;
            ok &= CheckTexture("Assets/Resources/Graphics/Textures/Tex_Floor_Tactical.png", 128);
            ok &= CheckTexture("Assets/Resources/Graphics/Textures/Tex_Wall_Tactical.png", 128);
            ok &= CheckTexture("Assets/Resources/Graphics/Icons/Icon_Operative_Tactical.png", 128);
            ok &= CheckTexture("Assets/Resources/Graphics/Icons/Icon_Enemy_Tactical.png", 128);
            
            ok &= CheckTile("Assets/Resources/MapTiles/TacticalFloor.asset", "Tex_Floor_Tactical");
            ok &= CheckTile("Assets/Resources/MapTiles/TacticalWall.asset", "Tex_Wall_Tactical");
            ok &= CheckTile("Assets/Resources/MapTiles/ApartmentWindow.asset", "Window");

            if (ok) Debug.Log("[Step 2] Assets Configuration: OK ✅");
            return ok;
        }

        private static bool VerifyPrefabs()
        {
            bool ok = true;
            ok &= CheckPrefab("Assets/Prefabs/World/TacticalDoor.prefab");
            ok &= CheckPrefab("Assets/Prefabs/World/TacticalCover.prefab");
            ok &= CheckPrefab("Assets/Prefabs/Gameplay/Operatives/Operative_Player_A.prefab");
            ok &= CheckPrefab("Assets/Prefabs/Gameplay/Enemies/Enemy_Grunt.prefab");

            if (ok) Debug.Log("[Step 3] Prefab Integrity: OK ✅");
            return ok;
        }

        private static bool CheckObject(string name, System.Type requiredComponent)
        {
            GameObject go = GameObject.Find(name);
            if (go == null) { Debug.LogError($"[Hierarchy] Missing object: {name}"); return false; }
            if (requiredComponent != null && go.GetComponent(requiredComponent) == null) { Debug.LogError($"[Hierarchy] Object {name} missing component: {requiredComponent.Name}"); return false; }
            return true;
        }

        private static bool CheckTexture(string path, int expectedPpu)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) { Debug.LogError($"[Assets] Texture not found: {path}"); return false; }
            
            bool ok = true;
            if (importer.spritePixelsPerUnit != expectedPpu) { Debug.LogError($"[Assets] Texture {path} has wrong PPU: {importer.spritePixelsPerUnit} (Expected {expectedPpu})"); ok = false; }
            
            if (importer.spritePivot != new Vector2(0.5f, 0.5f)) { Debug.LogError($"[Assets] Texture {path} has wrong Pivot: {importer.spritePivot} (Expected Center 0.5, 0.5)"); ok = false; }
            
            return ok;
        }

        private static bool CheckTile(string path, string expectedSpritePart)
        {
            var tile = AssetDatabase.LoadAssetAtPath<Tile>(path);
            if (tile == null) { Debug.LogError($"[Assets] Tile asset missing: {path}"); return false; }
            if (tile.sprite == null || (!string.IsNullOrEmpty(expectedSpritePart) && !tile.sprite.name.ToLower().Contains(expectedSpritePart.ToLower()))) { Debug.LogError($"[Assets] Tile {path} has wrong or missing sprite."); return false; }
            return true;
        }

        private static bool CheckPrefab(string path)
        {
            if (!File.Exists(path)) { Debug.LogError($"[Prefabs] Prefab missing at path: {path}"); return false; }
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go == null) { Debug.LogError($"[Prefabs] Failed to load prefab: {path}"); return false; }
            return true;
        }
    }
}
