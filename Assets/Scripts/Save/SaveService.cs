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
        private const string SaveFileName = "mission_save_v1.json";
        private const string BackupFileName = "mission_save_v1.backup.json";
        private const string TempFileName = "mission_save_v1.tmp";

        [SerializeField] private int schemaVersion = 1;
        [SerializeField] private bool loadOnStart;
        [SerializeField] private MissionStateService missionStateService;
        [SerializeField] private ObjectiveService objectiveService;

        private string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);
        private string BackupPath => Path.Combine(Application.persistentDataPath, BackupFileName);
        private string TempPath => Path.Combine(Application.persistentDataPath, TempFileName);
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
            if (objectiveService != null)
            {
                objectiveService.MilestoneReached += OnMilestoneReached;
            }
        }

        private void OnDisable()
        {
            if (missionStateService != null)
            {
                missionStateService.StateChanged -= OnMissionStateChanged;
            }
            if (objectiveService != null)
            {
                objectiveService.MilestoneReached -= OnMilestoneReached;
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

        private void OnMilestoneReached()
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
                WriteSnapshot(snapshot);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"SaveService: failed to save snapshot. {exception.Message}");
            }
        }

        public bool TryLoad()
        {
            if (missionStateService == null || objectiveService == null)
            {
                return false;
            }

            if (TryLoadSnapshot(SavePath, out var snapshot) && IsCompatible(snapshot))
            {
                ApplySnapshot(snapshot);
                return true;
            }

            if (TryLoadSnapshot(BackupPath, out snapshot) && IsCompatible(snapshot))
            {
                TryRestorePrimaryFromBackup();
                ApplySnapshot(snapshot);
                return true;
            }

            return false;
        }

        private void ApplySnapshot(MissionSaveSnapshot snapshot)
        {
            missionStateService.ForceStateForLoad(snapshot.missionState);
            objectiveService.ApplySnapshot(
                snapshot.infiltrationComplete,
                snapshot.hostageFreed,
                snapshot.hostageExtracted,
                snapshot.squadAlive,
                snapshot.hostageAlive);

            Loaded?.Invoke(snapshot);
        }

        private bool IsCompatible(MissionSaveSnapshot snapshot)
        {
            if (snapshot == null)
            {
                Debug.LogWarning("SaveService: save file is empty or invalid JSON.");
                return false;
            }

            if (snapshot.schemaVersion != schemaVersion)
            {
                Debug.LogWarning($"SaveService: schema mismatch {snapshot.schemaVersion} != {schemaVersion}.");
                return false;
            }

            return true;
        }

        private void WriteSnapshot(MissionSaveSnapshot snapshot)
        {
            var json = JsonUtility.ToJson(snapshot, true);
            File.WriteAllText(TempPath, json);

            try
            {
                if (File.Exists(SavePath))
                {
                    try
                    {
                        File.Replace(TempPath, SavePath, BackupPath, true);
                    }
                    catch
                    {
                        File.Copy(TempPath, SavePath, true);
                        File.Copy(TempPath, BackupPath, true);
                    }
                }
                else
                {
                    File.Copy(TempPath, SavePath, true);
                    File.Copy(TempPath, BackupPath, true);
                }
            }
            finally
            {
                if (File.Exists(TempPath))
                {
                    File.Delete(TempPath);
                }
            }
        }

        private bool TryLoadSnapshot(string path, out MissionSaveSnapshot snapshot)
        {
            snapshot = null;

            if (!File.Exists(path))
            {
                return false;
            }

            try
            {
                var json = File.ReadAllText(path);
                snapshot = JsonUtility.FromJson<MissionSaveSnapshot>(json);
                return snapshot != null;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"SaveService: failed to load snapshot from {Path.GetFileName(path)}. {exception.Message}");
                return false;
            }
        }

        private void TryRestorePrimaryFromBackup()
        {
            try
            {
                if (File.Exists(BackupPath))
                {
                    File.Copy(BackupPath, SavePath, true);
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"SaveService: failed to restore primary save from backup. {exception.Message}");
            }
        }
    }
}
