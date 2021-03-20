// https://www.notion.so/Smart-XR-Device-Selection-ef7297ec1dae42ad8a509519d2718275
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace absurdjoy {
    /// <summary>
    /// This script will find a best-match target for XR, but controlled easily via scripting.
    /// eg: You can start in 2D, and once assets are loaded or after user input, trigger the load to XR.
    /// Also supports fallbacks/force-no-XR mode to speed up development where an HMD isn't required.
    /// 
    /// Note that one day this will be deprecated and/or enhanced by the XR XDK Management package:
    /// https://docs.unity3d.com/Packages/com.unity.xr.management@3.0/manual/index.html
    /// But, as of this writing, this package does not support all XR platforms.
    /// </summary>
    public class TurnOnXR : LifecycleBase {
        [Tooltip("For debug purposes, force all initializations to fail.")]
        public bool forceFailure = false;
        [Tooltip("Should this script generate log output?")]
        public bool log = false;

        [Tooltip("Executed when XR is successfully loaded.")]
        public UnityEvent onSuccess;
        [Tooltip("Executed when XR fails to load.")]
        public UnityEvent onFailure;

        private Coroutine coroutine;

        public const string commandLineOverrideString = "xrSystem";

        public override void Trigger() {
            if (coroutine != null) {
                StopCoroutine(coroutine);
                Log("Trigger was called twice! This is probably a bad.");
            }
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            Log("OSX Not Supported.");
            onFailure.Invoke();
#else
            coroutine = StartCoroutine(LoadBestDevice());
#endif
        }

        private IEnumerator LoadBestDevice() {
            if (XRSettings.enabled == true) {
                Log("XR already enabled.");
                yield break;
            }

            Log("Attempting start...");

            List<string> xrDevicesToAttempt = new List<string>(XRSettings.supportedDevices);

            // Let's check the command line environment to see if we are trying to force a first-pick. Note that this
            // will still result in other devices being tried if the suggested one fails.
            // We add any ol' arbitrary string here and don't check it against the list of supported devices. (Unity
            // does a pretty graceful failure if you add something not in that list.)
            if (!string.IsNullOrEmpty(commandLineOverrideString) && CommandLineArguments.HasKey(commandLineOverrideString)) {
                var forceTarget = CommandLineArguments.GetValue(commandLineOverrideString);
                if (!string.IsNullOrEmpty(forceTarget)) {
                    Log("Command Line Argument detected - making first target `" + forceTarget + "`");
                    // Remove any duplicates
                    xrDevicesToAttempt.Remove(forceTarget);
                    // Re-insert this element at the first index
                    xrDevicesToAttempt.Insert(0, forceTarget);
                }
                else {
                    Debug.LogError("Detected XR override argument `" + commandLineOverrideString + "`, but no value was given. Try adding a value, eg: `=OpenVR`.");
                }
            }

            foreach (var device in xrDevicesToAttempt) {
                if (string.IsNullOrEmpty(device) || device == "None") {
                    continue;
                }

                yield return StartCoroutine(LoadDevice(device));
                if (XRSettings.loadedDeviceName == device) {
                    Log("Success!");

                    // Switching XRSettings On takes place on the next frame, so delay once.
                    // Newer versions of Unity might automatically do this with XRSettings.LoadDeviceByName,
                    // So this may now be unnecessary.
                    XRSettings.enabled = true;
                    yield return null;
                    break;
                }
                else {
                    Log("Failed.");
                }
            }

            coroutine = null;

            if (XRSettings.enabled) {
                Log("Complete - Success");
                onSuccess.Invoke();
            }
            else {
                Log("Complete - Failure.");
                onFailure.Invoke();
            }
        }

        private IEnumerator LoadDevice(string device) {
            Log("Attempting to start XR device: `" + device + "`");
            if (forceFailure) {
                // Forcing a failure state while still iterating through the list.
                // We do this here instead of bypassing the whole startup routine, because the Coroutine timing
                // might be important for the rest of the application.
            }
            else {
                XRSettings.LoadDeviceByName(device);
            }

            // Wait one frame. Hardware loading happens at start of next frame.
            yield return null;
        }

        private void Log(string text) {
            if (log) {
                Debug.Log("TurnOnXR: " + text, gameObject);
            }
        }
    }

#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TurnOnXR))]
    public class TurnOnXREditor : Editor {
        private SerializedProperty lifecycleTrigger;
        private SerializedProperty forceFailure;
        private SerializedProperty log;
        private SerializedProperty onSuccess;
        private SerializedProperty onFailure;
        private bool showEvents = false;
        private bool showDebug = false;

        private void OnEnable() {
            lifecycleTrigger = serializedObject.FindProperty("lifecycleTrigger");
            forceFailure = serializedObject.FindProperty("forceFailure");
            log = serializedObject.FindProperty("log");
            onSuccess = serializedObject.FindProperty("onSuccess");
            onFailure = serializedObject.FindProperty("onFailure");
        }

        public override void OnInspectorGUI() {
            // Manually recreate the inspector so we can add the event foldout:
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((TurnOnXR)target), typeof(TurnOnXR), false);
            GUI.enabled = true;

            serializedObject.Update();

            EditorGUILayout.PropertyField(lifecycleTrigger, new GUIContent("Initialize On"));

            showEvents = EditorGUILayout.Foldout(showEvents, "Completion Events");
            if (showEvents) {
                EditorGUILayout.PropertyField(onSuccess);
                EditorGUILayout.PropertyField(onFailure);
            }

            showDebug = EditorGUILayout.Foldout(showDebug, "Developer Controls");
            if (showDebug) {
                EditorGUILayout.PropertyField(forceFailure);
                EditorGUILayout.PropertyField(log);
            }

            serializedObject.ApplyModifiedProperties();

            // Custom helpboxes:

            if (forceFailure.boolValue == true) {
                EditorGUILayout.HelpBox("This script has ForceFailure enabled. Don't forget to turn that off if it isn't intended!", MessageType.Error);
            }

            var origTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            if (UnityEditorInternal.VR.VREditor.GetVREnabledOnTargetGroup(origTargetGroup) == false) {
                EditorGUILayout.HelpBox("XR support is disabled in this project! You can change this in Project Settings > Player > XR Settings.", MessageType.Error);
                if (GUILayout.Button("Fix it!\n(Enable XR support)")) {
                    UnityEditorInternal.VR.VREditor.SetVREnabledOnTargetGroup(origTargetGroup, true);
                }
            }

            if (XRSettings.supportedDevices.Length == 0 || (XRSettings.supportedDevices.Length == 1 && XRSettings.supportedDevices[0] == "None")) {
                EditorGUILayout.HelpBox("You have no XR devices supported in Project Settings > Player > XR Settings. This script will do nothing.", MessageType.Error);
            }

            if (XRSettings.supportedDevices.Length > 0 && XRSettings.supportedDevices[0] != "None") {
                EditorGUILayout.HelpBox("This script works best when the first supported XR device is `none`. You can change this in Project Settings > Player > XR Settings.", MessageType.Warning);
                if (GUILayout.Button("Fix it!\n(Add `None` to supportedDevices)")) {
                    List<string> devices = new List<string>(XRSettings.supportedDevices);
                    // Just in case it wasn't in the first position.
                    devices.Remove("None");
                    devices.Insert(0, "None");
                    UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(origTargetGroup, devices.ToArray());
                }
            }
        }
    }
#endif
}