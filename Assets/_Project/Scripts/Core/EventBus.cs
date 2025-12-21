using System;

namespace AsgardFoundry.Core
{
    /// <summary>
    /// Simple event bus for decoupled communication between systems.
    /// Uses C# events for type-safe subscriptions.
    /// </summary>
    public static class EventBus
    {
        // ═══════════════════════════════════════════════════════════
        // VILLAGER EVENTS
        // ═══════════════════════════════════════════════════════════
        
        /// <summary>Fired when a villager is generated.</summary>
        public static event Action<Data.SystemType> OnVillagerGenerated;
        
        /// <summary>Fired when villager generation fails (overheat/full).</summary>
        public static event Action<string> OnVillagerGenerationFailed;

        public static void VillagerGenerated(Data.SystemType targetSystem) 
            => OnVillagerGenerated?.Invoke(targetSystem);
        
        public static void VillagerGenerationFailed(string reason) 
            => OnVillagerGenerationFailed?.Invoke(reason);

        // ═══════════════════════════════════════════════════════════
        // RESOURCE EVENTS
        // ═══════════════════════════════════════════════════════════
        
        /// <summary>Fired when a resource amount changes.</summary>
        public static event Action<Data.ResourceType, double> OnResourceChanged;
        
        /// <summary>Fired when production completes a cycle.</summary>
        public static event Action<Data.SystemType, float> OnProductionCycleComplete;

        public static void ResourceChanged(Data.ResourceType type, double newAmount) 
            => OnResourceChanged?.Invoke(type, newAmount);
        
        public static void ProductionCycleComplete(Data.SystemType system, float amount) 
            => OnProductionCycleComplete?.Invoke(system, amount);

        // ═══════════════════════════════════════════════════════════
        // EITR EVENTS
        // ═══════════════════════════════════════════════════════════
        
        /// <summary>Fired when EITR becomes overheated.</summary>
        public static event Action OnEitrOverheated;
        
        /// <summary>Fired when EITR recovers from overheat.</summary>
        public static event Action OnEitrRecovered;

        public static void EitrOverheated() => OnEitrOverheated?.Invoke();
        public static void EitrRecovered() => OnEitrRecovered?.Invoke();

        // ═══════════════════════════════════════════════════════════
        // CITY EVENTS
        // ═══════════════════════════════════════════════════════════
        
        /// <summary>Fired when a building is upgraded.</summary>
        public static event Action<string, int> OnBuildingUpgraded;
        
        /// <summary>Fired when the era advances.</summary>
        public static event Action<Data.Era> OnEraAdvanced;

        public static void BuildingUpgraded(string buildingId, int newTier) 
            => OnBuildingUpgraded?.Invoke(buildingId, newTier);
        
        public static void EraAdvanced(Data.Era newEra) 
            => OnEraAdvanced?.Invoke(newEra);

        // ═══════════════════════════════════════════════════════════
        // GAME STATE EVENTS
        // ═══════════════════════════════════════════════════════════
        
        /// <summary>Fired when game is saved.</summary>
        public static event Action OnGameSaved;
        
        /// <summary>Fired when game is loaded.</summary>
        public static event Action OnGameLoaded;

        public static void GameSaved() => OnGameSaved?.Invoke();
        public static void GameLoaded() => OnGameLoaded?.Invoke();

        /// <summary>
        /// Clear all event subscriptions. Call this on scene unload to prevent leaks.
        /// </summary>
        public static void ClearAll()
        {
            OnVillagerGenerated = null;
            OnVillagerGenerationFailed = null;
            OnResourceChanged = null;
            OnProductionCycleComplete = null;
            OnEitrOverheated = null;
            OnEitrRecovered = null;
            OnBuildingUpgraded = null;
            OnEraAdvanced = null;
            OnGameSaved = null;
            OnGameLoaded = null;
        }
    }
}
