using System;
using System.Collections.Generic;

namespace AsgardFoundry.Data
{
    /// <summary>
    /// Central game state container. This is the single source of truth
    /// for all persistent game data.
    /// </summary>
    [Serializable]
    public class GameState
    {
        /// <summary>EITR (energy) for villager generation.</summary>
        public EitrState Eitr = new EitrState();

        /// <summary>Production system states keyed by type.</summary>
        public Dictionary<SystemType, ProductionSystemState> Systems = 
            new Dictionary<SystemType, ProductionSystemState>();

        /// <summary>Current resource amounts keyed by type.</summary>
        public Dictionary<ResourceType, double> Resources = 
            new Dictionary<ResourceType, double>();

        /// <summary>List of unlocked automation role IDs.</summary>
        public List<string> UnlockedAutomations = new List<string>();

        /// <summary>City progression state.</summary>
        public CityProgression City = new CityProgression();

        /// <summary>Currently selected system for villager pre-assignment.</summary>
        public SystemType SelectedAssignment = SystemType.Mining;

        /// <summary>Unix timestamp of last save (for offline calculation).</summary>
        public long LastSaveTimestamp;

        /// <summary>Total play time in seconds.</summary>
        public double TotalPlayTime;

        /// <summary>
        /// Initialize a new game state with default values.
        /// </summary>
        public static GameState CreateNew()
        {
            var state = new GameState();

            // Initialize production systems
            state.Systems[SystemType.Mining] = new ProductionSystemState 
            { 
                Type = SystemType.Mining, 
                CycleTime = 10f,
                ProductionPerVillager = 5f 
            };
            state.Systems[SystemType.Woodcutting] = new ProductionSystemState 
            { 
                Type = SystemType.Woodcutting, 
                CycleTime = 12f,
                ProductionPerVillager = 4f 
            };
            state.Systems[SystemType.Farming] = new ProductionSystemState 
            { 
                Type = SystemType.Farming, 
                CycleTime = 60f,  // Long cycle
                ProductionPerVillager = 20f 
            };
            state.Systems[SystemType.Smithing] = new ProductionSystemState 
            { 
                Type = SystemType.Smithing, 
                CycleTime = 30f,
                ProductionPerVillager = 2f 
            };
            state.Systems[SystemType.Market] = new ProductionSystemState 
            { 
                Type = SystemType.Market, 
                CycleTime = 15f,
                ProductionPerVillager = 10f 
            };

            // Initialize resources
            foreach (ResourceType resource in Enum.GetValues(typeof(ResourceType)))
            {
                state.Resources[resource] = 0;
            }

            // Unlock initial building slots
            state.City.UnlockedSlots.Add("mine_slot");
            state.City.UnlockedSlots.Add("lumber_slot");
            state.City.BuildingTiers["mine"] = 1;
            state.City.BuildingTiers["lumber"] = 1;

            state.LastSaveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            return state;
        }

        /// <summary>
        /// Ensures all production systems are initialized.
        /// Call this after loading to fix incomplete saves.
        /// </summary>
        public void EnsureSystemsInitialized()
        {
            if (!Systems.ContainsKey(SystemType.Mining))
            {
                Systems[SystemType.Mining] = new ProductionSystemState 
                { 
                    Type = SystemType.Mining, 
                    CycleTime = 10f,
                    ProductionPerVillager = 5f 
                };
            }
            if (!Systems.ContainsKey(SystemType.Woodcutting))
            {
                Systems[SystemType.Woodcutting] = new ProductionSystemState 
                { 
                    Type = SystemType.Woodcutting, 
                    CycleTime = 12f,
                    ProductionPerVillager = 4f 
                };
            }
            if (!Systems.ContainsKey(SystemType.Farming))
            {
                Systems[SystemType.Farming] = new ProductionSystemState 
                { 
                    Type = SystemType.Farming, 
                    CycleTime = 60f,
                    ProductionPerVillager = 20f 
                };
            }
            if (!Systems.ContainsKey(SystemType.Smithing))
            {
                Systems[SystemType.Smithing] = new ProductionSystemState 
                { 
                    Type = SystemType.Smithing, 
                    CycleTime = 30f,
                    ProductionPerVillager = 2f 
                };
            }
            if (!Systems.ContainsKey(SystemType.Market))
            {
                Systems[SystemType.Market] = new ProductionSystemState 
                { 
                    Type = SystemType.Market, 
                    CycleTime = 15f,
                    ProductionPerVillager = 10f 
                };
            }
            
            // Repair systems with invalid values
            foreach (var system in Systems.Values)
            {
                if (system.MaxVillagers <= 0)
                {
                    system.MaxVillagers = 10; // Default value
                    UnityEngine.Debug.Log($"[GameState] Repaired MaxVillagers for {system.Type}");
                }
                if (system.CycleTime <= 0)
                {
                    system.CycleTime = 10f;
                }
                if (system.ProductionPerVillager <= 0)
                {
                    system.ProductionPerVillager = 5f;
                }
            }
        }

        /// <summary>
        /// Get a resource amount.
        /// </summary>
        public double GetResource(ResourceType type)
        {
            return Resources.TryGetValue(type, out double amount) ? amount : 0;
        }

        /// <summary>
        /// Add to a resource amount.
        /// </summary>
        public void AddResource(ResourceType type, double amount)
        {
            if (!Resources.ContainsKey(type))
                Resources[type] = 0;
            Resources[type] += amount;
        }

        /// <summary>
        /// Try to spend a resource amount.
        /// </summary>
        public bool TrySpendResource(ResourceType type, double amount)
        {
            if (GetResource(type) < amount) return false;
            Resources[type] -= amount;
            return true;
        }

        /// <summary>
        /// Get the currently selected production system for assignment.
        /// </summary>
        public ProductionSystemState GetSelectedSystem()
        {
            return Systems.TryGetValue(SelectedAssignment, out var system) ? system : null;
        }
    }
}
