using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TacticalBreach.Editor
{
    public static class Day1SceneSetup
    {
        [MenuItem("TacticalBreach/Setup Day 1 Scene")]
        public static void SetupScene()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("Cannot setup scene in Play mode.");
                return;
            }

            // 1. Create Grid
            GameObject gridGo = GameObject.Find("Grid");
            if (gridGo == null)
            {
                gridGo = new GameObject("Grid");
                gridGo.AddComponent<Grid>();
                Debug.Log("Created Grid object.");
            }

            // 2. Create World_Base (Floor)
            GameObject baseGo = GameObject.Find("World_Base");
            if (baseGo == null)
            {
                baseGo = new GameObject("World_Base");
                baseGo.transform.SetParent(gridGo.transform);
                var tilemap = baseGo.AddComponent<Tilemap>();
                var renderer = baseGo.AddComponent<TilemapRenderer>();
                renderer.sortingOrder = 0; // Bottom layer
                Debug.Log("Created World_Base layer.");
            }

            // 3. Create World_Collision (Walls)
            GameObject collisionGo = GameObject.Find("World_Collision");
            if (collisionGo == null)
            {
                collisionGo = new GameObject("World_Collision");
                collisionGo.transform.SetParent(gridGo.transform);
                var tilemap = collisionGo.AddComponent<Tilemap>();
                var renderer = collisionGo.AddComponent<TilemapRenderer>();
                renderer.sortingOrder = 10; // Above floor

                // Add Physics
                var collider = collisionGo.AddComponent<TilemapCollider2D>();
                var composite = collisionGo.AddComponent<CompositeCollider2D>();
                var rb = collisionGo.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Static;
                }
                collider.compositeOperation = Collider2D.CompositeOperation.Merge;
                composite.geometryType = CompositeCollider2D.GeometryType.Polygons;

                Debug.Log("Created World_Collision layer with physics.");
            }

            // 4. Ensure Sorting Layers (Log warning as it's hard to create via script without SerializedObject)
            Debug.Log("Scene setup complete. Please ensure 'Floor' and 'Walls' sorting layers are configured manually if needed.");
            
            Selection.activeGameObject = gridGo;
        }
    }
}
