using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using TacticalBreach.Editor;

namespace TacticalBreach.Ci
{
    public static class GraphicsBaker
    {
        [InitializeOnLoadMethod]
        private static void AutoRun()
        {
            if (!Application.isBatchMode || !System.Environment.CommandLine.Contains("-BakeGraphics"))
                return;

            Debug.Log("[CI] Baking Graphics...");
            MapTileAssetBootstrap.GenerateApartmentTiles();
            
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/VerticalSlice/VS01_Rescue.unity", OpenSceneMode.Single);
            
            MapTileAssetBootstrap.BakeVs01RescueLayout();
            SetupTacticalEnvironment();
            
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveOpenScenes();

            Debug.Log("[CI] Baking Graphics COMPLETED.");
            EditorApplication.Exit(0);
        }

        private static void SetupTacticalEnvironment()
        {
            // 0. Tilemap Materials.
            var litMatGuid = "51bc4a5b15d239941a796041609cd381"; // ConcreteFloor_Lit.mat guid
            var litMatPath = AssetDatabase.GUIDToAssetPath(litMatGuid);
            var litMat = AssetDatabase.LoadAssetAtPath<Material>(litMatPath);

            if (litMat != null)
            {
                var tilemapRenderers = Object.FindObjectsByType<TilemapRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var tr in tilemapRenderers)
                {
                    tr.sharedMaterial = litMat;
                }
                Debug.Log($"[CI] Applied Lit Material to {tilemapRenderers.Length} TilemapRenderers.");
            }

            // 1. Global Light 2D.
            var globalLightGo = GameObject.Find("GlobalLight2D");
            if (globalLightGo == null)
            {
                globalLightGo = new GameObject("GlobalLight2D");
                var light = globalLightGo.AddComponent<Light2D>();
                light.lightType = Light2D.LightType.Global;
                light.intensity = 0.15f;
                light.color = new Color(0.8f, 0.9f, 1f, 1f); // Slightly cool tint.
            }

            // 2. Global Volume (Post-processing).
            var volumeGo = GameObject.Find("GlobalVolume");
            if (volumeGo == null)
            {
                volumeGo = new GameObject("GlobalVolume");
                var volume = volumeGo.AddComponent<Volume>();
                volume.isGlobal = true;

                var profile = ScriptableObject.CreateInstance<VolumeProfile>();
                var bloom = profile.Add<Bloom>(true);
                bloom.threshold.Override(0.9f);
                bloom.intensity.Override(1.5f);
                bloom.scatter.Override(0.7f);

                var vignette = profile.Add<Vignette>(true);
                vignette.intensity.Override(0.45f);
                vignette.smoothness.Override(0.3f);
                vignette.color.Override(Color.black);

                var colorAdjust = profile.Add<ColorAdjustments>(true);
                colorAdjust.contrast.Override(15f);
                colorAdjust.saturation.Override(5f);

                AssetDatabase.CreateAsset(profile, "Assets/Resources/Settings/TacticalVolumeProfile.asset");
                volume.sharedProfile = profile;
            }
        }
    }
}
