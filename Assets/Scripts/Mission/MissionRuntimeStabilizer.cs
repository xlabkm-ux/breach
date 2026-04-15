using System.Collections.Generic;
using Breach.AI;
using Breach.Combat;
using Breach.Hostage;
using Breach.Squad;
using UnityEngine;

namespace Breach.Mission
{
    public sealed class MissionRuntimeStabilizer : MonoBehaviour
    {
        [SerializeField] private float cameraLerp = 12f;
        [SerializeField] private float defaultCameraSize = 8f;
        [SerializeField] private int minEnemyCount = 2;

        private static Sprite fallbackSprite;
        private ActiveOperativeSwitchService switchService;
        private Camera mainCamera;
        private bool initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureRuntimeInstance()
        {
            if (FindAnyObjectByType<MissionRuntimeStabilizer>() != null)
            {
                return;
            }

            var runtimeObject = new GameObject("MissionRuntimeStabilizer_Runtime");
            runtimeObject.AddComponent<MissionRuntimeStabilizer>();
        }

        private void Start()
        {
            InitializeMissionRuntime();
        }

        private void Update()
        {
            if (!initialized)
            {
                InitializeMissionRuntime();
            }
        }

        private void LateUpdate()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            if (switchService == null)
            {
                switchService = FindAnyObjectByType<ActiveOperativeSwitchService>();
            }
            if (mainCamera == null || switchService == null || switchService.ActiveOperative == null)
            {
                return;
            }

            var cameraPos = mainCamera.transform.position;
            var target = switchService.ActiveOperative.transform.position;
            var desired = new Vector3(target.x, target.y, cameraPos.z);
            mainCamera.transform.position = Vector3.Lerp(cameraPos, desired, cameraLerp * Time.deltaTime);
        }

        private void InitializeMissionRuntime()
        {
            mainCamera = Camera.main;
            switchService = FindAnyObjectByType<ActiveOperativeSwitchService>();

            EnsureLayout();
            EnsureOperatives();
            EnsureEnemies();
            EnsureHostage();
            EnsureCameraDefaults();

            initialized = true;
        }

        public void ReinitializeForLoadedState()
        {
            initialized = false;
            InitializeMissionRuntime();
        }

        private static void EnsureLayout()
        {
            var builders = FindObjectsByType<ApartmentLayoutBuilder>(UnityEngine.FindObjectsInactive.Exclude);
            foreach (var builder in builders)
            {
                builder.Rebuild();
            }
        }

        private void EnsureOperatives()
        {
            var operatives = FindObjectsByType<OperativeMember>(UnityEngine.FindObjectsInactive.Exclude);
            for (var i = 0; i < operatives.Length; i++)
            {
                var operative = operatives[i];
                if (operative == null)
                {
                    continue;
                }

                EnsureVisual(operative.gameObject, new Color(0.2f, 0.95f, 0.3f, 1f), 0.45f);
                EnsureCollider(operative.gameObject, 0.35f);
                EnsureTeam(operative.GetComponent<HealthComponent>(), TeamId.Squad);
                EnsureDamageFeedback(operative.gameObject);
                EnsureShooter(operative.gameObject);

                if (i == 0 && operative.transform.position.sqrMagnitude < 0.01f)
                {
                    operative.transform.position = new Vector3(-7f, -2f, 0f);
                }
                else if (i == 1 && operative.transform.position.sqrMagnitude < 0.01f)
                {
                    operative.transform.position = new Vector3(-5.5f, -2f, 0f);
                }
            }
        }

        private void EnsureEnemies()
        {
            var enemies = CollectEnemyObjects();
            while (enemies.Count < minEnemyCount)
            {
                enemies.Add(CreateRuntimeEnemy(enemies.Count));
            }

            for (var i = 0; i < enemies.Count; i++)
            {
                var enemyObject = enemies[i];
                if (enemyObject == null)
                {
                    continue;
                }

                var health = enemyObject.GetComponent<HealthComponent>();
                if (health == null)
                {
                    health = enemyObject.AddComponent<HealthComponent>();
                }

                if (enemyObject.GetComponent<EnemyPatrolAgent>() == null)
                {
                    enemyObject.AddComponent<EnemyPatrolAgent>();
                }

                if (enemyObject.GetComponent<EnemyVisionCone>() == null)
                {
                    enemyObject.AddComponent<EnemyVisionCone>();
                }

                if (enemyObject.GetComponent<EnemyAlertController>() == null)
                {
                    enemyObject.AddComponent<EnemyAlertController>();
                }

                EnsureVisual(enemyObject, new Color(1f, 0.25f, 0.25f, 1f), 0.45f);
                EnsureCollider(enemyObject, 0.35f);
                EnsureTeam(health, TeamId.Enemy);
                EnsureDamageFeedback(enemyObject);

                if (enemyObject.transform.position.sqrMagnitude < 0.01f)
                {
                    var x = 3.5f + i * 1.7f;
                    var y = (i % 2 == 0) ? 1.5f : -0.5f;
                    enemyObject.transform.position = new Vector3(x, y, 0f);
                }
            }
        }

