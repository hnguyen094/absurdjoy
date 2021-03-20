using UnityEngine;
using UnityEngine.Events;

namespace absurdjoy {
    public class LifecycleTimer : LifecycleBase {
        public UnityEvent OnTimer;
        public float time;
        public bool repeat;
        public bool useUnscaledTime;

        private float timeRemaining;

        [HideInInspector] // You can manually pause the timer via code references.
        public bool timerActive = false;

        /// <summary>
        /// Trigger starts the countdown clock.
        /// </summary>
        public override void Trigger() {
            timerActive = true;
            timeRemaining = time;
        }

        private void Update() {
            if (timerActive) {
                if (useUnscaledTime) {
                    timeRemaining -= Time.unscaledDeltaTime;
                }
                else {
                    timeRemaining -= Time.deltaTime;
                }

                if (timeRemaining <= 0) {
                    OnTimer.Invoke();
                    if (repeat) {
                        timeRemaining += time;
                    }
                    else {
                        timerActive = false;
                    }
                }
            }
        }
    }
}