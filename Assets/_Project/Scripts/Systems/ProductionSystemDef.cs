using UnityEngine;
using AsgardFoundry.Data;

namespace AsgardFoundry.Systems
{
    /// <summary>
    /// ScriptableObject defining a production system's base configuration.
    /// These values are used to initialize ProductionSystemState.
    /// </summary>
    [CreateAssetMenu(fileName = "NewProductionSystem", menuName = "Asgard Foundry/Production System Definition")]
    public class ProductionSystemDef : ScriptableObject
    {
        [Header("Identity")]
        public SystemType Type;
        public string DisplayName;
        public string Description;

        [Header("Production")]
        [Tooltip("Seconds per production cycle")]
        public float BaseCycleTime = 20f;

        [Tooltip("Units produced per villager per cycle")]
        public float BaseProductionPerVillager = 5f;

        [Tooltip("Primary resource produced")]
        public ResourceType OutputResource;

        [Header("Capacity")]
        [Tooltip("Starting max villagers")]
        public int BaseMaxVillagers = 10;

        [Tooltip("Max villagers per tier upgrade")]
        public int VillagersPerTier = 5;

        [Header("Upgrades")]
        [Tooltip("Production multiplier per tier")]
        public float TierProductionMultiplier = 1.5f;

        [Tooltip("Cycle time reduction per tier (multiplicative)")]
        public float TierCycleTimeMultiplier = 0.9f;

        [Header("Interconnections")]
        [Tooltip("Resources required for production (e.g., smithing needs metal + wood)")]
        public ResourceRequirement[] InputRequirements;

        [Header("Visuals")]
        public Sprite Icon;
        public Sprite[] TierSprites;

        /// <summary>
        /// Create a new ProductionSystemState from this definition.
        /// </summary>
        public ProductionSystemState CreateState()
        {
            return new ProductionSystemState
            {
                Type = Type,
                CycleTime = BaseCycleTime,
                ProductionPerVillager = BaseProductionPerVillager,
                MaxVillagers = BaseMaxVillagers,
                Tier = 1
            };
        }

        /// <summary>
        /// Apply tier upgrades to a system state.
        /// </summary>
        public void ApplyTierUpgrade(ProductionSystemState state)
        {
            state.Tier++;
            state.MaxVillagers += VillagersPerTier;
            state.ProductionPerVillager *= TierProductionMultiplier;
            state.CycleTime *= TierCycleTimeMultiplier;
        }
    }

    /// <summary>
    /// Represents a resource requirement for production.
    /// </summary>
    [System.Serializable]
    public class ResourceRequirement
    {
        public ResourceType Resource;
        public float AmountPerCycle;
    }
}
