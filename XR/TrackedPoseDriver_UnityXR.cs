using UnityEngine;
using UnityEngine.XR;

namespace absurdjoy {
    /// <summary>
    /// This grabs tracking data for the indicated XRNode from the UnityXR Input system.
    /// </summary>
    public class TrackedPoseDriver_UnityXR : TrackedPoseDriver_Abstract {
        protected UnityXRDevice uDevice;

        public override void Initialize() {
            ResetDevice();
            uDevice = new UnityXRDevice(xrNode, deviceIndex, OnDeviceValid, OnDeviceInvalid, debugLog);
        }

        protected void ResetDevice() {
            if (uDevice != null) {
                uDevice.Destroy();
                uDevice = null;
            }
        }

        protected override void OnDestroy() {
            ResetDevice();
            base.OnDestroy();
        }

        // Housekeeping Items:

        public override bool IsDeviceValid() {
            return uDevice.device.isValid;
        }

        // Tracking Items:

        public override bool IsTrackingValid() {
            return IsDeviceValid() && uDevice.GetInputTrackingState() != InputTrackingState.None;
        }

        public override Vector3 GetPosition() {
            return uDevice.GetVec3(CommonUsages.devicePosition);
        }

        public override Quaternion GetRotation() {
            return uDevice.GetQuaternion(CommonUsages.deviceRotation);
        }

        public override Vector3 GetVelocity() {
            return uDevice.GetVec3(CommonUsages.deviceVelocity);
        }

        public override Vector3 GetAngularVelocity() {
            return uDevice.GetVec3(CommonUsages.deviceAngularVelocity);
        }

        public override Vector3 GetAcceleration() {
            return uDevice.GetVec3(CommonUsages.deviceAcceleration);
        }

        public override Vector3 GetAngularAcceleration() {
            return uDevice.GetVec3(CommonUsages.deviceAngularAcceleration);
        }
    }
}