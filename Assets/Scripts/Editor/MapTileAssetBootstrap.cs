using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TacticalBreach.Editor
{
    public static class MapTileAssetBootstrap
    {
        private const string ResourceFolder = "Assets/Resources/MapTiles";

        private readonly struct TileSpec
        {
            public TileSpec(string assetName, string spritePath)
            {
                AssetName = assetName;
                SpritePath = spritePath;
            }

            public string AssetName { get; }
            public string SpritePath { get; }
        }

        private static readonly IReadOnlyList<TileSpec> Specs = new[]
        {
            new TileSpec("ApartmentFloor", "Assets/Resources/Graphics/Textures/Tex_Floor_Concrete_Dark.png"),
            new TileSpec("ApartmentRoomFloor", "Assets/Resources/Graphics/Textures/Tex_Floor_Concrete_Dark.png"),
            new TileSpec("ApartmentWall", "Assets/Resources/Graphics/Textures/Tex_Floor_Concrete_Dark.png"),
            new TileSpec("ApartmentBoundary", "Assets/Resources/Graphics/Textures/Tex_Floor_Concrete_Dark.png"),
            new TileSpec("ApartmentRoomBoundary", "Assets/Resources/Graphics/Textures/Tex_Floor_Concrete_Dark.png"),
            new TileSpec("ApartmentDoor", "Assets/Resources/Graphics/Textures/Tex_Floor_Concrete_Dark.png"),
            new TileSpec("ApartmentWindow", "Assets/Resources/Graphics/Textures/Tex_Floor_Concrete_Dark.png"),
            new TileSpec("ApartmentExtract", "Assets/Resources/Graphics/Textures/Tex_Floor_Concrete_Dark.png"),
        };

        [MenuItem("TacticalBreach/Tools/Generate Apartment Tiles")]
        public static void GenerateApartmentTiles()
        {
            EnsureFolder(ResourceFolder);

            foreach (var spec in Specs)
            {
                CreateOrUpdateTile(spec);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Apartment map tiles generated.");
        }

        [MenuItem("TacticalBreach/Tools/Bake VS01 Rescue Layout")]
        public static void BakeVs01RescueLayout()
        {
            const string scenePath = "Assets/Scenes/VerticalSlice/VS01_Rescue.unity";

            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var builder = Object.FindAnyObjectByType<TacticalBreach.Mission.ApartmentLayoutBuilder>();
            if (builder == null)
            {
                Debug.LogError("ApartmentLayoutBuilder was not found in VS01_Rescue.");
                EditorApplication.Exit(1);
                return;
            }

            builder.Rebuild();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveOpenScenes();
            Debug.Log("VS01_Rescue layout baked and saved.");
        }

        private static void CreateOrUpdateTile(TileSpec spec)
        {
            var tileAssetPath = Path.Combine(ResourceFolder, $"{spec.AssetName}.asset").Replace('\\', '/');
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spec.SpritePath);
            if (sprite == null)
            {
                Debug.LogError($"Missing sprite for tile generation: {spec.SpritePath}");
                return;
            }

            var litMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Core/Materials/ConcreteFloor_Lit.mat");

            var tile = AssetDatabase.LoadAssetAtPath<Tile>(tileAssetPath);
            if (tile == null)
            {
                tile = ScriptableObject.CreateInstance<Tile>();
                AssetDatabase.CreateAsset(tile, tileAssetPath);
            }

            tile.sprite = sprite;
            tile.color = Color.white;
            
            // Set the material if it exists to support 2D lighting.
            #if UNITY_URP
            // In a real project we'd set the material here, but standard Tile doesn't have a material field.
            // We use the GameObject factory or a custom Tile subclass if needed.
            // For now, we rely on the Tilemap Renderer using the correct material.
            #endif

            EditorUtility.SetDirty(tile);
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            const string root = "Assets";
            var relative = folderPath.Substring(root.Length + 1);
            var current = root;
            foreach (var part in relative.Split('/'))
            {
                var candidate = $"{current}/{part}";
                if (!AssetDatabase.IsValidFolder(candidate))
                {
                    AssetDatabase.CreateFolder(current, part);
                }

                current = candidate;
            }
        }
    }
}
