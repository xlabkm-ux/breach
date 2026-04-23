using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using TacticalBreach.AI;

namespace TacticalBreach.Editor
{
    public static class Day1CharacterSetup
    {
        [MenuItem("TacticalBreach/Setup Day 1 Characters")]
        public static void SetupCharacters()
        {
            SetupTexture("Assets/Resources/Graphics/Icons/Icon_Operative_Tactical.png");
            SetupTexture("Assets/Resources/Graphics/Icons/Icon_Enemy_Tactical.png");

            UpdateCharacterPrefab("Assets/Prefabs/Gameplay/Operatives/Operative_Player_A.prefab", "Assets/Resources/Graphics/Icons/Icon_Operative_Tactical.png");
            UpdateCharacterPrefab("Assets/Prefabs/Gameplay/Operatives/Operative_Player_B.prefab", "Assets/Resources/Graphics/Icons/Icon_Operative_Tactical.png");
            UpdateCharacterPrefab("Assets/Prefabs/Gameplay/Enemies/Enemy_Grunt.prefab", "Assets/Resources/Graphics/Icons/Icon_Enemy_Tactical.png");

            AssetDatabase.SaveAssets();
            Debug.Log("Day 1 Characters Setup Complete.");
        }

        private static void SetupTexture(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return;

            bool changed = false;
            if (importer.textureType != TextureImporterType.Sprite) { importer.textureType = TextureImporterType.Sprite; changed = true; }
            if (importer.spritePixelsPerUnit != 128f) { importer.spritePixelsPerUnit = 128f; changed = true; }
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

        private static void UpdateCharacterPrefab(string prefabPath, string spritePath)
        {
            if (!System.IO.File.Exists(prefabPath)) return;

            var root = PrefabUtility.LoadPrefabContents(prefabPath);
            if (root == null) return;

            try
            {
                // Physical position must be Z=0 for NavMesh stability
                root.transform.position = new Vector3(root.transform.position.x, root.transform.position.y, 0f);
                
                var renderer = root.GetComponentInChildren<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                    renderer.color = Color.white;
                    renderer.sortingOrder = 3; // Layer 3: Characters
                    // Visual offset only: 3 units closer to camera than Floor
                    renderer.transform.localPosition = new Vector3(0, 0, -3f);
                }

                var agent = root.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.enabled = false; // Disable during setup/gen
                    agent.updateRotation = false;
                    agent.updateUpAxis = false;
                    agent.radius = 0.5f; // Match 1x1 tile size
                    agent.height = 1f;
                    agent.speed = 3.5f;
                    agent.acceleration = 20f;
                    agent.angularSpeed = 0f;
                }

                // Add Red Marker for Player visibility (Centered)
                if (prefabPath.Contains("Operative"))
                {
                    var markerTransform = root.transform.Find("Marker");
                    GameObject markerGo = markerTransform != null ? markerTransform.gameObject : null;

                    if (markerGo == null)
                    {
                        markerGo = new GameObject("Marker");
                        markerGo.transform.SetParent(root.transform);
                    }
                    markerGo.transform.localPosition = Vector3.zero; // CENTERED

                    var markerRenderer = markerGo.GetComponent<SpriteRenderer>() ?? markerGo.AddComponent<SpriteRenderer>();
                    var knobSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
                    if (knobSprite != null) markerRenderer.sprite = knobSprite;
                    
                    markerRenderer.color = Color.red;
                    markerRenderer.sortingOrder = 100;
                    markerGo.transform.localScale = new Vector3(0.3f, 0.3f, 1f); // Make it a small center dot
                }

                EditorUtility.SetDirty(root);
                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
    }
}
