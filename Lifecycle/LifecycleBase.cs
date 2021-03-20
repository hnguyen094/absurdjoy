// https://www.notion.so/Lifecycle-Scripts-23301c044eff4662baa888080a0522a8
using UnityEngine;

namespace absurdjoy {
    /// <summary>
    /// Extend this class to create scripts that Trigger() at a specific time in the Unity lifecycle.
    /// </summary>
    public abstract class LifecycleBase : MonoBehaviour {
        public enum LifeCycle {
            Never,
            Awake,
            OnEnable,
            Start,
            OnApplicationPause,
            OnApplicationResume,
            OnApplicationQuit,
            OnDisable,
            OnDestroy,
            OnBecameVisible,
            OnBecameInvisible,
        }

        public LifeCycle lifecycleTrigger = LifeCycle.Start;

        protected virtual void Awake() {
            if (lifecycleTrigger == LifeCycle.Awake) {
                Trigger();
            }
        }

        protected virtual void OnEnable() {
            if (lifecycleTrigger == LifeCycle.OnEnable) {
                Trigger();
            }
        }

        protected virtual void Start() {
            if (lifecycleTrigger == LifeCycle.Start) {
                Trigger();
            }
        }

        protected void OnBecameVisible() {
            if (lifecycleTrigger == LifeCycle.OnBecameVisible) {
                Trigger();
            }
        }

        protected void OnBecameInvisible() {
            if (lifecycleTrigger == LifeCycle.OnBecameInvisible) {
                Trigger();
            }
        }

        protected virtual void OnApplicationPause(bool pauseStatus) {
            if (lifecycleTrigger == LifeCycle.OnApplicationPause && pauseStatus == true) {
                Trigger();
            }
            else if (lifecycleTrigger == LifeCycle.OnApplicationResume && pauseStatus == false) {
                Trigger();
            }
        }

        protected virtual void OnApplicationQuit() {
            if (lifecycleTrigger == LifeCycle.OnApplicationQuit) {
                Trigger();
            }
        }

        protected virtual void OnDisable() {
            if (lifecycleTrigger == LifeCycle.OnDisable) {
                Trigger();
            }
        }

        protected virtual void OnDestroy() {
            if (lifecycleTrigger == LifeCycle.OnDestroy) {
                Trigger();
            }
        }

        public abstract void Trigger();
    }
}