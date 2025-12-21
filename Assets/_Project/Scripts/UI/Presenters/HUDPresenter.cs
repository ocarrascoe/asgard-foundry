using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AsgardFoundry.Core;
using AsgardFoundry.Data;

namespace AsgardFoundry.UI.Presenters
{
    /// <summary>
    /// Presents the top HUD showing resources, EITR bar, and major stats.
    /// </summary>
    public class HUDPresenter : MonoBehaviour
    {
        [Header("Resource Display")]
        [SerializeField] private TMP_Text stoneText;
        [SerializeField] private TMP_Text woodText;
        [SerializeField] private TMP_Text foodText;
        [SerializeField] private TMP_Text metalText;
        [SerializeField] private TMP_Text goldText;

        [Header("Stats")]
        [SerializeField] private TMP_Text totalVillagersText;
        [SerializeField] private TMP_Text eraText;

        [Header("Number Formatting")]
        [SerializeField] private bool useShortFormat = true;

        private void Start()
        {
            EventBus.OnResourceChanged += HandleResourceChanged;
            EventBus.OnVillagerGenerated += HandleVillagerGenerated;
        }

        private void OnDestroy()
        {
            EventBus.OnResourceChanged -= HandleResourceChanged;
            EventBus.OnVillagerGenerated -= HandleVillagerGenerated;
        }

        private void Update()
        {
            if (GameManager.Instance == null) return;

            RefreshAllResources();
            RefreshStats();
        }

        private void RefreshAllResources()
        {
            var state = GameManager.Instance.State;

            UpdateResourceText(stoneText, "Stone", state.GetResource(ResourceType.Stone));
            UpdateResourceText(woodText, "Wood", state.GetResource(ResourceType.Wood));
            UpdateResourceText(foodText, "Food", state.GetResource(ResourceType.Food));
            UpdateResourceText(metalText, "Metal", state.GetResource(ResourceType.Metal));
            UpdateResourceText(goldText, "Gold", state.GetResource(ResourceType.Gold));
        }

        private void RefreshStats()
        {
            var state = GameManager.Instance.State;

            // Total villagers across all systems
            if (totalVillagersText != null)
            {
                int total = 0;
                foreach (var sys in state.Systems.Values)
                {
                    total += sys.VillagerCount;
                }
                totalVillagersText.text = $"Villagers: {total}";
            }

            // Era
            if (eraText != null)
            {
                eraText.text = state.City.CurrentEra.ToString();
            }
        }

        private void UpdateResourceText(TMP_Text text, string label, double amount)
        {
            if (text == null) return;
            text.text = $"{label}: {FormatNumber(amount)}";
        }

        private void HandleResourceChanged(ResourceType type, double amount)
        {
            // Resources update in Update() anyway, but this could trigger animations
        }

        private void HandleVillagerGenerated(SystemType system)
        {
            // Could trigger a +1 animation near villager count
        }

        /// <summary>
        /// Format large numbers for display (1.5M, 2.3B, etc.)
        /// </summary>
        public string FormatNumber(double value)
        {
            if (!useShortFormat || value < 1000)
            {
                return value.ToString("F0");
            }

            string[] suffixes = { "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc" };
            int suffixIndex = 0;
            double displayValue = value;

            while (displayValue >= 1000 && suffixIndex < suffixes.Length - 1)
            {
                displayValue /= 1000;
                suffixIndex++;
            }

            if (displayValue >= 100)
                return $"{displayValue:F0}{suffixes[suffixIndex]}";
            else if (displayValue >= 10)
                return $"{displayValue:F1}{suffixes[suffixIndex]}";
            else
                return $"{displayValue:F2}{suffixes[suffixIndex]}";
        }
    }
}
