using System;

namespace AsgardFoundry.Automation
{
    /// <summary>
    /// Defines an automation role (diegetic manager) with unlock requirements.
    /// </summary>
    [Serializable]
    public class AutomationDef
    {
        /// <summary>Unique identifier for this automation.</summary>
        public string RoleId;

        /// <summary>Display name shown in UI.</summary>
        public string DisplayName;

        /// <summary>Description of what this automation does.</summary>
        public string Description;

        /// <summary>Which system this automation affects.</summary>
        public Data.SystemType TargetSystem;

        /// <summary>Resource costs to unlock.</summary>
        public ResourceCost[] UnlockCosts;

        /// <summary>Minimum tier required for the target system.</summary>
        public int RequiredSystemTier = 1;

        /// <summary>Minimum villagers required in the target system.</summary>
        public int RequiredVillagerCount = 0;

        /// <summary>Other automations that must be unlocked first.</summary>
        public string[] PrerequisiteAutomations;
    }

    /// <summary>
    /// Represents a resource cost for purchases/upgrades.
    /// </summary>
    [Serializable]
    public class ResourceCost
    {
        public Data.ResourceType Resource;
        public double Amount;
    }
}
