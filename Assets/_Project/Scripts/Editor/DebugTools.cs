using UnityEngine;
using UnityEditor;
using AsgardFoundry.Core;
using AsgardFoundry.Persistence;

namespace AsgardFoundry.Editor
{
    public static class DebugTools
    {
        [MenuItem("Asgard Foundry/Debug/Reset Game (Delete Save)")]
        public static void ResetGame()
        {
            if (EditorUtility.DisplayDialog(
                "Reset Game",
                "This will delete your save file and reset all progress. Are you sure?",
                "Yes, Reset",
                "Cancel"))
            {
                SaveManager.DeleteSave();
                
                // If in play mode, also reset runtime state
                if (Application.isPlaying && GameManager.Instance != null)
                {
                    GameManager.Instance.ResetGame();
                }
                
                EditorUtility.DisplayDialog("Game Reset", "Save file deleted. Restart Play mode for a fresh game.", "OK");
            }
        }

        [MenuItem("Asgard Foundry/Debug/Show Save File Location")]
        public static void ShowSaveLocation()
        {
            string path = SaveManager.SaveFilePath;
            Debug.Log($"[Debug] Save file location: {path}");
            EditorUtility.DisplayDialog("Save Location", path, "OK");
        }

        [MenuItem("Asgard Foundry/Debug/Log Current State")]
        public static void LogCurrentState()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[Debug] Must be in Play mode to log state");
                return;
            }

            if (GameManager.Instance == null)
            {
                Debug.LogError("[Debug] GameManager not found");
                return;
            }

            var state = GameManager.Instance.State;
            Debug.Log("=== CURRENT GAME STATE ===");
            Debug.Log($"EITR: {state.Eitr.CurrentEitr}/{state.Eitr.MaxEitr}");
            Debug.Log($"Selected System: {state.SelectedAssignment}");
            
            foreach (var kvp in state.Systems)
            {
                var sys = kvp.Value;
                Debug.Log($"{sys.Type}: {sys.VillagerCount}/{sys.MaxVillagers} villagers, Cycle: {sys.AccumulatedProgress:F1}/{sys.CycleTime}s");
            }
            
            foreach (var kvp in state.Resources)
            {
                Debug.Log($"{kvp.Key}: {kvp.Value:F0}");
            }
        }
    }
}
