using UnityEngine;

namespace AsgardFoundry.Core
{
    /// <summary>
    /// Tracks game time including delta time and handles pause state.
    /// Provides accurate timing for production calculations.
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        /// <summary>Current game delta time (respects pause).</summary>
        public float DeltaTime => IsPaused ? 0f : Time.deltaTime;

        /// <summary>Unscaled delta time (ignores pause).</summary>
        public float UnscaledDeltaTime => Time.unscaledDeltaTime;

        /// <summary>Whether the game is currently paused.</summary>
        public bool IsPaused { get; private set; }

        /// <summary>Total game time since session started.</summary>
        public float SessionTime { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            if (!IsPaused)
            {
                SessionTime += Time.deltaTime;
            }
        }

        /// <summary>
        /// Pause the game (e.g., when opening menus).
        /// </summary>
        public void Pause()
        {
            IsPaused = true;
            Time.timeScale = 0f;
        }

        /// <summary>
        /// Resume the game.
        /// </summary>
        public void Resume()
        {
            IsPaused = false;
            Time.timeScale = 1f;
        }

        /// <summary>
        /// Toggle pause state.
        /// </summary>
        public void TogglePause()
        {
            if (IsPaused) Resume();
            else Pause();
        }

        /// <summary>
        /// Get current Unix timestamp.
        /// </summary>
        public static long GetUnixTimestamp()
        {
            return System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Calculate seconds elapsed since a given Unix timestamp.
        /// </summary>
        public static float GetSecondsSince(long timestamp)
        {
            long now = GetUnixTimestamp();
            return now - timestamp;
        }
    }
}
