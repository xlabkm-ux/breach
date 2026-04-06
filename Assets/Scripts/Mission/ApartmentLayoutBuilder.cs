using UnityEngine;
using UnityEngine.Tilemaps;

namespace Breach.Mission
{
    [ExecuteAlways]
    public sealed class ApartmentLayoutBuilder : MonoBehaviour
    {
        [SerializeField] private bool rebuildInEditor = true;
        [SerializeField] private bool rebuildOnPlay = true;

        private static Tile _floorTile;
        private static Tile _wallTile;
        private static Tile _roomTile;
        private static Tile _extractTile;

        private void OnEnable()
        {
            if (!Application.isPlaying && rebuildInEditor)
            {
                Rebuild();
            }
        }

        private void Start()
        {
            if (Application.isPlaying && rebuildOnPlay)
            {
                Rebuild();
            }
        }

        [ContextMenu("Rebuild Apartment Layout")]
        public void Rebuild()
        {
            EnsureTiles();

            var baseMap = FindTilemap("World_Base");
            var collisionMap = FindTilemap("World_Collision");
            var decorMap = FindTilemap("World_Decor");
            var interactablesMap = FindTilemap("World_Interactables");
            if (baseMap == null || collisionMap == null || decorMap == null || interactablesMap == null)
            {
                return;
            }

            baseMap.ClearAllTiles();
            collisionMap.ClearAllTiles();
            decorMap.ClearAllTiles();
            interactablesMap.ClearAllTiles();

            // Main apartment floor block.
            FillRect(baseMap, -12, -6, 24, 12, _floorTile);

            // Main walls.
            FillRectBorder(collisionMap, -12, -6, 24, 12, _wallTile);

            // Hostage room block.
            FillRect(baseMap, 4, -2, 7, 5, _roomTile);
            FillRectBorder(collisionMap, 4, -2, 7, 5, _wallTile);

            // Door opening to hostage room.
            ClearRect(collisionMap, 4, 0, 1, 1);

            // Two entry points.
            ClearRect(collisionMap, -12, 2, 1, 1);
            ClearRect(collisionMap, 0, -6, 1, 1);

            // Extraction zone marker.
            FillRect(interactablesMap, -10, -5, 3, 2, _extractTile);

            // Minimal decor anchors for readability.
            FillRect(decorMap, -6, 3, 2, 1, _roomTile);
            FillRect(decorMap, 0, 3, 2, 1, _roomTile);
            FillRect(decorMap, -2, -1, 1, 2, _roomTile);
        }

        private static Tilemap FindTilemap(string objectName)
        {
            var go = GameObject.Find(objectName);
            return go != null ? go.GetComponent<Tilemap>() : null;
        }

        private static void FillRect(Tilemap map, int x, int y, int w, int h, Tile tile)
        {
            for (var ix = x; ix < x + w; ix++)
            {
                for (var iy = y; iy < y + h; iy++)
                {
                    map.SetTile(new Vector3Int(ix, iy, 0), tile);
                }
            }
        }

        private static void FillRectBorder(Tilemap map, int x, int y, int w, int h, Tile tile)
        {
            for (var ix = x; ix < x + w; ix++)
            {
                map.SetTile(new Vector3Int(ix, y, 0), tile);
                map.SetTile(new Vector3Int(ix, y + h - 1, 0), tile);
            }

            for (var iy = y; iy < y + h; iy++)
            {
                map.SetTile(new Vector3Int(x, iy, 0), tile);
                map.SetTile(new Vector3Int(x + w - 1, iy, 0), tile);
            }
        }

        private static void ClearRect(Tilemap map, int x, int y, int w, int h)
        {
            for (var ix = x; ix < x + w; ix++)
            {
                for (var iy = y; iy < y + h; iy++)
                {
                    map.SetTile(new Vector3Int(ix, iy, 0), null);
                }
            }
        }

        private static void EnsureTiles()
        {
            _floorTile ??= BuildTile(BuildSolidSprite(new Color(0.25f, 0.29f, 0.34f)), Color.white);
            _wallTile ??= BuildTile(BuildSolidSprite(new Color(0.42f, 0.44f, 0.48f)), Color.white);
            _roomTile ??= BuildTile(BuildSolidSprite(new Color(0.33f, 0.37f, 0.42f)), Color.white);
            _extractTile ??= BuildTile(BuildSolidSprite(new Color(0.22f, 0.55f, 0.52f)), Color.white);
        }

        private static Tile BuildTile(Sprite sprite, Color color)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.color = color;
            return tile;
        }

        private static Sprite BuildSolidSprite(Color color)
        {
            var texture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            var pixels = new Color[32 * 32];
            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            texture.SetPixels(pixels);
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32f);
        }
    }
}
