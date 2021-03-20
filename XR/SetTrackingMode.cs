// https://gist.github.com/SnugglePilot/278f0a779c4bffea34c258c7beb9c08c
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace absurdjoy.axe {
    public class SetTrackingMode : MonoBehaviour {
        public TrackingOriginModeFlags trackingFlags = TrackingOriginModeFlags.Floor;
        public bool recenterOnSet = true;

        private void OnEnable() {
            SetTrackingModeTo(); // Change tracking mode on whatever is already initialized.
            SubsystemManager.reloadSubsytemsCompleted += SetTrackingModeTo; // Add listener for future inits.
        }

        private void OnDisable() {
            SubsystemManager.reloadSubsytemsCompleted -= SetTrackingModeTo;
        }

        /// <summary>
        /// Sets the tracking mode of all currently initialized XR Subsystems to the origin configured in the inspector.
        /// </summary>
        public void SetTrackingModeTo() {
            SetTrackingModeTo(trackingFlags, recenterOnSet);
        }

        /// <summary>
        /// Sets the tracking mode of all currently initialized XR Subsystems to the origin specified. 
        /// </summary>
        public void SetTrackingModeTo(TrackingOriginModeFlags flags, bool recenter) {
            List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
            SubsystemManager.GetInstances(subsystems);

            foreach (var subsystem in subsystems) {
                if (subsystem.TrySetTrackingOriginMode(flags)) {
                    if (recenter) {
                        // Now that we've changed the origin mode we'll have to recalibrate to this new point
                        // in space (it will re-zero off of the last-set-height, which for Oculus Quest is the
                        // HMD boot position).
                        subsystem.TryRecenter();
                    }
                }
                else {
                    Debug.LogError("Failed to set the TrackingOriginMode of id:" + subsystem.SubsystemDescriptor.id, gameObject);
                }
            }
        }
    }
}