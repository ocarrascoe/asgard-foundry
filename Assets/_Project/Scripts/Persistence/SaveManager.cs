using System;
using System.IO;
using UnityEngine;
using AsgardFoundry.Data;

namespace AsgardFoundry.Persistence
{
    /// <summary>
    /// Handles JSON-based save/load operations.
    /// Saves to Application.persistentDataPath for iOS compatibility.
    /// </summary>
    public static class SaveManager
    {
        private const string SAVE_FILE_NAME = "asgard_foundry_save.json";

        /// <summary>
        /// Get the full path to the save file.
        /// </summary>
        public static string SaveFilePath => 
            Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

        /// <summary>
        /// Save the game state to JSON file.
        /// </summary>
        public static void SaveGame(GameState state)
        {
            try
            {
                // Convert dictionaries to serializable format
                var saveData = new SaveData(state);
                string json = JsonUtility.ToJson(saveData, prettyPrint: true);
                File.WriteAllText(SaveFilePath, json);
                Debug.Log($"[SaveManager] Game saved to: {SaveFilePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Failed to save: {ex.Message}");
            }
        }

        /// <summary>
        /// Load the game state from JSON file.
        /// Returns null if no save exists or load fails.
        /// </summary>
        public static GameState LoadGame()
        {
            try
            {
                if (!File.Exists(SaveFilePath))
                {
                    Debug.Log("[SaveManager] No save file found");
                    return null;
                }

                string json = File.ReadAllText(SaveFilePath);
                var saveData = JsonUtility.FromJson<SaveData>(json);
                return saveData.ToGameState();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Failed to load: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Delete the save file (for debugging/reset).
        /// </summary>
        public static void DeleteSave()
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    File.Delete(SaveFilePath);
                    Debug.Log("[SaveManager] Save file deleted");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] Failed to delete save: {ex.Message}");
            }
        }

        /// <summary>
        /// Check if a save file exists.
        /// </summary>
        public static bool SaveExists() => File.Exists(SaveFilePath);
    }

    /// <summary>
    /// Serializable wrapper for GameState to handle dictionaries.
    /// Unity's JsonUtility doesn't serialize Dictionary directly.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public EitrState Eitr;
        public SerializableSystemState[] Systems;
        public SerializableResource[] Resources;
        public string[] UnlockedAutomations;
        public SerializableCityProgression City;
        public int SelectedAssignment;
        public long LastSaveTimestamp;
        public double TotalPlayTime;

        public SaveData() { }

        public SaveData(GameState state)
        {
            Eitr = state.Eitr;
            SelectedAssignment = (int)state.SelectedAssignment;
            LastSaveTimestamp = state.LastSaveTimestamp;
            TotalPlayTime = state.TotalPlayTime;
            UnlockedAutomations = state.UnlockedAutomations.ToArray();

            // Convert systems dictionary
            Systems = new SerializableSystemState[state.Systems.Count];
            int i = 0;
            foreach (var kvp in state.Systems)
            {
                Systems[i++] = new SerializableSystemState(kvp.Value);
            }

            // Convert resources dictionary
            Resources = new SerializableResource[state.Resources.Count];
            i = 0;
            foreach (var kvp in state.Resources)
            {
                Resources[i++] = new SerializableResource 
                { 
                    Type = (int)kvp.Key, 
                    Amount = kvp.Value 
                };
            }

            // Convert city progression
            City = new SerializableCityProgression(state.City);
        }

        public GameState ToGameState()
        {
            var state = new GameState
            {
                Eitr = Eitr,
                SelectedAssignment = (SystemType)SelectedAssignment,
                LastSaveTimestamp = LastSaveTimestamp,
                TotalPlayTime = TotalPlayTime,
                UnlockedAutomations = new System.Collections.Generic.List<string>(UnlockedAutomations ?? Array.Empty<string>())
            };

            // Restore systems
            if (Systems != null)
            {
                foreach (var sysData in Systems)
                {
                    var sysState = sysData.ToProductionSystemState();
                    state.Systems[sysState.Type] = sysState;
                }
            }

            // Restore resources
            if (Resources != null)
            {
                foreach (var res in Resources)
                {
                    state.Resources[(ResourceType)res.Type] = res.Amount;
                }
            }

            // Restore city
            state.City = City?.ToCityProgression() ?? new CityProgression();

            return state;
        }
    }

    [Serializable]
    public class SerializableSystemState
    {
        public int Type;
        public int VillagerCount;
        public int MaxVillagers;
        public float ProductionPerVillager;
        public float CycleTime;
        public int Tier;
        public bool IsAutomated;
        public float AccumulatedProgress;

        public SerializableSystemState() { }

        public SerializableSystemState(ProductionSystemState state)
        {
            Type = (int)state.Type;
            VillagerCount = state.VillagerCount;
            MaxVillagers = state.MaxVillagers;
            ProductionPerVillager = state.ProductionPerVillager;
            CycleTime = state.CycleTime;
            Tier = state.Tier;
            IsAutomated = state.IsAutomated;
            AccumulatedProgress = state.AccumulatedProgress;
        }

        public ProductionSystemState ToProductionSystemState()
        {
            return new ProductionSystemState
            {
                Type = (SystemType)Type,
                VillagerCount = VillagerCount,
                MaxVillagers = MaxVillagers,
                ProductionPerVillager = ProductionPerVillager,
                CycleTime = CycleTime,
                Tier = Tier,
                IsAutomated = IsAutomated,
                AccumulatedProgress = AccumulatedProgress
            };
        }
    }

    [Serializable]
    public class SerializableResource
    {
        public int Type;
        public double Amount;
    }

    [Serializable]
    public class SerializableCityProgression
    {
        public int CurrentEra;
        public SerializableBuildingTier[] BuildingTiers;
        public string[] UnlockedSlots;

        public SerializableCityProgression() { }

        public SerializableCityProgression(CityProgression city)
        {
            CurrentEra = (int)city.CurrentEra;
            UnlockedSlots = city.UnlockedSlots.ToArray();

            BuildingTiers = new SerializableBuildingTier[city.BuildingTiers.Count];
            int i = 0;
            foreach (var kvp in city.BuildingTiers)
            {
                BuildingTiers[i++] = new SerializableBuildingTier 
                { 
                    BuildingId = kvp.Key, 
                    Tier = kvp.Value 
                };
            }
        }

        public CityProgression ToCityProgression()
        {
            var city = new CityProgression
            {
                CurrentEra = (Era)CurrentEra,
                UnlockedSlots = new System.Collections.Generic.List<string>(UnlockedSlots ?? Array.Empty<string>())
            };

            if (BuildingTiers != null)
            {
                foreach (var bt in BuildingTiers)
                {
                    city.BuildingTiers[bt.BuildingId] = bt.Tier;
                }
            }

            return city;
        }
    }

    [Serializable]
    public class SerializableBuildingTier
    {
        public string BuildingId;
        public int Tier;
    }
}
