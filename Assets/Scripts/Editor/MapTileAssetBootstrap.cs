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
            new TileSpec("ApartmentFloor", "Assets/_Core/Graphics/Tiles/Kenney/tile_10.png"),
            new TileSpec("ApartmentRoomFloor", "Assets/_Core/Graphics/Tiles/Kenney/tile_20.png"),
            new TileSpec("ApartmentWall", "Assets/_Core/Graphics/Tiles/Kenney/tile_100.png"),
            new TileSpec("ApartmentBoundary", "Assets/_Core/Graphics/Tiles/Kenney/tile_60.png"),
            new TileSpec("ApartmentRoomBoundary", "Assets/_Core/Graphics/Tiles/Kenney/tile_40.png"),
            new TileSpec("ApartmentDoor", "Assets/_Core/Graphics/Tiles/Kenney/tile_130.png"),
            new TileSpec("ApartmentWindow", "Assets/_Core/Graphics/Tiles/Kenney/tile_133.png"),
            new TileSpec("ApartmentExtract", "Assets/_Core/Graphics/Tiles/Kenney/tile_131.png"),
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

            var tile = AssetDatabase.LoadAssetAtPath<Tile>(tileAssetPath);
            if (tile == null)
            {
                tile = ScriptableObject.CreateInstance<Tile>();
                AssetDatabase.CreateAsset(tile, tileAssetPath);
            }

            tile.sprite = sprite;
            tile.color = Color.white;
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
