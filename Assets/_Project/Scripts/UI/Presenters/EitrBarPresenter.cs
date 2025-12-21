using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AsgardFoundry.Core;

namespace AsgardFoundry.UI.Presenters
{
    /// <summary>
    /// Presents the EITR (energy) bar showing current/max and overheat state.
    /// Updates every frame for smooth animation.
    /// </summary>
    public class EitrBarPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image fillImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TMP_Text eitrText;
        [SerializeField] private GameObject overheatOverlay;
        [SerializeField] private TMP_Text overheatCountdownText;

        [Header("Colors")]
        [SerializeField] private Color normalColor = new Color(0.2f, 0.7f, 1f);
        [SerializeField] private Color lowColor = new Color(1f, 0.5f, 0.2f);
        [SerializeField] private Color overheatColor = new Color(1f, 0.2f, 0.2f);
        [SerializeField] private float lowThreshold = 0.25f;

        [Header("Animation")]
        [SerializeField] private float fillLerpSpeed = 8f;

        private float targetFill;
        private float currentFill;

        private void Start()
        {
            // Subscribe to events for immediate feedback
            EventBus.OnEitrOverheated += HandleOverheat;
            EventBus.OnEitrRecovered += HandleRecovered;
        }

        private void OnDestroy()
        {
            EventBus.OnEitrOverheated -= HandleOverheat;
            EventBus.OnEitrRecovered -= HandleRecovered;
        }

        private void Update()
        {
            if (GameManager.Instance == null) return;

            var eitr = GameManager.Instance.State.Eitr;
            
            // Calculate target fill
            targetFill = eitr.CurrentEitr / eitr.MaxEitr;

            // Smooth animation
            currentFill = Mathf.Lerp(currentFill, targetFill, fillLerpSpeed * Time.deltaTime);
            
            if (fillImage != null)
            {
                fillImage.fillAmount = currentFill;

                // Color based on state
                if (eitr.IsOverheated)
                {
                    fillImage.color = overheatColor;
                }
                else if (currentFill < lowThreshold)
                {
                    fillImage.color = Color.Lerp(lowColor, normalColor, currentFill / lowThreshold);
                }
                else
                {
                    fillImage.color = normalColor;
                }
            }

            // Update text
            if (eitrText != null)
            {
                eitrText.text = $"{Mathf.RoundToInt(eitr.CurrentEitr)}/{Mathf.RoundToInt(eitr.MaxEitr)}";
            }

            // Overheat overlay
            if (overheatOverlay != null)
            {
                overheatOverlay.SetActive(eitr.IsOverheated);
            }

            // Countdown text
            if (overheatCountdownText != null && eitr.IsOverheated)
            {
                overheatCountdownText.text = $"{eitr.OverheatCooldownRemaining:F1}s";
            }
        }

        private void HandleOverheat()
        {
            // Could add shake animation or sound here
            Debug.Log("[EitrBar] Overheated!");
        }

        private void HandleRecovered()
        {
            Debug.Log("[EitrBar] Recovered!");
        }
    }
}