        private static List<GameObject> CollectEnemyObjects()
        {
            var result = new List<GameObject>();

            var alertControllers = FindObjectsByType<EnemyAlertController>(UnityEngine.FindObjectsInactive.Exclude);
            foreach (var alertController in alertControllers)
            {
                if (alertController != null && !result.Contains(alertController.gameObject))
                {
                    result.Add(alertController.gameObject);
                }
            }

            var patrolAgents = FindObjectsByType<EnemyPatrolAgent>(UnityEngine.FindObjectsInactive.Exclude);
            foreach (var patrolAgent in patrolAgents)
            {
                if (patrolAgent != null && !result.Contains(patrolAgent.gameObject))
                {
                    result.Add(patrolAgent.gameObject);
                }
            }

            var namedEnemies = GameObject.FindObjectsByType<Transform>(UnityEngine.FindObjectsInactive.Exclude);
            foreach (var tr in namedEnemies)
            {
                if (tr == null || tr.gameObject == null)
                {
                    continue;
                }

                if (!tr.gameObject.name.Contains("Enemy"))
                {
                    continue;
                }

                if (!result.Contains(tr.gameObject))
                {
                    result.Add(tr.gameObject);
                }
            }

            return result;
        }

        private static GameObject CreateRuntimeEnemy(int index)
        {
            var enemyObject = new GameObject($"Enemy_Runtime_{index + 1}");
            enemyObject.transform.position = new Vector3(4.2f + index * 1.4f, 1.2f - (index % 2) * 1.4f, 0f);
            return enemyObject;
        }

        private static void EnsureHostage()
        {
            var hostage = FindAnyObjectByType<HostageController>();
            if (hostage == null)
            {
                return;
            }

            EnsureVisual(hostage.gameObject, new Color(1f, 0.9f, 0.15f, 1f), 0.42f);
            EnsureCollider(hostage.gameObject, 0.32f);
            EnsureTeam(hostage.GetComponent<HealthComponent>(), TeamId.Neutral);
            EnsureDamageFeedback(hostage.gameObject);

            if (hostage.transform.position.sqrMagnitude < 0.01f)
            {
                hostage.transform.position = new Vector3(7.2f, 0.2f, 0f);
            }
        }

        private void EnsureCameraDefaults()
        {
            if (mainCamera == null)
            {
                return;
            }

            mainCamera.orthographic = true;
            if (mainCamera.orthographicSize < defaultCameraSize)
            {
                mainCamera.orthographicSize = defaultCameraSize;
            }
            var pos = mainCamera.transform.position;
            if (Mathf.Abs(pos.z) < 0.01f)
            {
                mainCamera.transform.position = new Vector3(pos.x, pos.y, -10f);
            }
        }

        private static void EnsureVisual(GameObject target, Color color, float scale)
        {
            var renderer = target.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = target.AddComponent<SpriteRenderer>();
            }

            if (fallbackSprite == null)
            {
                var texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
                var pixels = new Color[16 * 16];
                for (var i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = Color.white;
                }
                texture.SetPixels(pixels);
                texture.Apply();
                fallbackSprite = Sprite.Create(texture, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16f);
            }

            if (renderer.sprite == null)
            {
                renderer.sprite = fallbackSprite;
            }

            renderer.color = color;
            target.transform.localScale = new Vector3(scale, scale, 1f);
        }

        private static void EnsureCollider(GameObject target, float radius)
        {
            var collider = target.GetComponent<CircleCollider2D>();
            if (collider == null)
            {
                collider = target.AddComponent<CircleCollider2D>();
            }

            collider.isTrigger = false;
            collider.radius = radius;
        }

        private static void EnsureTeam(HealthComponent healthComponent, TeamId teamId)
        {
            if (healthComponent == null)
            {
                return;
            }

            if (healthComponent.Team != teamId)
            {
                healthComponent.SetTeam(teamId);
            }
        }

        private static void EnsureShooter(GameObject target)
        {
            if (target.GetComponent<SimpleShooter>() == null)
            {
                target.AddComponent<SimpleShooter>();
            }

            if (target.GetComponent<NoiseEmitter>() == null)
            {
                target.AddComponent<NoiseEmitter>();
            }
        }

        private static void EnsureDamageFeedback(GameObject target)
        {
            if (target.GetComponent<DamageFlashFeedback>() == null)
            {
                target.AddComponent<DamageFlashFeedback>();
            }
        }
    }
}
