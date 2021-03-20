using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace absurdjoy {
    /// <summary>
    /// Basic management script to wrangle all the CompanionCameras we might have in the scene.
    /// </summary>
    public class CompanionCameraManager : MonoBehaviour {
        public bool isEnabled { get; protected set; }

        [Tooltip("If true, will disable desktop view of the VR headset if one the companion cameras is on.")]
        public bool autoSetXRView = true;

        [Tooltip("Which transform should compatible cameras be assigned? (Optional)")]
        [SerializeField]
        protected Transform targetTransform;

        protected List<CompanionCamera> companions; // List of all detected companions in children
        protected int currentIndex = 0; // Currently selected companion (might not be active)

        /// <summary>
        /// If enabled, returns the companion currently selected.
        /// </summary>
        public CompanionCamera CurrentCompanion {
            get {
                if (!isEnabled || companions == null || currentIndex >= companions.Count) {
                    return null;
                }
                return companions[(int)currentIndex];
            }
        }

        /// <summary>
        /// If inactive, reactivates the last companion (or - enables the first registered if it hasn't been used yet).
        /// If active, disables every companion.
        /// </summary>
        public void ToggleEnabled() {
            if (isEnabled) {
                DisableAll();
            }
            else {
                EnableLast();
            }
        }

        /// <summary>
        /// Assigns a transform to each of the companions. Depending on the companion, may have no effect or may be required.
        /// </summary>
        public void AssignTargetTransform(Transform newTransform) {
            targetTransform = newTransform;
            foreach (var companion in companions) {
                companion.SetTargetTransform(targetTransform);
            }
        }

        /// <summary>
        /// Register a companion camera to the list of available items.
        /// </summary>
        /// <param name="companion">The camera to add.</param>
        /// <returns>returns true if the camera was added to the list; false if it already existed on the list.</returns>
        public bool AddCompanionIfNecessary(CompanionCamera companion) {
            if (companions == null) {
                companions = new List<CompanionCamera>();
            }

            if (companions.Contains(companion)) {
                return false;
            }
            else {
                companions.Add(companion);
                companion.SetTargetTransform(targetTransform);
                companion.SetCamerasEnabled(false);
                return true;
            }
        }

        /// <summary>
        /// Removes a companion camera from registration, and disables it if it was active.
        /// </summary>
        /// <param name="companion">The companion to remove.</param>
        /// <returns>Returns true if the item was removed; false if it wasn't on the list.</returns>
        public bool RemoveCompanionIfNecessary(CompanionCamera companion) {
            companion.SetCamerasEnabled(false);
            if (companions == null) {
                return false;
            }

            if (companions.Contains(companion)) {
                companions.Remove(companion);
                if (isEnabled && CurrentCompanion == companion) {
                    DisableAll();
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Turn all companion cameras off. If autoSetXRView is enabled, will turn back on the Device View as well.
        /// </summary>
        public void DisableAll() {
            isEnabled = false;
            foreach (var cam in companions) {
                cam.SetCamerasEnabled(false);
            }

            if (autoSetXRView) {
                // Turns on rendering of the VR device to the desktop.
                XRSettings.showDeviceView = true;
            }
        }

        /// <summary>
        /// Restore the last companion camera. Defaults to the first camera if none have been used before.
        /// </summary>
        public void EnableLast() {
            // In case we haven't initialized yet:
            if (currentIndex <= -1) {
                currentIndex = 0;
            }

            // In case we have removed cameras:
            if (currentIndex >= companions.Count) {
                currentIndex = companions.Count - 1;
            }

            SwitchToCamera(currentIndex);
        }

        /// <summary>
        /// Switches to a specific companion camera. Must be initialized/registered with us first.
        /// </summary>
        public void SwitchToCamera(CompanionCamera target) {
            for (int i = 0; i < companions.Count; i++) {
                if (companions[i] == target) {
                    SwitchToCamera(i);
                    return;
                }
            }

            Debug.LogError("Couldn't find companion camera in our index. Did it initialize properly?", target.gameObject);
        }

        /// <summary>
        /// Switches to a companion camera via index number.
        /// </summary>
        public void SwitchToCamera(int newIndex) {
            if (newIndex >= companions.Count || newIndex <= -1) {
                Debug.LogWarning("Tried to activate a companion camera that didn't exist at index: " + newIndex, gameObject);
                return;
            }

            if (currentIndex != newIndex) {
                companions[currentIndex].SetCamerasEnabled(false);
            }

            companions[newIndex].SetCamerasEnabled(true);
            currentIndex = newIndex;

            Debug.Log("Switched companion camera to: " + companions[newIndex].gameObject.name);

            if (autoSetXRView) {
                // Turns off the rendering of the VR device to the desktop.
                XRSettings.showDeviceView = false;
            }

            isEnabled = true;
        }

        /// <summary>
        /// Cycles to the next camera in the index
        /// </summary>
        public void SwitchToNextCamera() {
            SwitchToCameraByIndexDelta(1);
        }

        /// <summary>
        /// Cycles to the previous camera in the index
        /// </summary>
        public void SwitchToPreviousCamera() {
            SwitchToCameraByIndexDelta(-1);
        }

        /// <summary>
        /// Modifies the current index by a specified amount (looping if necessary) and activates that camera. 
        /// </summary>
        /// <param name="delta">the amount to modulate by. can be negative or positive.</param>
        public void SwitchToCameraByIndexDelta(int delta) {
            if (companions.Count <= 0) {
                Debug.LogWarning("Can't switch by delta if there are no cameras.", gameObject);
                return;
            }

            var nextIndex = currentIndex + delta;

            // Handle looping of the index:
            while (nextIndex >= companions.Count) {
                nextIndex -= companions.Count;
            }

            while (nextIndex < 0) {
                nextIndex += companions.Count;
            }

            SwitchToCamera(nextIndex);
        }
    }
}