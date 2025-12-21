using System.Collections.Generic;
using UnityEngine;
using AsgardFoundry.Core;
using AsgardFoundry.Data;

namespace AsgardFoundry.Automation
{
    /// <summary>
    /// Manages automation unlocks and applies their effects.
    /// Handles prerequisite checking and unlock flow.
    /// </summary>
    public class AutomationManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private AutomationDefAsset[] availableAutomations;

        /// <summary>
        /// Check if an automation can be unlocked.
        /// </summary>
        public bool CanUnlock(string roleId)
        {
            if (GameManager.Instance == null) return false;

            var state = GameManager.Instance.State;
            var def = GetAutomationDef(roleId);
            if (def == null) return false;

            // Already unlocked?
            if (state.UnlockedAutomations.Contains(roleId))
                return false;

            // Check system tier
            if (state.Systems.TryGetValue(def.TargetSystem, out var system))
            {
                if (system.Tier < def.RequiredSystemTier)
                    return false;
                if (system.VillagerCount < def.RequiredVillagerCount)
                    return false;
            }
            else
            {
                return false;
            }

            // Check resource costs
            if (def.UnlockCosts != null)
            {
                foreach (var cost in def.UnlockCosts)
                {
                    if (state.GetResource(cost.Resource) < cost.Amount)
                        return false;
                }
            }

            // Check prerequisites
            if (def.PrerequisiteAutomations != null)
            {
                foreach (var prereq in def.PrerequisiteAutomations)
                {
                    if (!state.UnlockedAutomations.Contains(prereq))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Attempt to unlock an automation.
        /// </summary>
        public bool TryUnlock(string roleId)
        {
            if (!CanUnlock(roleId)) return false;

            var state = GameManager.Instance.State;
            var def = GetAutomationDef(roleId);

            // Spend resources
            if (def.UnlockCosts != null)
            {
                foreach (var cost in def.UnlockCosts)
                {
                    if (!state.TrySpendResource(cost.Resource, cost.Amount))
                    {
                        Debug.LogError($"[AutomationManager] Failed to spend {cost.Amount} {cost.Resource}");
                        return false;
                    }
                }
            }

            // Mark as unlocked
            state.UnlockedAutomations.Add(roleId);

            // Apply automation effect
            if (state.Systems.TryGetValue(def.TargetSystem, out var system))
            {
                system.IsAutomated = true;
            }

            Debug.Log($"[AutomationManager] Unlocked: {def.DisplayName}");
            return true;
        }

        /// <summary>
        /// Get an automation definition by ID.
        /// </summary>
        private AutomationDef GetAutomationDef(string roleId)
        {
            if (availableAutomations == null) return null;

            foreach (var asset in availableAutomations)
            {
                if (asset != null && asset.Definition.RoleId == roleId)
                    return asset.Definition;
            }

            return null;
        }

        /// <summary>
        /// Get all automation definitions.
        /// </summary>
        public IEnumerable<AutomationDef> GetAllAutomations()
        {
            if (availableAutomations == null) yield break;

            foreach (var asset in availableAutomations)
            {
                if (asset != null)
                    yield return asset.Definition;
            }
        }
    }

    /// <summary>
    /// ScriptableObject wrapper for AutomationDef.
    /// Allows defining automations in the Unity editor.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAutomation", menuName = "Asgard Foundry/Automation Definition")]
    public class AutomationDefAsset : ScriptableObject
    {
        public AutomationDef Definition;
    }
}
