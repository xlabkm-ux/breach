using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TacticalBreach.Editor
{
    public static class Day1LevelGenerator
    {
        class BSPRoom
        {
            public int x, y, w, h;
            public BSPRoom left, right;
            public bool isSplitH;
            public int splitPos;
            
            public BSPRoom(int x, int y, int w, int h)
            {
                this.x = x; this.y = y; this.w = w; this.h = h;
            }
        }

        // Generation Settings
        private const int MapWidth = 40;
        private const int MapHeight = 30;
        private const int MinRoomSize = 12; // Increased for more space
        private const float SplitThreshold = 1.25f;

        [MenuItem("TacticalBreach/Generate Mission (BSP Procedural)")]
        public static void GenerateDay1Level()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("Cannot generate level in Play mode.");
                return;
            }

            Debug.Log($"[Emergency Reset] Starting Clean Generation ({MapWidth}x{MapHeight})...");

            // 0. Force Cleanup Everything
            ForceCleanupScene();

            // 1. Force Setup everything first
            Day1SceneSetup.SetupScene();
            Day1AssetSetup.SetupAssets();
            Day1CharacterSetup.SetupCharacters();
            Day1VisualPolisher.Polish();

            // 1. Setup Grid and Tilemaps
            var gridGo = GameObject.Find("Grid") ?? new GameObject("Grid");
            gridGo.name = "Grid";
            var grid = gridGo.GetComponent<Grid>() ?? gridGo.AddComponent<Grid>();

            var baseMap = FindTilemap("World_Base");
            if (baseMap == null)
            {
                var go = new GameObject("World_Base");
                go.transform.SetParent(gridGo.transform);
                baseMap = go.AddComponent<Tilemap>();
                go.AddComponent<TilemapRenderer>();
            }

            var collisionMap = FindTilemap("World_Collision");
            if (collisionMap == null)
            {
                var go = new GameObject("World_Collision");
                go.transform.SetParent(gridGo.transform);
                collisionMap = go.AddComponent<Tilemap>();
                go.AddComponent<TilemapRenderer>();
            }

            var floorTile = AssetDatabase.LoadAssetAtPath<Tile>("Assets/Resources/MapTiles/TacticalFloor.asset");
            var wallTile = AssetDatabase.LoadAssetAtPath<Tile>("Assets/Resources/MapTiles/TacticalWall.asset");
            
            if (floorTile == null || wallTile == null)
            {
                Debug.LogError("Floor or Wall tile missing! Run Setup Day 1 Assets first.");
                return;
            }

            // CLEAR OLD TILES
            baseMap.ClearAllTiles();
            collisionMap.ClearAllTiles();

            // 2. Procedural Layout (Floor + Walls)
            int startX = -MapWidth / 2;
            int startY = -MapHeight / 2;
            
            // Fill floor
            for (int ix = startX; ix < startX + MapWidth; ix++)
                for (int iy = startY; iy < startY + MapHeight; iy++)
                    baseMap.SetTile(new Vector3Int(ix, iy, 0), floorTile);

            // Draw ONLY the outer perimeter walls (Hollow)
            for (int ix = startX; ix < startX + MapWidth; ix++)
            {
                collisionMap.SetTile(new Vector3Int(ix, startY, 0), wallTile);
                collisionMap.SetTile(new Vector3Int(ix, startY + MapHeight - 1, 0), wallTile);
            }
            for (int iy = startY; iy < startY + MapHeight; iy++)
            {
                collisionMap.SetTile(new Vector3Int(startX, iy, 0), wallTile);
                collisionMap.SetTile(new Vector3Int(startX + MapWidth - 1, iy, 0), wallTile);
            }

            // Split the building into rooms (NO walls drawn during split)
            BSPRoom root = new BSPRoom(startX, startY, MapWidth, MapHeight);
            SplitRoom(root, MinRoomSize, collisionMap, wallTile);

            // Collect Leaves for windows, furniture, enemies
            List<BSPRoom> leaves = new List<BSPRoom>();
            CollectLeaves(root, leaves);

            // Draw walls only around leaf room perimeters
            DrawLeafRoomWalls(root, null, collisionMap, wallTile);

            // Carve doorways through shared walls
            ConnectRooms(root, collisionMap);

            // Add Windows on perimeter
            AddWindows(leaves, collisionMap);

            // Add Furniture/Covers
            SpawnFurniture(leaves);

            // Add Enemies
            SpawnEnemies(leaves);

            // Add Entrance Door
            AddEntrance(leaves, collisionMap);

            // 4. Final Setup and Environment Polishing
            SetupEnvironment();
            SetupColliders(baseMap); // Floor MUST have collider for NavMesh surface!
            SetupColliders(collisionMap);
            SetupSpawnArea(baseMap, collisionMap);
            
            // Initial positioning
            PositionOperativesOnStreet();
            SpawnEnemies(leaves);
            
            // Bake NavMesh
            BakeNavMesh();

            // Final Snap and enable agents
            SnapAllAgentsToNavMesh();

            // Focus camera on operatives
            var cam = Camera.main;
            if (cam != null)
            {
                cam.cullingMask = -1;
                cam.orthographic = true;
                cam.orthographicSize = 10;
                cam.transform.position = new Vector3(0, -MapHeight/2 - 5f, -10f);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("Total Generation Complete! All agents snapped and camera focused.");
        }

        private static void DrawRect(Tilemap map, int x, int y, int w, int h, Tile tile)
        {
            for (int ix = x; ix < x + w; ix++)
            {
                map.SetTile(new Vector3Int(ix, y, 0), tile);
                map.SetTile(new Vector3Int(ix, y + h - 1, 0), tile);
            }
            for (int iy = y; iy < y + h; iy++)
            {
                map.SetTile(new Vector3Int(x, iy, 0), tile);
                map.SetTile(new Vector3Int(x + w - 1, iy, 0), tile);
            }
        }

        private static void SplitRoom(BSPRoom room, int minSize, Tilemap collisionMap, Tile wallTile)
        {
            bool splitH = Random.value > 0.5f;
            if (room.w > room.h && (float)room.w / room.h >= SplitThreshold) splitH = false;
            else if (room.h > room.w && (float)room.h / room.w >= SplitThreshold) splitH = true;

            // Stop splitting if room is too small for TWO rooms of minSize
            if (splitH && room.h <= minSize * 2) return;
            if (!splitH && room.w <= minSize * 2) return;

            int max = (splitH ? room.h : room.w) - minSize;
            if (max <= minSize) return;

            int splitPoint = Random.Range(minSize, max);
            room.isSplitH = splitH;
            room.splitPos = splitPoint;

            if (splitH)
            {
                room.left = new BSPRoom(room.x, room.y, room.w, splitPoint);
                room.right = new BSPRoom(room.x, room.y + splitPoint, room.w, room.h - splitPoint);
            }
            else
            {
                room.left = new BSPRoom(room.x, room.y, splitPoint, room.h);
                room.right = new BSPRoom(room.x + splitPoint, room.y, room.w - splitPoint, room.h);
            }

            SplitRoom(room.left, minSize, collisionMap, wallTile);
            SplitRoom(room.right, minSize, collisionMap, wallTile);
        }

        // Draw walls around leaf rooms, carving 2-wide doorways at split boundaries
        private static void DrawLeafRoomWalls(BSPRoom node, BSPRoom parent, Tilemap collisionMap, Tile wallTile)
        {
            if (node.left == null && node.right == null)
            {
                // Leaf room: draw only interior walls (inner perimeter)
                for (int ix = node.x; ix < node.x + node.w; ix++)
                {
                    SetWallIfNotOuter(collisionMap, ix, node.y, wallTile, node);
                    SetWallIfNotOuter(collisionMap, ix, node.y + node.h - 1, wallTile, node);
                }
                for (int iy = node.y; iy < node.y + node.h; iy++)
                {
                    SetWallIfNotOuter(collisionMap, node.x, iy, wallTile, node);
                    SetWallIfNotOuter(collisionMap, node.x + node.w - 1, iy, wallTile, node);
                }
                return;
            }
            if (node.left != null) DrawLeafRoomWalls(node.left, node, collisionMap, wallTile);
            if (node.right != null) DrawLeafRoomWalls(node.right, node, collisionMap, wallTile);
        }

        private static void SetWallIfNotOuter(Tilemap map, int x, int y, Tile tile, BSPRoom room)
        {
            // Don't override the outer perimeter (already drawn)
            map.SetTile(new Vector3Int(x, y, 0), tile);
        }


        private static void ConnectRooms(BSPRoom node, Tilemap collisionMap)
        {
            if (node.left != null && node.right != null)
            {
                var doorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/World/TacticalDoor.prefab");
                var doorsContainer = GameObject.Find("Doors") ?? new GameObject("Doors");

                int doorX, doorY;
                float rot = 0;

                if (node.isSplitH)
                {
                    doorY = node.right.y;
                    doorX = node.x + Random.Range(1, node.w - 1);
                    rot = 90;
                }
                else
                {
                    doorX = node.right.x;
                    doorY = node.y + Random.Range(1, node.h - 1);
                    rot = 0;
                }

                // Punch Wall
                collisionMap.SetTile(new Vector3Int(doorX, doorY, 0), null);

                if (doorPrefab != null)
                {
                    var doorGo = (GameObject)PrefabUtility.InstantiatePrefab(doorPrefab, doorsContainer.transform);
                    doorGo.transform.position = new Vector3(doorX + 0.5f, doorY + 0.5f, 0);
                    doorGo.transform.rotation = Quaternion.Euler(0, 0, rot);
                }
            }

            if (node.left != null) ConnectRooms(node.left, collisionMap);
            if (node.right != null) ConnectRooms(node.right, collisionMap);
        }

        private static void AddWindows(List<BSPRoom> rooms, Tilemap collisionMap)
        {
            var windowTile = AssetDatabase.LoadAssetAtPath<Tile>("Assets/Resources/MapTiles/ApartmentWindow.asset");
            if (windowTile == null) return;

            foreach (var room in rooms)
            {
                // Simple logic: if room is on perimeter, place 1-2 windows
                if (room.x == -MapWidth / 2) // West
                    collisionMap.SetTile(new Vector3Int(room.x, room.y + room.h / 2, 0), windowTile);
                if (room.x + room.w == MapWidth / 2) // East
                    collisionMap.SetTile(new Vector3Int(room.x + room.w - 1, room.y + room.h / 2, 0), windowTile);
                if (room.y + room.h == MapHeight / 2) // North
                    collisionMap.SetTile(new Vector3Int(room.x + room.w / 2, room.y + room.h - 1, 0), windowTile);
                // South is for entrance
            }
        }

        private static void SpawnFurniture(List<BSPRoom> rooms)
        {
            var coverPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/World/TacticalCover.prefab");
            if (coverPrefab == null) return;

            var furnitureContainer = GameObject.Find("Furniture") ?? new GameObject("Furniture");

            foreach (var room in rooms)
            {
                int count = Random.Range(1, 3);
                for (int i = 0; i < count; i++)
                {
                    int fx = room.x + Random.Range(2, room.w - 2);
                    int fy = room.y + Random.Range(2, room.h - 2);
                    
                    var go = (GameObject)PrefabUtility.InstantiatePrefab(coverPrefab, furnitureContainer.transform);
                    go.transform.position = new Vector3(fx + 0.5f, fy + 0.5f, 0);
                    go.transform.localScale = Vector3.one;
                    go.transform.rotation = Quaternion.identity;
                }
            }
        }

        private static void SpawnEnemies(List<BSPRoom> rooms)
        {
            string[] possiblePaths = {
                "Assets/Prefabs/Gameplay/Enemies/Enemy_Grunt.prefab",
                "Assets/Prefabs/Enemies/Enemy_Grunt.prefab",
                "Assets/Prefabs/World/Enemy_Grunt.prefab"
            };
            
            GameObject enemyPrefab = null;
            foreach(var p in possiblePaths) 
            {
                enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(p);
                if (enemyPrefab != null) break;
            }

            if (enemyPrefab == null)
            {
                Debug.LogError("[Spawn] Enemy Grunt prefab NOT FOUND in any path!");
                return;
            }

            var enemiesContainer = GameObject.Find("Enemies") ?? new GameObject("Enemies");
            enemiesContainer.transform.position = Vector3.zero;

            int totalEnemies = 0;
            foreach (var room in rooms)
            {
                if (room.y == -MapHeight / 2) continue;

                int count = Random.Range(1, 2); // 1 enemy per room for now
                for (int i = 0; i < count; i++)
                {
                    int ex = room.x + Random.Range(2, room.w - 2);
                    int ey = room.y + Random.Range(2, room.h - 2);
                    
                    var go = (GameObject)PrefabUtility.InstantiatePrefab(enemyPrefab, enemiesContainer.transform);
                    go.transform.position = new Vector3(ex + 0.5f, ey + 0.5f, 0);
                    go.transform.localScale = Vector3.one;
                    totalEnemies++;
                }
            }
            Debug.Log($"[Spawn] Instantiated {totalEnemies} enemies.");
        }

        private static void PositionOperativesOnStreet()
        {
            var ops = Object.FindObjectsByType<TacticalBreach.Squad.OperativeMember>(FindObjectsInactive.Include);
            Debug.Log($"[Spawn] Found {ops.Length} operatives in scene to position.");
            
            for (int i = 0; i < ops.Length; i++)
            {
                ops[i].gameObject.SetActive(true);
                ops[i].transform.position = new Vector3(-2f + i * 2f, -MapHeight/2 - 5f, 0f);
                ops[i].transform.localScale = Vector3.one;
            }
        }

        private static void AddEntrance(List<BSPRoom> rooms, Tilemap collisionMap)
        {
            var doorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/World/TacticalDoor.prefab");
            var doorsContainer = GameObject.Find("Doors") ?? new GameObject("Doors");
            
            // Find a room on the south side
            var southRooms = rooms.FindAll(r => r.y == -MapHeight / 2);
            if (southRooms.Count > 0)
            {
                var target = southRooms[southRooms.Count / 2];
                int entX = target.x + target.w / 2;
                int entY = -MapHeight / 2;

                collisionMap.SetTile(new Vector3Int(entX, entY, 0), null);
                
                if (doorPrefab != null)
                {
                    var doorGo = (GameObject)PrefabUtility.InstantiatePrefab(doorPrefab, doorsContainer.transform);
                    doorGo.name = "Entrance_Door";
                    doorGo.transform.position = new Vector3(entX + 0.5f, entY + 0.5f, 0);
                    doorGo.transform.rotation = Quaternion.Euler(0, 0, 90);
                    doorGo.transform.localScale = Vector3.one;
                }
            }
        }

        private static void SetupSpawnArea(Tilemap baseMap, Tilemap collisionMap)
        {
            var floorTile = AssetDatabase.LoadAssetAtPath<Tile>("Assets/Resources/MapTiles/TacticalFloor.asset");
            // Expand floor below building for street
            for (int ix = -MapWidth/2 - 5; ix < MapWidth/2 + 5; ix++)
                for (int iy = -MapHeight/2 - 10; iy < -MapHeight/2; iy++)
                    baseMap.SetTile(new Vector3Int(ix, iy, 0), floorTile);
        }


        private static void ForceCleanupScene()
        {
            // 1. Force cleanup all procedural containers (including inactive duplicates)
            string[] targets = { "Grid", "Doors", "Furniture", "NavMeshManager", "GlobalLight2D", "MissionRuntimeStabilizer_Runtime" };
            var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            
            foreach (var go in allObjects)
            {
                if (PrefabUtility.IsPartOfPrefabAsset(go)) continue;
                
                foreach (var targetName in targets)
                {
                    if (go.name == targetName)
                    {
                        Object.DestroyImmediate(go);
                        break;
                    }
                }
            }
            
            // 2. Do NOT delete operatives, just ensure they are active and ready
            var ops = Object.FindObjectsByType<TacticalBreach.Squad.OperativeMember>(FindObjectsInactive.Include);
            foreach (var op in ops)
            {
                op.gameObject.SetActive(true);
                op.transform.position = Vector3.zero; // Reset temporarily before Positioning
            }

            // 3. Delete ONLY procedural enemies, NOT operatives
            var allGOs = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);
            foreach (var go in allGOs)
            {
                if (go == null) continue; // Guard against already-destroyed objects
                if (PrefabUtility.IsPartOfPrefabAsset(go)) continue;
                
                // If it's an enemy or a known temporary source
                if (go.name.Contains("Enemy_Grunt") || go.name == "Enemies")
                {
                    Object.DestroyImmediate(go);
                }
            }
        }

        private static void SetupEnvironment()
        {
            var defaultMat = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
            
            // Layer 0: Floor
            var baseMap = FindTilemap("World_Base");
            if (baseMap != null)
            {
                baseMap.transform.position = new Vector3(0, 0, 0); // Z = 0
                var tr = baseMap.GetComponent<TilemapRenderer>();
                tr.sharedMaterial = defaultMat;
                tr.sortingOrder = 0;
            }

            // Layer 1: Walls
            var collisionMap = FindTilemap("World_Collision");
            if (collisionMap != null)
            {
                collisionMap.transform.position = new Vector3(0, 0, -1f); // Z = -1
                var tr = collisionMap.GetComponent<TilemapRenderer>();
                tr.sharedMaterial = defaultMat;
                tr.sortingOrder = 1;
            }

            // Layer 2: Props and Furniture
            string[] propContainers = { "Doors", "Furniture" };
            foreach (var name in propContainers)
            {
                var container = GameObject.Find(name);
                if (container != null)
                {
                    container.transform.position = new Vector3(0, 0, -2f); // Z = -2
                    var renderers = container.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var r in renderers) r.sortingOrder = 2;
                }
            }

            var lightGo = GameObject.Find("GlobalLight2D") ?? new GameObject("GlobalLight2D");
            var light = lightGo.GetComponent<UnityEngine.Rendering.Universal.Light2D>() ?? lightGo.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
            light.lightType = UnityEngine.Rendering.Universal.Light2D.LightType.Global;
            light.intensity = 1.0f;
        }

        private static void CollectLeaves(BSPRoom node, List<BSPRoom> leaves)
        {
            if (node.left == null && node.right == null) { leaves.Add(node); return; }
            if (node.left != null) CollectLeaves(node.left, leaves);
            if (node.right != null) CollectLeaves(node.right, leaves);
        }

        private static void SetupColliders(Tilemap map)
        {
            if (map == null) return;
            var tilemapCollider = map.GetComponent<TilemapCollider2D>() ?? map.gameObject.AddComponent<TilemapCollider2D>();
            var composite = map.GetComponent<CompositeCollider2D>() ?? map.gameObject.AddComponent<CompositeCollider2D>();
            var rb = map.GetComponent<Rigidbody2D>() ?? map.gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            
            tilemapCollider.compositeOperation = Collider2D.CompositeOperation.Merge;
            tilemapCollider.usedByComposite = true;
            composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
        }

        private static void BakeNavMesh()
        {
            var navManager = GameObject.Find("NavMeshManager") ?? new GameObject("NavMeshManager");
            navManager.transform.position = Vector3.zero;
            navManager.transform.rotation = Quaternion.Euler(-90, 0, 0);

            var surfaceType = System.Type.GetType("Unity.AI.Navigation.NavMeshSurface, Unity.AI.Navigation");
            if (surfaceType != null)
            {
                var surfaceComponent = navManager.GetComponent(surfaceType) ?? navManager.AddComponent(surfaceType);
                
                // 0 = All Objects, 1 = Volume, 2 = Current Hierarchy
                surfaceType.GetProperty("collectObjects")?.SetValue(surfaceComponent, 0); 
                // 1 = Physics Colliders
                surfaceType.GetProperty("useGeometry")?.SetValue(surfaceComponent, 1); 

                var settings = surfaceType.GetMethod("GetOrCreateDefaultSettings")?.Invoke(surfaceComponent, null);
                if (settings != null)
                {
                    settings.GetType().GetField("agentRadius")?.SetValue(settings, 0.3f);
                    settings.GetType().GetField("agentClimb")?.SetValue(settings, 0.1f);
                }

                surfaceType.GetMethod("BuildNavMesh")?.Invoke(surfaceComponent, null);
                Debug.Log("[NavMesh] Baked successfully on Z=0 plane using Physics.");
            }
        }

        private static void SnapAllAgentsToNavMesh()
        {
            var agents = Object.FindObjectsByType<UnityEngine.AI.NavMeshAgent>(FindObjectsInactive.Include);
            Debug.Log($"[NavMesh] Found {agents.Length} agents to snap.");

            foreach (var agent in agents)
            {
                agent.gameObject.SetActive(true);
                Vector3 pos = agent.transform.position;
                pos.z = 0; // Physics stays at zero
                
                if (UnityEngine.AI.NavMesh.SamplePosition(pos, out UnityEngine.AI.NavMeshHit hit, 10.0f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    agent.transform.position = hit.position;
                    agent.enabled = true;
                    
                    // FORCE VISIBILITY: GameObject at Z=0, but visuals at Z=-3
                    var sr = agent.GetComponentInChildren<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.transform.localPosition = new Vector3(0, 0, -3.0f);
                        sr.sortingOrder = 3;
                        sr.enabled = true;
                    }
                    agent.transform.localScale = Vector3.one;
                }
                else
                {
                    Debug.LogWarning($"[NavMesh] Could not snap {agent.name} at {pos} - no NavMesh nearby!");
                }
            }
        }

        private static Tilemap FindTilemap(string name)
        {
            var go = GameObject.Find(name);
            return go != null ? go.GetComponent<Tilemap>() : null;
        }
    }
}
