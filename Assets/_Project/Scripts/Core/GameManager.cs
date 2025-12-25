using UnityEngine;
using AsgardFoundry.Data;
using AsgardFoundry.Persistence;

namespace AsgardFoundry.Core
{
    /// <summary>
    /// Central game manager singleton. Initializes and coordinates all systems.
    /// Entry point for the game loop.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private bool autoSaveEnabled = true;
        [SerializeField] private float autoSaveInterval = 30f;

        /// <summary>Current game state (single source of truth).</summary>
        public GameState State { get; private set; }

        private float autoSaveTimer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeGame();
        }

        private void InitializeGame()
        {
            // Try to load existing save, otherwise create new game
            State = SaveManager.LoadGame();
            if (State == null)
            {
                State = GameState.CreateNew();
                Debug.Log("[GameManager] Created new game state");
            }
            else
            {
                Debug.Log("[GameManager] Loaded existing save");
                // Fix any incomplete data from old saves
                State.EnsureSystemsInitialized();
                ProcessOfflineProgress();
            }

            EventBus.GameLoaded();
        }

        private void ProcessOfflineProgress()
        {
            float offlineSeconds = TimeManager.GetSecondsSince(State.LastSaveTimestamp);
            
            if (offlineSeconds > 0 && offlineSeconds < 86400) // Max 24 hours
            {
                Debug.Log($"[GameManager] Processing {offlineSeconds:F0}s of offline progress");

                // Calculate offline production for each system
                foreach (var kvp in State.Systems)
                {
                    var system = kvp.Value;
                    float produced = system.CalculateOfflineProduction(offlineSeconds);

                    if (produced > 0)
                    {
                        // Map system to resource (simplified)
                        ResourceType resource = GetResourceForSystem(system.Type);
                        State.AddResource(resource, produced);
                        Debug.Log($"[Offline] {system.Type}: +{produced:F0} {resource}");
                    }
                }

                // Regenerate EITR while offline
                float eitrRegen = State.Eitr.RegenRate * offlineSeconds;
                State.Eitr.CurrentEitr = Mathf.Min(
                    State.Eitr.CurrentEitr + eitrRegen,
                    State.Eitr.MaxEitr
                );
                State.Eitr.IsOverheated = false;
            }
        }

        private void Update()
        {
            float dt = TimeManager.Instance?.DeltaTime ?? Time.deltaTime;

            // Update EITR
            bool wasOverheated = State.Eitr.IsOverheated;
            State.Eitr.Update(dt);
            
            if (wasOverheated && !State.Eitr.IsOverheated)
                EventBus.EitrRecovered();

            // Update production systems
            foreach (var kvp in State.Systems)
            {
                var system = kvp.Value;
                float produced = system.Update(dt);

                if (produced > 0)
                {
                    ResourceType resource = GetResourceForSystem(system.Type);
                    State.AddResource(resource, produced);
                    Debug.Log($"[Production] {system.Type}: +{produced:F1} {resource} (Villagers: {system.VillagerCount}, Progress: {system.AccumulatedProgress:F1}/{system.CycleTime})");
                    EventBus.ResourceChanged(resource, State.GetResource(resource));
                    EventBus.ProductionCycleComplete(system.Type, produced);
                }
            }

            // Track play time
            State.TotalPlayTime += dt;

            // Auto-save
            if (autoSaveEnabled)
            {
                autoSaveTimer += dt;
                if (autoSaveTimer >= autoSaveInterval)
                {
                    autoSaveTimer = 0f;
                    SaveGame();
                }
            }
        }

        /// <summary>
        /// Attempt to generate a villager for the currently selected system.
        /// </summary>
        public bool TryGenerateVillager()
        {
            Debug.Log($"[GameManager] TryGenerateVillager called. EITR: {State.Eitr.CurrentEitr}/{State.Eitr.MaxEitr}, CanGenerate: {State.Eitr.CanGenerate}, Overheated: {State.Eitr.IsOverheated}");
            
            if (!State.Eitr.CanGenerate)
            {
                string reason = State.Eitr.IsOverheated ? "Overheated" : "Not enough EITR";
                Debug.LogWarning($"[GameManager] Cannot generate: {reason}");
                EventBus.VillagerGenerationFailed(reason);
                return false;
            }

            var targetSystem = State.GetSelectedSystem();
            Debug.Log($"[GameManager] Selected system: {State.SelectedAssignment}, targetSystem is null: {targetSystem == null}");
            
            if (targetSystem == null)
            {
                Debug.LogError("[GameManager] targetSystem is NULL! Check GetSelectedSystem()");
                EventBus.VillagerGenerationFailed("No system selected");
                return false;
            }
            
            if (!targetSystem.CanAcceptVillager)
            {
                Debug.LogWarning($"[GameManager] System {targetSystem.Type} cannot accept villager");
                EventBus.VillagerGenerationFailed("System full");
                return false;
            }

            // Consume EITR and add villager
            if (State.Eitr.TryConsume())
            {
                targetSystem.TryAddVillager();
                Debug.Log($"[GameManager] Villager generated! New EITR: {State.Eitr.CurrentEitr}, Villagers in {targetSystem.Type}: {targetSystem.VillagerCount}");
                EventBus.VillagerGenerated(targetSystem.Type);

                if (State.Eitr.IsOverheated)
                    EventBus.EitrOverheated();

                return true;
            }
            
            Debug.LogWarning("[GameManager] TryConsume returned false");
            return false;
        }

        /// <summary>
        /// Set the pre-assignment target for new villagers.
        /// </summary>
        public void SetAssignmentTarget(SystemType target)
        {
            State.SelectedAssignment = target;
        }

        /// <summary>
        /// Register a tap action for the active system view.
        /// </summary>
        public void RegisterTap(SystemType system, float tapValue = 1f)
        {
            if (State.Systems.TryGetValue(system, out var systemState))
            {
                systemState.RegisterTap(tapValue);
            }
        }

        /// <summary>
        /// Reset the game to a fresh state (delete save and reinitialize).
        /// </summary>
        public void ResetGame()
        {
            SaveManager.DeleteSave();
            State = GameState.CreateNew();
            Debug.Log("[GameManager] Game reset to new state");
            EventBus.GameLoaded();
        }

        /// <summary>
        /// Save the game to disk.
        /// </summary>
        public void SaveGame()
        {
            State.LastSaveTimestamp = TimeManager.GetUnixTimestamp();
            SaveManager.SaveGame(State);
            EventBus.GameSaved();
        }

        /// <summary>
        /// Get the primary resource produced by a system type.
        /// </summary>
        public static ResourceType GetResourceForSystem(SystemType system)
        {
            return system switch
            {
                SystemType.Mining => ResourceType.Stone,
                SystemType.Woodcutting => ResourceType.Wood,
                SystemType.Farming => ResourceType.Food,
                SystemType.Smithing => ResourceType.Metal,
                SystemType.Market => ResourceType.Gold,
                _ => ResourceType.Stone
            };
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveGame();
            }
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                EventBus.ClearAll();
            }
        }
    }
}
