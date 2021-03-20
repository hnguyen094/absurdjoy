using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace absurdjoy {
    /// <summary>
    /// A base class for pulling data from XRNode
    /// </summary>
    public abstract class XRNode_Abstract : LifecycleBase {
        [Tooltip("If true, this script will spew debug updates.")]
        public bool debugLog;

        [Header("XR Node")]
        [Tooltip("Use LeftHand or RightHand for those roles, GameController for a non-position-tracked input device, or HardwareTracker for additional inputs.")]
        public XRNode xrNode = XRNode.LeftHand;

        [Tooltip("If there is more than one device of the given type, which one should we use?")]
        public int deviceIndex = 0;

        // When the device connects this function will fire.
        public UnityAction OnDeviceValid;
        // When the device becomes invalid (eg: disconnects) this function will fire.
        public UnityAction OnDeviceInvalid;

        protected void Log(string message) {
            if (debugLog) {
                Debug.Log(message, gameObject);
            }
        }

        /// <summary>
        /// Use this function to begin Initialization. You can call this manually or use the lifecycle dropdown in the inspector.
        /// </summary>
        public override void Trigger() {
            Initialize();
        }

        public abstract bool IsDeviceValid();
        public abstract void Initialize();
    }
}