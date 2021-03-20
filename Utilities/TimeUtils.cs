// https://www.notion.so/Time-Utilities-2f283c87dec94b98987b66a01854d618
using System;
using System.Collections;
using UnityEngine;

namespace absurdjoy {
    public static class TimeUtils {
        /// <summary>
        /// Converts the given date into Unix time. Requires .NET4.6
        /// Note this function will fail on Sunday, 4 December 292,277,026,596AD so...
        /// Please replace this code before then.
        /// </summary>
        public static long ToUnixTime(DateTime date) {
            return ((DateTimeOffset)date).ToUnixTimeSeconds();
        }

        /// <summary>
        /// Returns the Unix time right now.
        /// </summary>
        public static long GetUnixTime() {
            return ToUnixTime(DateTime.UtcNow);
        }

        /// <summary>
        /// Converts the current Unix time to a hex encoded string.
        /// </summary>
        public static string GetUnixTimeHex() {
            return GetUnixTime().ToString("x");
        }

        /// <summary>
        /// Perform an action after a specified amount of time (attached to a gameObject)
        /// </summary>
        /// <param name="targetGameObject"></param>
        /// <param name="time"></param>
        /// <param name="callback"></param>
        /// <param name="triggerIfDestroyedEarly"></param>
        public static void AfterTimer(GameObject targetGameObject, float time, System.Action callback, bool triggerIfDestroyedEarly = false) {
            AfterTimer_Component afterTimer_component = targetGameObject.AddComponent<AfterTimer_Component>();
            afterTimer_component.Init(time, callback, triggerIfDestroyedEarly);
        }
    }

    /// <summary>
    /// Component used by the AfterTimer function.
    /// This is based on code offered in the SteamVR example scripts.
    /// </summary>
    [System.Serializable]
    public class AfterTimer_Component : MonoBehaviour {
        private Action callback;
        private float triggerTime;
        private bool timerActive = false;
        private bool triggerOnEarlyDestroy = false;

        public void Init(float time, System.Action callback, bool earlydestroy) {
            triggerTime = time;
            this.callback = callback;
            triggerOnEarlyDestroy = earlydestroy;
            timerActive = true;
            StartCoroutine(Wait());
        }

        private IEnumerator Wait() {
            yield return new WaitForSeconds(triggerTime);
            timerActive = false;
            callback.Invoke();
            Destroy(this);
        }

        void OnDestroy() {
            if (timerActive) {
                //If the component or its GameObject get destroyed before the timer is complete, clean up
                StopCoroutine(Wait());
                timerActive = false;

                if (triggerOnEarlyDestroy) {
                    callback.Invoke();
                }
            }
        }
    }
}