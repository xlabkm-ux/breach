using UnityEditor;
using UnityEngine;
using TacticalBreach.World;
using TacticalBreach.Combat;

namespace TacticalBreach.Editor
{
    public static class Day1PrefabSetup
    {
        private const string PrefabFolderPath = "Assets/Prefabs/World";

        [MenuItem("TacticalBreach/Setup Day 1 Prefabs")]
        public static void SetupPrefabs()
        {
            if (!AssetDatabase.IsValidFolder(PrefabFolderPath))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                    AssetDatabase.CreateFolder("Assets", "Prefabs");
                AssetDatabase.CreateFolder("Assets/Prefabs", "World");
            }

            CreateDoorPrefab();
            CreateCoverPrefab();

            AssetDatabase.SaveAssets();
            Debug.Log("Day 1 Prefabs Setup Complete.");
        }

        private static void CreateDoorPrefab()
        {
            string path = $"{PrefabFolderPath}/TacticalDoor.prefab";
            GameObject go = new GameObject("TacticalDoor");
            
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Graphics/Textures/Tex_Door_Tactical.png");
            renderer.sortingOrder = 20;

            var collider = go.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;

            go.AddComponent<TacticalDoor>();

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            Debug.Log($"Created Door prefab: {path}");
        }

        private static void CreateCoverPrefab()
        {
            string path = $"{PrefabFolderPath}/TacticalCover.prefab";
            GameObject go = new GameObject("TacticalCover");
            
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Graphics/Textures/Tex_Cover_Tactical.png");
            renderer.sortingOrder = 15;

            var collider = go.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.8f, 0.8f);

            go.AddComponent<CoverMarker>();

            // Try to set layer "Cover" if it exists
            int layer = LayerMask.NameToLayer("Cover");
            if (layer != -1) go.layer = layer;

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            Debug.Log($"Created Cover prefab: {path}");
        }
    }
}
