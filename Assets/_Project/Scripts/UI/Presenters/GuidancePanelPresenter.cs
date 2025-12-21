using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AsgardFoundry.Core;
using AsgardFoundry.Data;

namespace AsgardFoundry.UI.Presenters
{
    /// <summary>
    /// Presents the "What You Can Do Now" guidance panel,
    /// showing available actions, blocked items, and requirements.
    /// </summary>
    public class GuidancePanelPresenter : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Button toggleButton;
        [SerializeField] private Button closeButton;

        [Header("Content")]
        [SerializeField] private Transform availableActionsContainer;
        [SerializeField] private Transform blockedActionsContainer;
        [SerializeField] private GameObject actionItemPrefab;

        [Header("Settings")]
        [SerializeField] private float refreshInterval = 2f;

        private bool isOpen;
        private float refreshTimer;
        private List<GameObject> spawnedItems = new List<GameObject>();

        private void Start()
        {
            if (toggleButton != null)
                toggleButton.onClick.AddListener(Toggle);

            if (closeButton != null)
                closeButton.onClick.AddListener(Close);

            panelRoot?.SetActive(false);
            RefreshGuidance();
        }

        private void Update()
        {
            if (!isOpen) return;

            refreshTimer += Time.deltaTime;
            if (refreshTimer >= refreshInterval)
            {
                refreshTimer = 0f;
                RefreshGuidance();
            }
        }

        public void Toggle()
        {
            if (isOpen) Close();
            else Open();
        }

        public void Open()
        {
            isOpen = true;
            panelRoot?.SetActive(true);
            RefreshGuidance();
        }

        public void Close()
        {
            isOpen = false;
            panelRoot?.SetActive(false);
        }

        private void RefreshGuidance()
        {
            if (GameManager.Instance == null) return;

            ClearItems();
            var state = GameManager.Instance.State;

            // Available actions
            CheckAvailableUpgrades(state);
            CheckAvailableAutomations(state);
            CheckAvailableSlots(state);

            // Blocked actions
            CheckBlockedItems(state);
        }

        private void CheckAvailableUpgrades(GameState state)
        {
            foreach (var kvp in state.Systems)
            {
                var system = kvp.Value;
                if (system.Tier < 3)
                {
                    // Check if we have resources for upgrade (simplified)
                    double stoneCost = system.Tier * 100;
                    double woodCost = system.Tier * 50;

                    if (state.GetResource(ResourceType.Stone) >= stoneCost &&
                        state.GetResource(ResourceType.Wood) >= woodCost)
                    {
                        SpawnActionItem(
                            availableActionsContainer,
                            $"Upgrade {kvp.Key} to Tier {system.Tier + 1}",
                            $"Cost: {stoneCost} Stone, {woodCost} Wood",
                            true
                        );
                    }
                }
            }
        }

        private void CheckAvailableAutomations(GameState state)
        {
            // Check for unlockable automation roles
            // This would check against AutomationDef ScriptableObjects
            // Simplified placeholder logic
            if (!state.UnlockedAutomations.Contains("mining_foreman"))
            {
                if (state.Systems.TryGetValue(SystemType.Mining, out var mining))
                {
                    if (mining.VillagerCount >= 5)
                    {
                        SpawnActionItem(
                            availableActionsContainer,
                            "Hire Mining Foreman",
                            "Automates mining production",
                            true
                        );
                    }
                }
            }
        }

        private void CheckAvailableSlots(GameState state)
        {
            // Check for unlockable building slots
            if (!state.City.IsSlotUnlocked("farm_slot"))
            {
                if (state.GetResource(ResourceType.Wood) >= 200)
                {
                    SpawnActionItem(
                        availableActionsContainer,
                        "Unlock Farm Slot",
                        "Enables food production",
                        true
                    );
                }
            }
        }

        private void CheckBlockedItems(GameState state)
        {
            // Show items that are blocked with requirements
            if (!state.City.IsSlotUnlocked("smithy_slot"))
            {
                SpawnActionItem(
                    blockedActionsContainer,
                    "Smithy (Locked)",
                    "Requires: Mine Tier 2, 500 Stone",
                    false
                );
            }

            // Era advancement
            if (state.City.CurrentEra < Era.Medieval)
            {
                SpawnActionItem(
                    blockedActionsContainer,
                    $"Advance to {(Era)((int)state.City.CurrentEra + 1)}",
                    "Requires: All buildings Tier 2+",
                    false
                );
            }
        }

        private void SpawnActionItem(Transform container, string title, string description, bool available)
        {
            if (actionItemPrefab == null || container == null) return;

            var item = Instantiate(actionItemPrefab, container);
            spawnedItems.Add(item);

            // Set up item (assumes prefab has TMP_Text children)
            var texts = item.GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 1) texts[0].text = title;
            if (texts.Length >= 2) texts[1].text = description;

            // Visual state
            var canvasGroup = item.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = available ? 1f : 0.5f;
            }
        }

        private void ClearItems()
        {
            foreach (var item in spawnedItems)
            {
                if (item != null) Destroy(item);
            }
            spawnedItems.Clear();
        }
    }
}
