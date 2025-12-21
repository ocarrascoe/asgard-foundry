using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace AsgardFoundry.UI.Components
{
    /// <summary>
    /// Button that triggers repeatedly while held down.
    /// Used for villager generation (like Egg Inc.'s hatch button).
    /// </summary>
    public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Header("Timing")]
        [Tooltip("Delay before auto-repeat starts")]
        [SerializeField] private float initialDelay = 0.3f;

        [Tooltip("Time between auto-repeat triggers")]
        [SerializeField] private float repeatInterval = 0.1f;

        [Tooltip("Minimum interval (speed increases over time)")]
        [SerializeField] private float minInterval = 0.02f;

        [Tooltip("How quickly interval decreases")]
        [SerializeField] private float accelerationRate = 0.95f;

        [Header("Events")]
        public UnityEvent OnPress = new UnityEvent();
        public UnityEvent OnRelease = new UnityEvent();
        public UnityEvent OnTrigger = new UnityEvent();

        private bool isHeld;
        private float holdTimer;
        private float currentInterval;
        private bool initialTriggerDone;

        private void Update()
        {
            if (!isHeld) return;

            holdTimer += Time.deltaTime;

            if (!initialTriggerDone)
            {
                if (holdTimer >= initialDelay)
                {
                    initialTriggerDone = true;
                    holdTimer = 0f;
                    Trigger();
                }
            }
            else
            {
                if (holdTimer >= currentInterval)
                {
                    holdTimer = 0f;
                    Trigger();

                    // Accelerate repeat rate
                    currentInterval = Mathf.Max(minInterval, currentInterval * accelerationRate);
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isHeld = true;
            holdTimer = 0f;
            currentInterval = repeatInterval;
            initialTriggerDone = false;

            OnPress?.Invoke();
            Trigger(); // Immediate first trigger
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Release();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Release();
        }

        private void Release()
        {
            if (isHeld)
            {
                isHeld = false;
                OnRelease?.Invoke();
            }
        }

        private void Trigger()
        {
            OnTrigger?.Invoke();
        }

        /// <summary>
        /// Force stop the hold state (e.g., when UI is disabled).
        /// </summary>
        public void ForceRelease()
        {
            Release();
        }

        private void OnDisable()
        {
            ForceRelease();
        }
    }
}
