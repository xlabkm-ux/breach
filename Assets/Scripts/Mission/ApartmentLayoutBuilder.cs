using UnityEngine;
using UnityEngine.Tilemaps;

namespace TacticalBreach.Mission
{
    [ExecuteAlways]
    public sealed class ApartmentLayoutBuilder : MonoBehaviour
    {
        [SerializeField] private bool rebuildInEditor = false;
        [SerializeField] private bool rebuildOnPlay = false;
        public bool AutoRebuildEnabled => rebuildInEditor || rebuildOnPlay;

        private static Tile _floorTile;
        private static Tile _wallTile;
        private static Tile _roomTile;
        private static Tile _extractTile;
        private static Tile _corridorTile;
        private static Tile _kitchenTile;
        private static Tile _bathroomTile;

        private void OnEnable()
        {
            // Auto-rebuild disabled to prevent destructive scene wiping
        }

        private void Start()
        {
            // Auto-rebuild disabled to prevent destructive scene wiping
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

            int startX = -14, startY = -8;
            int width = 28, height = 16;

            // 1. Base floor layer
            FillRect(baseMap, startX, startY, width, height, _floorTile);

            // 2. Outer Walls
            FillRectBorder(collisionMap, startX, startY, width, height, _wallTile);

            // 3. Central Corridor
            FillRect(baseMap, -4, -2, 12, 4, _corridorTile);

            // 4. Kitchen (Top Left)
            FillRect(baseMap, -12, 2, 8, 6, _kitchenTile);
            FillRectBorder(collisionMap, -13, 2, 10, 7, _wallTile);
            ClearRect(collisionMap, -5, 4, 1, 1); // Door to corridor

            // 5. Bathroom (Bottom Left)
            FillRect(baseMap, -12, -7, 8, 5, _bathroomTile);
            FillRectBorder(collisionMap, -13, -8, 10, 7, _wallTile);
            ClearRect(collisionMap, -5, -4, 1, 1); // Door to corridor

            // 6. Hostage Room (Top Right)
            FillRect(baseMap, 8, 2, 6, 6, _roomTile);
            FillRectBorder(collisionMap, 7, 2, 8, 7, _wallTile);
            ClearRect(collisionMap, 7, 4, 1, 1); // Door to corridor

            // 7. Guard Room (Bottom Right)
            FillRect(baseMap, 8, -7, 6, 5, _roomTile);
            FillRectBorder(collisionMap, 7, -8, 8, 7, _wallTile);
            ClearRect(collisionMap, 7, -4, 1, 1); // Door to corridor

            // Clean up overlaps in main corridor
            ClearRect(collisionMap, -4, -2, 12, 4);

            // Two main entry points on outer walls
            ClearRect(collisionMap, startX, 0, 1, 2); // Left entrance
            ClearRect(collisionMap, startX + width - 1, 0, 1, 2); // Right entrance

            // Extraction zone
            FillRect(interactablesMap, startX - 2, -1, 2, 4, _extractTile);

            // Minimal decor anchors
            FillRect(decorMap, -11, 6, 2, 1, _roomTile); // Kitchen counter
            FillRect(decorMap, -11, -6, 1, 2, _roomTile); // Bathroom tub
            FillRect(decorMap, 11, 6, 1, 1, _roomTile); // Hostage chair
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
            _floorTile ??= Resources.Load<Tile>("MapTiles/ApartmentFloor") ?? BuildTile(BuildSolidSprite(new Color(0.25f, 0.29f, 0.34f)), Color.white);
            _wallTile ??= Resources.Load<Tile>("MapTiles/ApartmentWall") ?? BuildTile(BuildSolidSprite(new Color(0.42f, 0.44f, 0.48f)), Color.white);
            _roomTile ??= Resources.Load<Tile>("MapTiles/ApartmentRoomFloor") ?? BuildTile(BuildSolidSprite(new Color(0.33f, 0.37f, 0.42f)), Color.white);
            _extractTile ??= Resources.Load<Tile>("MapTiles/ApartmentExtract") ?? BuildTile(BuildSolidSprite(new Color(0.22f, 0.55f, 0.52f)), Color.white);
            
            _corridorTile ??= BuildTintedTile("MapTiles/ApartmentFloor", new Color(0.85f, 0.85f, 0.9f)) ?? BuildTile(BuildSolidSprite(new Color(0.28f, 0.30f, 0.35f)), Color.white);
            _kitchenTile ??= BuildTintedTile("MapTiles/ApartmentFloor", new Color(0.95f, 0.95f, 0.8f)) ?? BuildTile(BuildSolidSprite(new Color(0.35f, 0.35f, 0.30f)), Color.white);
            _bathroomTile ??= BuildTintedTile("MapTiles/ApartmentFloor", new Color(0.75f, 0.85f, 0.95f)) ?? BuildTile(BuildSolidSprite(new Color(0.25f, 0.35f, 0.40f)), Color.white);
        }

        private static Tile BuildTintedTile(string path, Color color)
        {
            var baseTile = Resources.Load<Tile>(path);
            if (baseTile == null) return null;
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = baseTile.sprite;
            tile.color = color;
            return tile;
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

