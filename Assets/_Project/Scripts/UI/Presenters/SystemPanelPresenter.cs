using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AsgardFoundry.Core;
using AsgardFoundry.Data;
using AsgardFoundry.UI.Components;

namespace AsgardFoundry.UI.Presenters
{
    /// <summary>
    /// Presents a production system panel with villager count, production rate,
    /// tap zone, and upgrade options.
    /// </summary>
    public class SystemPanelPresenter : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Button closeButton;

        [Header("Info Display")]
        [SerializeField] private TMP_Text systemNameText;
        [SerializeField] private TMP_Text villagerCountText;
        [SerializeField] private TMP_Text productionRateText;
        [SerializeField] private Image cycleProgressBar;
        [SerializeField] private TMP_Text tierText;

        [Header("Tap Zone")]
        [SerializeField] private Button tapButton;
        [SerializeField] private TMP_Text tapBonusText;
        [SerializeField] private float tapValue = 2f;

        [Header("Animation")]
        [SerializeField] private Animator panelAnimator;

        private SystemType currentSystem;
        private bool isOpen;

        private void Start()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(Close);

            if (tapButton != null)
                tapButton.onClick.AddListener(OnTap);

            panelRoot?.SetActive(false);
        }

        /// <summary>
        /// Open the panel for a specific system.
        /// </summary>
        public void Open(SystemType system)
        {
            currentSystem = system;
            isOpen = true;
            panelRoot?.SetActive(true);

            // Update static info
            if (systemNameText != null)
                systemNameText.text = GetSystemDisplayName(system);

            if (panelAnimator != null)
                panelAnimator.SetTrigger("Open");
        }

        /// <summary>
        /// Close the panel.
        /// </summary>
        public void Close()
        {
            isOpen = false;

            if (panelAnimator != null)
            {
                panelAnimator.SetTrigger("Close");
                // Panel will be disabled by animation event
            }
            else
            {
                panelRoot?.SetActive(false);
            }
        }

        private void Update()
        {
            if (!isOpen || GameManager.Instance == null) return;

            var state = GameManager.Instance.State;
            if (!state.Systems.TryGetValue(currentSystem, out var system)) return;

            // Villager count
            if (villagerCountText != null)
                villagerCountText.text = $"{system.VillagerCount}/{system.MaxVillagers}";

            // Production rate
            if (productionRateText != null)
            {
                float perSec = system.ProductionPerSecond;
                string resourceName = GameManager.GetResourceForSystem(currentSystem).ToString();
                productionRateText.text = $"{perSec:F1} {resourceName}/sec";
            }

            // Cycle progress
            if (cycleProgressBar != null)
                cycleProgressBar.fillAmount = system.AccumulatedProgress / system.CycleTime;

            // Tier
            if (tierText != null)
                tierText.text = $"Tier {system.Tier}";

            // Tap bonus indicator
            if (tapBonusText != null)
            {
                if (system.TapBonus > 0)
                    tapBonusText.text = $"+{system.TapBonus:F0}";
                else
                    tapBonusText.text = "";
            }
        }

        private void OnTap()
        {
            GameManager.Instance?.RegisterTap(currentSystem, tapValue);

            // Could add tap feedback animation here
        }

        /// <summary>
        /// Called by animation event when close animation finishes.
        /// </summary>
        public void OnCloseAnimationComplete()
        {
            panelRoot?.SetActive(false);
        }

        private string GetSystemDisplayName(SystemType type)
        {
            return type switch
            {
                SystemType.Mining => "Mine",
                SystemType.Woodcutting => "Lumber Mill",
                SystemType.Farming => "Farm",
                SystemType.Smithing => "Smithy",
                SystemType.Market => "Market",
                _ => type.ToString()
            };
        }
    }
}
