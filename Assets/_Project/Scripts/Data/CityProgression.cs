using System;
using System.Collections.Generic;

namespace AsgardFoundry.Data
{
    /// <summary>
    /// City progression state tracking era, building tiers, and unlocked slots.
    /// </summary>
    [Serializable]
    public class CityProgression
    {
        /// <summary>Current era (affects available resources and visuals).</summary>
        public Era CurrentEra = Era.StoneAge;

        /// <summary>Building tier levels (building ID -> tier 1-3).</summary>
        public Dictionary<string, int> BuildingTiers = new Dictionary<string, int>();

        /// <summary>List of unlocked building slot IDs.</summary>
        public List<string> UnlockedSlots = new List<string>();

        /// <summary>
        /// Get the tier of a specific building.
        /// Returns 0 if not built yet.
        /// </summary>
        public int GetBuildingTier(string buildingId)
        {
            return BuildingTiers.TryGetValue(buildingId, out int tier) ? tier : 0;
        }

        /// <summary>
        /// Upgrade a building to the next tier.
        /// </summary>
        public bool TryUpgradeBuilding(string buildingId, int maxTier = 3)
        {
            int currentTier = GetBuildingTier(buildingId);
            if (currentTier >= maxTier) return false;

            BuildingTiers[buildingId] = currentTier + 1;
            return true;
        }

        /// <summary>
        /// Check if a building slot is unlocked.
        /// </summary>
        public bool IsSlotUnlocked(string slotId)
        {
            return UnlockedSlots.Contains(slotId);
        }

        /// <summary>
        /// Unlock a new building slot.
        /// </summary>
        public bool TryUnlockSlot(string slotId)
        {
            if (IsSlotUnlocked(slotId)) return false;
            UnlockedSlots.Add(slotId);
            return true;
        }

        /// <summary>
        /// Check if the player can advance to the next era.
        /// Override this with actual requirements.
        /// </summary>
        public bool CanAdvanceEra()
        {
            // TODO: Add real requirements (e.g., all buildings at tier 2+)
            return true;
        }

        /// <summary>
        /// Advance to the next era if possible.
        /// </summary>
        public bool TryAdvanceEra()
        {
            if (!CanAdvanceEra()) return false;
            if (CurrentEra >= Era.Medieval) return false;

            CurrentEra = (Era)((int)CurrentEra + 1);
            return true;
        }
    }
}
