using System;

namespace AsgardFoundry.Data
{
    /// <summary>
    /// EITR (Energy) state for villager generation.
    /// Uses an overheat mechanic similar to weapon systems.
    /// </summary>
    [Serializable]
    public class EitrState
    {
        /// <summary>Current EITR amount (0 to MaxEitr).</summary>
        public float CurrentEitr = 100f;

        /// <summary>Maximum EITR capacity (upgradable).</summary>
        public float MaxEitr = 100f;

        /// <summary>EITR regeneration rate (units per second).</summary>
        public float RegenRate = 5f;

        /// <summary>EITR cost per villager generated.</summary>
        public float CostPerVillager = 10f;

        /// <summary>True when EITR is depleted and in cooldown.</summary>
        public bool IsOverheated = false;

        /// <summary>Remaining cooldown time before regen resumes (seconds).</summary>
        public float OverheatCooldownRemaining = 0f;

        /// <summary>Total overheat cooldown duration (seconds).</summary>
        public float OverheatCooldownDuration = 3f;

        /// <summary>
        /// Check if we can generate a villager.
        /// </summary>
        public bool CanGenerate => !IsOverheated && CurrentEitr >= CostPerVillager;

        /// <summary>
        /// Consume EITR to generate a villager.
        /// Returns true if successful.
        /// </summary>
        public bool TryConsume()
        {
            if (!CanGenerate) return false;

            CurrentEitr -= CostPerVillager;

            if (CurrentEitr <= 0f)
            {
                CurrentEitr = 0f;
                IsOverheated = true;
                OverheatCooldownRemaining = OverheatCooldownDuration;
            }

            return true;
        }

        /// <summary>
        /// Update EITR regeneration over time.
        /// </summary>
        public void Update(float deltaTime)
        {
            if (IsOverheated)
            {
                OverheatCooldownRemaining -= deltaTime;
                if (OverheatCooldownRemaining <= 0f)
                {
                    IsOverheated = false;
                    OverheatCooldownRemaining = 0f;
                }
            }
            else if (CurrentEitr < MaxEitr)
            {
                CurrentEitr += RegenRate * deltaTime;
                if (CurrentEitr > MaxEitr)
                {
                    CurrentEitr = MaxEitr;
                }
            }
        }
    }
}
