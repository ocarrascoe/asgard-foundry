using UnityEngine;
using AsgardFoundry.Data;

namespace AsgardFoundry.ScriptableObjects
{
    /// <summary>
    /// Global game configuration ScriptableObject.
    /// Contains balance values and tuning parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Asgard Foundry/Game Configuration")]
    public class GameConfig : ScriptableObject
    {
        [Header("EITR (Energy) Settings")]
        [Tooltip("Starting max EITR")]
        public float StartingMaxEitr = 100f;

        [Tooltip("Base EITR regeneration rate (per second)")]
        public float BaseEitrRegenRate = 5f;

        [Tooltip("EITR cost per villager")]
        public float EitrCostPerVillager = 10f;

        [Tooltip("Overheat cooldown duration (seconds)")]
        public float OverheatCooldownDuration = 3f;

        [Header("Offline Progress")]
        [Tooltip("Maximum offline time to calculate (hours)")]
        public float MaxOfflineHours = 24f;

        [Tooltip("Offline production efficiency (1.0 = 100%)")]
        [Range(0f, 1f)]
        public float OfflineEfficiency = 0.75f;

        [Header("Tap Bonus")]
        [Tooltip("Base bonus per tap")]
        public float BaseTapBonus = 2f;

        [Tooltip("Tap cooldown (seconds)")]
        public float TapCooldown = 0f;

        [Header("Save System")]
        [Tooltip("Auto-save interval (seconds)")]
        public float AutoSaveInterval = 30f;

        [Header("Starting Resources")]
        public ResourceAmount[] StartingResources;

        /// <summary>
        /// Apply this config to a new game state.
        /// </summary>
        public void ApplyToState(GameState state)
        {
            state.Eitr.MaxEitr = StartingMaxEitr;
            state.Eitr.CurrentEitr = StartingMaxEitr;
            state.Eitr.RegenRate = BaseEitrRegenRate;
            state.Eitr.CostPerVillager = EitrCostPerVillager;
            state.Eitr.OverheatCooldownDuration = OverheatCooldownDuration;

            if (StartingResources != null)
            {
                foreach (var res in StartingResources)
                {
                    state.AddResource(res.Type, res.Amount);
                }
            }
        }
    }

    [System.Serializable]
    public class ResourceAmount
    {
        public ResourceType Type;
        public double Amount;
    }
}
