using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AsgardFoundry.Core;
using AsgardFoundry.Data;
using AsgardFoundry.UI.Components;

namespace AsgardFoundry.UI.Presenters
{
    /// <summary>
    /// Manages the bottom HUD with pre-assignment tabs and generate button.
    /// Controls villager generation flow.
    /// </summary>
    public class BottomHUDPresenter : MonoBehaviour
    {
        [Header("Pre-Assignment Tabs")]
        [SerializeField] private ToggleGroup tabGroup;
        [SerializeField] private Toggle miningTab;
        [SerializeField] private Toggle woodcuttingTab;
        [SerializeField] private Toggle farmingTab;
        [SerializeField] private Toggle smithingTab;
        [SerializeField] private Toggle marketTab;

        [Header("Villager Counts (per System)")]
        [SerializeField] private TMP_Text miningCountText;
        [SerializeField] private TMP_Text woodcuttingCountText;
        [SerializeField] private TMP_Text farmingCountText;
        [SerializeField] private TMP_Text smithingCountText;
        [SerializeField] private TMP_Text marketCountText;

        [Header("Generate Button")]
        [SerializeField] private HoldButton generateButton;
        [SerializeField] private Image generateButtonImage;
        [SerializeField] private Color normalColor = new Color(0.8f, 0.2f, 0.2f);
        [SerializeField] private Color overheatColor = new Color(0.4f, 0.4f, 0.4f);

        [Header("Feedback")]
        [SerializeField] private Animator villagerSpawnAnimator;

        private void Start()
        {
            // Set up tab listeners
            SetupTab(miningTab, SystemType.Mining);
            SetupTab(woodcuttingTab, SystemType.Woodcutting);
            SetupTab(farmingTab, SystemType.Farming);
            SetupTab(smithingTab, SystemType.Smithing);
            SetupTab(marketTab, SystemType.Market);

            // Set up generate button
            if (generateButton != null)
            {
                generateButton.OnTrigger.AddListener(OnGenerateTriggered);
                Debug.Log("[BottomHUDPresenter] Generate button connected successfully");
            }
            else
            {
                Debug.LogError("[BottomHUDPresenter] Generate Button is NOT assigned! Drag it in the Inspector.");
            }

            // Subscribe to events
            EventBus.OnVillagerGenerated += HandleVillagerGenerated;
            EventBus.OnEitrOverheated += HandleOverheated;
            EventBus.OnEitrRecovered += HandleRecovered;

            // Initialize first tab
            if (miningTab != null)
                miningTab.isOn = true;
        }

        private void OnDestroy()
        {
            EventBus.OnVillagerGenerated -= HandleVillagerGenerated;
            EventBus.OnEitrOverheated -= HandleOverheated;
            EventBus.OnEitrRecovered -= HandleRecovered;
        }

        private void Update()
        {
            UpdateGenerateButtonVisual();
            RefreshVillagerCounts();
        }

        private void RefreshVillagerCounts()
        {
            if (GameManager.Instance == null) return;
            var state = GameManager.Instance.State;

            UpdateCountText(miningCountText, "Miners", SystemType.Mining, state);
            UpdateCountText(woodcuttingCountText, "Cutters", SystemType.Woodcutting, state);
            UpdateCountText(farmingCountText, "Farmers", SystemType.Farming, state);
            UpdateCountText(smithingCountText, "Smiths", SystemType.Smithing, state);
            UpdateCountText(marketCountText, "Traders", SystemType.Market, state);
        }

        private void UpdateCountText(TMP_Text text, string label, SystemType type, GameState state)
        {
            if (text == null) return;
            if (state.Systems.TryGetValue(type, out var system))
            {
                text.text = $"{label}: {system.VillagerCount}";
            }
        }

        private void SetupTab(Toggle tab, SystemType system)
        {
            if (tab == null) return;

            tab.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    GameManager.Instance?.SetAssignmentTarget(system);
                }
            });

            // Disable tabs for locked systems
            UpdateTabInteractability(tab, system);
        }

        private void UpdateTabInteractability(Toggle tab, SystemType system)
        {
            if (GameManager.Instance == null) return;

            var state = GameManager.Instance.State;
            
            // Check if system is unlocked (has building slot)
            string slotId = system.ToString().ToLower() + "_slot";
            bool isUnlocked = state.City.IsSlotUnlocked(slotId);

            // Mining and Woodcutting always unlocked at start
            if (system == SystemType.Mining || system == SystemType.Woodcutting)
                isUnlocked = true;

            tab.interactable = isUnlocked;
        }

        private void OnGenerateTriggered()
        {
            Debug.Log("[BottomHUDPresenter] Generate button pressed!");
            
            if (GameManager.Instance == null)
            {
                Debug.LogError("[BottomHUDPresenter] GameManager.Instance is NULL! Make sure GameManager exists in the scene.");
                return;
            }
            
            GameManager.Instance.TryGenerateVillager();
        }

        private void UpdateGenerateButtonVisual()
        {
            if (generateButtonImage == null || GameManager.Instance == null) return;

            var eitr = GameManager.Instance.State.Eitr;
            generateButtonImage.color = eitr.IsOverheated ? overheatColor : normalColor;
        }

        private void HandleVillagerGenerated(SystemType system)
        {
            // Play spawn animation (only if assigned)
            if (villagerSpawnAnimator)
                villagerSpawnAnimator.SetTrigger("Spawn");
        }

        private void HandleOverheated()
        {
            // Could add shake or flash effect
        }

        private void HandleRecovered()
        {
            // Could add ready pulse effect
        }
    }
}
