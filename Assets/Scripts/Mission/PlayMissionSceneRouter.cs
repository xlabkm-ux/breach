using UnityEngine;
using UnityEngine.SceneManagement;

namespace Breach.Mission
{
    public static class PlayMissionSceneRouter
    {
        private const string MissionSceneName = "VS01_Rescue";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureMissionSceneIsLoadedOnPlay()
        {
            var activeScene = SceneManager.GetActiveScene();
            if (activeScene.name == MissionSceneName)
            {
                return;
            }

            if (Application.isPlaying)
            {
                SceneManager.LoadScene(MissionSceneName, LoadSceneMode.Single);
            }
        }
    }
}
