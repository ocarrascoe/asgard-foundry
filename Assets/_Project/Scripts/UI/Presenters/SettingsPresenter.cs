using UnityEngine;
using UnityEngine.UI;
using AsgardFoundry.Core;

namespace AsgardFoundry.UI.Presenters
{
    public class SettingsPresenter : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Button openSettingsButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button resetGameButton;

        private void Start()
        {
            // Initial state
            if (settingsPanel != null)
                settingsPanel.SetActive(false);

            // Setup listeners
            if (openSettingsButton != null)
                openSettingsButton.onClick.AddListener(OpenSettings);

            if (closeButton != null)
                closeButton.onClick.AddListener(CloseSettings);

            if (resetGameButton != null)
                resetGameButton.onClick.AddListener(OnResetClicked);
        }

        private void OpenSettings()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
            
            // Optional: Pause game?
            // Time.timeScale = 0; 
        }

        private void CloseSettings()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
            
            // Optional: Unpause
            // Time.timeScale = 1;
        }

        private void OnResetClicked()
        {
            // Reset game and maybe reload scene or just re-init
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetGame();
                CloseSettings();
                
                // Reload scene to ensure fresh UI state
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            }
        }
    }
}
