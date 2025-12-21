using System;

namespace AsgardFoundry.Data
{
    /// <summary>
    /// State of a single production system (Mining, Woodcutting, etc.).
    /// Handles villager assignment and production calculations.
    /// </summary>
    [Serializable]
    public class ProductionSystemState
    {
        /// <summary>Which system this state represents.</summary>
        public SystemType Type;

        /// <summary>Current number of villagers assigned to this system.</summary>
        public int VillagerCount = 0;

        /// <summary>Maximum villagers this system can hold (upgradable).</summary>
        public int MaxVillagers = 10;

        /// <summary>Units produced per villager per cycle.</summary>
        public float ProductionPerVillager = 5f;

        /// <summary>Duration of one production cycle in seconds.</summary>
        public float CycleTime = 20f;

        /// <summary>Current building upgrade tier (1-3).</summary>
        public int Tier = 1;

        /// <summary>Whether automation (manager) is active for this system.</summary>
        public bool IsAutomated = false;

        /// <summary>Accumulated progress within current cycle (0 to CycleTime).</summary>
        public float AccumulatedProgress = 0f;

        /// <summary>Tap bonus accumulated this cycle.</summary>
        public float TapBonus = 0f;

        /// <summary>
        /// Calculate production per second for this system.
        /// </summary>
        public float ProductionPerSecond => 
            (VillagerCount * ProductionPerVillager) / CycleTime;

        /// <summary>
        /// Whether this system has room for more villagers.
        /// </summary>
        public bool CanAcceptVillager => VillagerCount < MaxVillagers;

        /// <summary>
        /// Add a villager to this system.
        /// Returns true if successful.
        /// </summary>
        public bool TryAddVillager()
        {
            if (!CanAcceptVillager) return false;
            VillagerCount++;
            return true;
        }

        /// <summary>
        /// Register a tap action for bonus production.
        /// </summary>
        public void RegisterTap(float tapValue = 1f)
        {
            TapBonus += tapValue;
        }

        /// <summary>
        /// Update production cycle progress.
        /// Returns amount produced this update (including tap bonus).
        /// </summary>
        public float Update(float deltaTime)
        {
            if (VillagerCount <= 0) return 0f;

            AccumulatedProgress += deltaTime;
            float produced = 0f;

            while (AccumulatedProgress >= CycleTime)
            {
                AccumulatedProgress -= CycleTime;
                produced += (VillagerCount * ProductionPerVillager) + TapBonus;
                TapBonus = 0f;
            }

            return produced;
        }

        /// <summary>
        /// Calculate offline production for a given duration.
        /// </summary>
        public float CalculateOfflineProduction(float offlineSeconds)
        {
            if (VillagerCount <= 0) return 0f;

            int completedCycles = (int)(offlineSeconds / CycleTime);
            float baseProduction = completedCycles * VillagerCount * ProductionPerVillager;

            // Partial cycle progress
            float remainingTime = offlineSeconds - (completedCycles * CycleTime);
            AccumulatedProgress += remainingTime;

            return baseProduction;
        }
    }
}
