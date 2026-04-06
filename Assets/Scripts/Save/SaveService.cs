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
        [SerializeField] private MissionStateService missionStateService;
        [SerializeField] private ObjectiveService objectiveService;

        private string SavePath => Path.Combine(Application.persistentDataPath, "mission_save_v1.json");

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
            TryLoad();
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

            var json = JsonUtility.ToJson(snapshot, true);
            File.WriteAllText(SavePath, json);
        }

        public bool TryLoad()
        {
            if (!File.Exists(SavePath) || missionStateService == null || objectiveService == null)
            {
                return false;
            }

            var json = File.ReadAllText(SavePath);
            var snapshot = JsonUtility.FromJson<MissionSaveSnapshot>(json);
            if (snapshot == null || snapshot.schemaVersion != schemaVersion)
            {
                return false;
            }

            missionStateService.ForceStateForLoad(snapshot.missionState);
            objectiveService.ApplySnapshot(
                snapshot.infiltrationComplete,
                snapshot.hostageFreed,
                snapshot.hostageExtracted,
                snapshot.squadAlive,
                snapshot.hostageAlive);
            return true;
        }
    }
}
