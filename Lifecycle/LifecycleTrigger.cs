using UnityEngine.Events;

namespace absurdjoy {
    /// <summary>
    /// Very simple class that just invokes a UnityEvent at a certain time in the lifecycle.
    /// </summary>
    public class LifecycleTrigger : LifecycleBase {
        public UnityEvent lifecycleEvent;

        public override void Trigger() {
            lifecycleEvent.Invoke();
        }
    }
}