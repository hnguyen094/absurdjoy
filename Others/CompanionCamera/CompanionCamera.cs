// https://www.notion.so/Companion-Display-Management-685f7b246a12487191e4d0f14b7f8619
using UnityEngine;

namespace absurdjoy {
    /// <summary>
    /// A "companion" camera is one that displays to a computer monitor while an accompanying XR player is in an HMD nearby.
    /// This script can be extended for added functionality, but acts as a decent placeholder for a static camera in a scene.
    /// </summary>
    public class CompanionCamera : MonoBehaviour {
        public bool isEnabled { get; protected set; }

        [Tooltip("If unassigned, will search for one in the scene")]
        public CompanionCameraManager companionCameraManager;

        // To support layered cameras we allow there to be multiple, but the common use case is 
        // for there to be just one child camera.
        [Tooltip("If unassigned, will search children for valid cameras.")]
        [SerializeField]
        protected Camera[] cameras;

        // Used by some subsystems.
        protected Transform targetTransform;

        /// <summary>
        /// Find our cameras and register with the manager
        /// </summary>
        protected virtual void OnEnable() {
            if (cameras == null || cameras.Length == 0) {
                // Intentionally excluding disabled children from this list so we can turn off bad actors
                cameras = GetComponentsInChildren<Camera>(false);
            }

            if (cameras == null || cameras.Length == 0) {
                Debug.LogError("CompanionCamera: required camera not found.", gameObject);
            }

            if (companionCameraManager == null) {
                companionCameraManager = GameObject.FindObjectOfType<CompanionCameraManager>();
            }

            if (companionCameraManager == null) {
                Debug.LogError("CompanionCamera: required CompanionCameraManager not found.", gameObject);
            }
            companionCameraManager.AddCompanionIfNecessary(this);
        }

        /// <summary>
        /// Deregister with the manager
        /// </summary>
        protected virtual void OnDisable() {
            if (companionCameraManager != null) {
                companionCameraManager.RemoveCompanionIfNecessary(this);
            }
        }

        /// <summary>
        /// Sets whether this companion should be active or not.
        /// </summary>
        public void SetCamerasEnabled(bool to) {
            foreach (var cam in cameras) {
                cam.enabled = to;
            }

            isEnabled = to;
        }

        /// <summary>
        /// Toggle the companion visibility.
        /// </summary>
        public bool ToggleCamerasEnabled() {
            SetCamerasEnabled(!isEnabled);
            return isEnabled;
        }

        /// <summary>
        /// Sets the targetTransform for companion behaviours.
        /// </summary>
        public virtual void SetTargetTransform(Transform to) {
            targetTransform = to;

            var iats = GetComponentsInChildren<IAssignableTransform>();
            foreach (var iat in iats) {
                iat.AssignTransform(to);
            }
        }

        /// <summary>
        /// Activates this camera via the manager.
        /// </summary>
        public void ActivateViaManager() {
            companionCameraManager.SwitchToCamera(this);
        }
    }
}