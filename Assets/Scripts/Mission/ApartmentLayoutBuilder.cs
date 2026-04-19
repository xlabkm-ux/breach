using UnityEngine;
using UnityEngine.Tilemaps;

namespace TacticalBreach.Mission
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
        private static Tile _boundaryTile;
        private static Tile _roomBoundaryTile;
        private static Tile _doorTile;
        private static Tile _windowTile;

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
            FillRectBorder(decorMap, -13, -7, 26, 14, _boundaryTile);
            PlaceWindowBand(decorMap, -8, 6, 2, _windowTile);
            PlaceWindowBand(decorMap, 0, 6, 2, _windowTile);
            PlaceWindowBand(decorMap, 8, 6, 2, _windowTile);

            // Hostage room block.
            FillRect(baseMap, 4, -2, 7, 5, _roomTile);
            FillRectBorder(collisionMap, 4, -2, 7, 5, _wallTile);
            FillRectBorder(decorMap, 4, -2, 7, 5, _roomBoundaryTile);

            // Door opening to hostage room.
            ClearRect(collisionMap, 4, 0, 1, 1);
            SetTile(decorMap, 4, 0, _doorTile);

            // Two entry points.
            ClearRect(collisionMap, -12, 2, 1, 1);
            ClearRect(collisionMap, 0, -6, 1, 1);
            SetTile(decorMap, -12, 2, _doorTile);
            SetTile(decorMap, 0, -6, _doorTile);

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

        private static void PlaceWindowBand(Tilemap map, int startX, int y, int length, Tile tile)
        {
            for (var ix = startX; ix < startX + length; ix++)
            {
                map.SetTile(new Vector3Int(ix, y, 0), tile);
            }
        }

        private static void SetTile(Tilemap map, int x, int y, Tile tile)
        {
            map.SetTile(new Vector3Int(x, y, 0), tile);
        }

        private static void EnsureTiles()
        {
            _floorTile ??= LoadTile("ApartmentFloor");
            _wallTile ??= LoadTile("ApartmentWall");
            _roomTile ??= LoadTile("ApartmentRoomFloor");
            _extractTile ??= LoadTile("ApartmentExtract");
            _boundaryTile ??= LoadTile("ApartmentBoundary");
            _roomBoundaryTile ??= LoadTile("ApartmentRoomBoundary");
            _doorTile ??= LoadTile("ApartmentDoor");
            _windowTile ??= LoadTile("ApartmentWindow");
        }

        private static Tile LoadTile(string resourceName)
        {
            var tile = Resources.Load<Tile>($"MapTiles/{resourceName}");
            if (tile == null)
            {
                Debug.LogError($"Missing tile asset: Resources/MapTiles/{resourceName}");
            }

            return tile;
        }
    }
}
