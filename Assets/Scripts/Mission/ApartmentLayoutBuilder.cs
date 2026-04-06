using UnityEngine;
using UnityEngine.Tilemaps;

namespace Breach.Mission
{
    [ExecuteAlways]
    public sealed class ApartmentLayoutBuilder : MonoBehaviour
    {
        [SerializeField] private bool rebuildInEditor = true;

        private static Tile _floorTile;
        private static Tile _wallTile;
        private static Tile _roomTile;
        private static Tile _extractTile;
        private static Sprite _baseSprite;

        private void OnEnable()
        {
            if (!Application.isPlaying && rebuildInEditor)
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
            if (_baseSprite == null)
            {
                var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                texture.SetPixel(0, 0, Color.white);
                texture.Apply();
                _baseSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
            }

            _floorTile ??= BuildTile(new Color(0.18f, 0.2f, 0.23f));
            _wallTile ??= BuildTile(new Color(0.35f, 0.35f, 0.38f));
            _roomTile ??= BuildTile(new Color(0.22f, 0.28f, 0.22f));
            _extractTile ??= BuildTile(new Color(0.1f, 0.45f, 0.55f));
        }

        private static Tile BuildTile(Color color)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = _baseSprite;
            tile.color = color;
            return tile;
        }
    }
}
