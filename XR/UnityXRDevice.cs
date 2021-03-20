using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace absurdjoy {
    public class UnityXRDevice {
        protected XRNode xrNode;
        protected int deviceIndex;
        public InputDevice device { get; private set; }

        protected UnityAction OnDeviceValid;
        protected UnityAction OnDeviceInvalid;

        private bool isDeviceAssigned = false;
        private bool debugLog = false;

        public UnityXRDevice(XRNode targetXRNode, int deviceIndex, UnityAction OnDeviceValid, UnityAction OnDeviceInvalid, bool debugLog = false) {
            xrNode = targetXRNode;
            this.deviceIndex = deviceIndex;
            this.OnDeviceValid = OnDeviceValid;
            this.OnDeviceInvalid = OnDeviceInvalid;
            this.debugLog = debugLog;
            Initialize();
        }

        /// <summary>
        /// Once the XR system is up and running, check for a matching device.
        /// If this is called before XR is initialized, or before the device is turned on, the connection events will
        /// handle re-initialization later.
        /// </summary>
        public void Initialize() {
            RemoveListeners();

            var potentialDevices = new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(xrNode, potentialDevices);

            if (potentialDevices != null && potentialDevices.Count != 0) {
                if (deviceIndex >= potentialDevices.Count) {
                    // Debug.LogError(xrNode+" index " + deviceIndex + " exceeds device count: " + potentialDevices.Count);
                    // This can happen if a device is disconnecting. Should be auto-destroyed this frame or next.
                    AssignDevice(new InputDevice());
                }
                else {
                    if (debugLog) {
                        Debug.Log(xrNode + " index " + deviceIndex + " activated successfully.");
                    }
                    AssignDevice(potentialDevices[deviceIndex]);
                }
            }
            else {
                if (debugLog) {
                    Debug.LogWarning("No " + xrNode + " devices found. Waiting for new device connection. (May require HMD activation)");
                }

                // InputDevice can't be null, but it will default to .IsValid = false;
                AssignDevice(new InputDevice());
            }

            AddListeners();
        }

        protected void AssignDevice(InputDevice newDevice) {
            if (isDeviceAssigned && newDevice == device) {

                if (debugLog) {
                    Debug.Log("Device was already assigned.");
                }
                return;
            }

            device = newDevice;

            if (device.isValid) {
                isDeviceAssigned = true;
                if (debugLog) {
                    Debug.Log(string.Format("Valid device name '{0}' with role '{1}' assigned", device.name, device.characteristics.ToString()));
                }

                if (OnDeviceValid != null) {
                    OnDeviceValid.Invoke();
                }
            }
            else {
                isDeviceAssigned = false;
                if (debugLog) {
                    Debug.Log(string.Format("Invalid device name '{0}' with role '{1}' assigned", device.name, device.characteristics.ToString()));
                }

                if (OnDeviceInvalid != null) {
                    OnDeviceInvalid.Invoke();
                }
            }
        }

        public void RemoveListeners() {
            InputDevices.deviceConnected -= OnDeviceConnected;
            InputDevices.deviceDisconnected -= OnDeviceDisconnected;
            InputDevices.deviceConfigChanged -= OnDeviceConfigChanged;
        }

        protected void AddListeners() {
            InputDevices.deviceConnected += OnDeviceConnected;
            InputDevices.deviceDisconnected += OnDeviceDisconnected;
            InputDevices.deviceConfigChanged += OnDeviceConfigChanged;
        }

        protected void OnDeviceConnected(InputDevice target) {
            if (device == target || !device.isValid) {
                // so we don't spam everything waiting on a connection, let's see if this might be a new target for ourselves:
                var potentialDevices = new List<InputDevice>();
                InputDevices.GetDevicesAtXRNode(xrNode, potentialDevices);
                if (potentialDevices != null && potentialDevices.Count != 0) {
                    if (potentialDevices.Contains(target)) {
                        Initialize();
                    }
                }
            }
        }

        protected void OnDeviceDisconnected(InputDevice target) {
            if (device == target) {
                Initialize();
            }
        }

        protected void OnDeviceConfigChanged(InputDevice target) {
            if (device == target) {
                Initialize();
            }
        }

        public void Destroy() {
            RemoveListeners();
            OnDeviceInvalid = null;
            OnDeviceInvalid = null;
        }

        protected Vector2 v2;
        public Vector2 GetVec2(InputFeatureUsage<Vector2> feature) {
            if (device.TryGetFeatureValue(feature, out v2)) {
                return v2;
            }

            return Vector2.zero;
        }

        protected Vector3 v3;
        public Vector3 GetVec3(InputFeatureUsage<Vector3> feature) {
            if (device.TryGetFeatureValue(feature, out v3)) {
                return v3;
            }

            return Vector3.zero;
        }

        protected Quaternion q;
        public Quaternion GetQuaternion(InputFeatureUsage<Quaternion> feature) {
            if (device.TryGetFeatureValue(feature, out q)) {
                return q;
            }

            return Quaternion.identity;
        }

        protected bool b;
        public bool GetBool(InputFeatureUsage<bool> feature) {
            if (device.TryGetFeatureValue(feature, out b)) {
                return b;
            }

            return false;
        }

        protected InputTrackingState its;
        public InputTrackingState GetInputTrackingState() {
            if (device.TryGetFeatureValue(CommonUsages.trackingState, out its)) {
                return its;
            }

            return InputTrackingState.None;
        }
    }
}