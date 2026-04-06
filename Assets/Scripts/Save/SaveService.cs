using System;
using System.IO;
using Breach.Mission;
using UnityEngine;

namespace Breach.Save
{
    [Serializable]
    public sealed class MissionSaveSnapshot
    {
        public int schemaVersion;
        public MissionState missionState;
        public bool infiltrationComplete;
        public bool hostageFreed;
        public bool hostageExtracted;
        public bool squadAlive;
        public bool hostageAlive;
    }

    public sealed class SaveService : MonoBehaviour
    {
        [SerializeField] private int schemaVersion = 1;
        [SerializeField] private bool loadOnStart;
        [SerializeField] private MissionStateService missionStateService;
        [SerializeField] private ObjectiveService objectiveService;

        private string SavePath => Path.Combine(Application.persistentDataPath, "mission_save_v1.json");
        public event Action<MissionSaveSnapshot> Loaded;

        private void Awake()
        {
            if (missionStateService == null)
            {
                missionStateService = GetComponent<MissionStateService>();
            }
            if (objectiveService == null)
            {
                objectiveService = GetComponent<ObjectiveService>();
            }
        }

        private void OnEnable()
        {
            if (missionStateService != null)
            {
                missionStateService.StateChanged += OnMissionStateChanged;
            }
        }

        private void OnDisable()
        {
            if (missionStateService != null)
            {
                missionStateService.StateChanged -= OnMissionStateChanged;
            }
        }

        private void Start()
        {
            if (loadOnStart)
            {
                TryLoad();
            }
        }

        private void OnApplicationQuit()
        {
            SaveNow();
        }

        private void OnMissionStateChanged(MissionState from, MissionState to)
        {
            SaveNow();
        }

        public void SaveNow()
        {
            if (missionStateService == null || objectiveService == null)
            {
                return;
            }

            var snapshot = new MissionSaveSnapshot
            {
                schemaVersion = schemaVersion,
                missionState = missionStateService.CurrentState,
                infiltrationComplete = objectiveService.InfiltrationComplete,
                hostageFreed = objectiveService.HostageFreed,
                hostageExtracted = objectiveService.HostageExtracted,
                squadAlive = objectiveService.SquadAlive,
                hostageAlive = objectiveService.HostageAlive
            };

            try
            {
                var json = JsonUtility.ToJson(snapshot, true);
                File.WriteAllText(SavePath, json);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"SaveService: failed to save snapshot. {exception.Message}");
            }
        }

        public bool TryLoad()
        {
            if (!File.Exists(SavePath) || missionStateService == null || objectiveService == null)
            {
                return false;
            }

            MissionSaveSnapshot snapshot;
            try
            {
                var json = File.ReadAllText(SavePath);
                snapshot = JsonUtility.FromJson<MissionSaveSnapshot>(json);
                if (snapshot == null)
                {
                    Debug.LogWarning("SaveService: save file is empty or invalid JSON.");
                    return false;
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"SaveService: failed to load snapshot. {exception.Message}");
                return false;
            }

            if (snapshot.schemaVersion != schemaVersion)
            {
                Debug.LogWarning($"SaveService: schema mismatch {snapshot.schemaVersion} != {schemaVersion}.");
                return false;
            }

            missionStateService.ForceStateForLoad(snapshot.missionState);
            objectiveService.ApplySnapshot(
                snapshot.infiltrationComplete,
                snapshot.hostageFreed,
                snapshot.hostageExtracted,
                snapshot.squadAlive,
                snapshot.hostageAlive);

            Loaded?.Invoke(snapshot);
            return true;
        }
    }
}
