using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
        private static Sprite _baseSprite;
        private static Sprite _floorSprite;
        private static Sprite _wallSprite;
        private static Sprite _roomSprite;
        private static Sprite _extractSprite;

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
            if (_floorSprite == null || _wallSprite == null || _roomSprite == null || _extractSprite == null)
            {
                _floorSprite = TryLoadSprite("Assets/_Core/Graphics/Tiles/Kenney/tile_08.png");
                _wallSprite = TryLoadSprite("Assets/_Core/Graphics/Tiles/Kenney/tile_11.png");
                _roomSprite = TryLoadSprite("Assets/_Core/Graphics/Tiles/Kenney/tile_24.png");
                _extractSprite = TryLoadSprite("Assets/_Core/Graphics/Tiles/Kenney/tile_57.png");
            }

            if (_baseSprite == null && (_floorSprite == null || _wallSprite == null || _roomSprite == null || _extractSprite == null))
            {
                var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                texture.SetPixel(0, 0, Color.white);
                texture.Apply();
                _baseSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
            }

            _floorTile ??= BuildTile(_floorSprite ?? _baseSprite, new Color(0.9f, 0.9f, 0.95f));
            _wallTile ??= BuildTile(_wallSprite ?? _baseSprite, new Color(0.9f, 0.9f, 0.9f));
            _roomTile ??= BuildTile(_roomSprite ?? _baseSprite, new Color(0.95f, 0.95f, 0.95f));
            _extractTile ??= BuildTile(_extractSprite ?? _baseSprite, new Color(0.8f, 1f, 0.95f));
        }

        private static Tile BuildTile(Sprite sprite, Color color)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.color = color;
            return tile;
        }

        private static Sprite TryLoadSprite(string path)
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
#else
            return null;
#endif
        }
    }
}
