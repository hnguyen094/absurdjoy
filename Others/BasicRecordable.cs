// https://www.notion.so/Recording-and-Playback-6122c49cd3494b96a6fadb0cd7ed8ec5
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.Timeline;
#endif

using System;
using UnityEngine;

namespace absurdjoy {
    /// <summary>
    /// This class uses the GameObjectRecorder (an editor-only-script) to record
    /// the position of an object (optionally: children too) to an animation asset.
    /// Useful if you want to record -- say -- a VR avatar doing a dance. Or whatever.
    ///
    /// This script can only be run while in the Editor (not in compiled projects).
    ///
    /// TODO:
    /// Legacy Animations can be saved at runtime using this method:
    /// https://answers.unity.com/questions/911169/procudurally-generate-animationclip-at-runtime.html
    /// </summary>
    public class BasicRecordable : MonoBehaviour {
        public enum RecordType {
            LegacyAnimation, // Creates a Legacy Animation Clip you can insert into an Animation playback component.
            Animator,        // Creates an Animation Clip, and associates it with the newer Animator Controller component.
            Timeline         // Creates an Animation Clip, and associates it with the newest Timeline component. 
        }

        [Tooltip("Which type of asset do you want to record to?")]
        public RecordType recordingMethod;
        [Tooltip("What directory in the assets folder should we use for saving? (blank for root)")]
        public string saveFolder;

        [Tooltip("Which object are we recording?")]
        public GameObject targetObject;
        [Tooltip("Should we record the children of the target transform?")]
        public bool recursive;

        [MyBox.ReadOnly]
        public bool isRecording;

#if UNITY_EDITOR
        protected GameObjectRecorder recorder;

        /// <summary>
        /// Starts the recording process.
        /// </summary>
        protected void BeginRecord() {
            if (isRecording) {
                return;
            }

            isRecording = true;

            Debug.Log("<color=red>•</color> Recording");

            if (targetObject == null) {
                targetObject = this.gameObject;
            }

            recorder = new GameObjectRecorder(targetObject);
            recorder.BindComponentsOfType<Transform>(targetObject, recursive);
        }

        /// <summary>
        /// Concludes recording and saves assets.
        /// </summary>
        protected void EndRecord() {
            if (!isRecording) {
                return;
            }

            Debug.Log("<color=white>■</color> Recording Complete");

            // Dump the clip to the folder, which will be required for everything:
            var clip = new AnimationClip();
            if (recordingMethod == RecordType.LegacyAnimation) {
                clip.legacy = true;
            }

            recorder.SaveToClip(clip);
            SaveToDisk(clip, "anim");

            if (recordingMethod == RecordType.Timeline) {
                // Create the timeline asset:
                var timelineAsset = TimelineAsset.CreateInstance<TimelineAsset>();
                SaveToDisk(timelineAsset, "playable");
                // (Asset needs to be saved to disk first or else changes won't propagate)

                var track = timelineAsset.CreateTrack<AnimationTrack>();
                track.CreateClip(clip);
            }
            else if (recordingMethod == RecordType.Animator) {
                // Create the animator (controller) asset:
                var controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/" + saveFolder + "/" + clip.name + "_controller.controller");
                controller.AddMotion(clip, 0);
            }

            recorder = null;
            isRecording = false;
        }

        /// <summary>
        /// Save an asset to disk with the given extension.
        /// </summary>
        /// <param name="obj">Asset to save</param>
        /// <param name="extension">the filename extension to assign</param>
        protected void SaveToDisk(UnityEngine.Object obj, string extension) {
            if (obj != null) {
                // If the name is null or empty the save function fails pretty hard. Let's fix that:
                if (string.IsNullOrEmpty(obj.name)) {
                    // Generate a pseudo-random name for the clip so we (probably) won't get collisions or filesystem errors. Not bombproof.
                    obj.name = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8).Replace('/', '_').Replace('+', '-');
                }

                var path = AssetDatabase.GenerateUniqueAssetPath("Assets/" + saveFolder + "/" + obj.name + "." + extension);
                Debug.Log("<color=green>✓</color> Saving Asset: " + path);
                AssetDatabase.CreateAsset(obj, path);
                AssetDatabase.SaveAssets();
            }
        }

        protected virtual void LateUpdate() {
            if (recorder != null) {
                recorder.TakeSnapshot(Time.deltaTime);
            }
        }
#endif

        /// <summary>
        /// Toggle recording on or off.
        /// </summary>
        public void ToggleRecord() {
            if (isRecording) {
                EndRecord();
            }
            else {
                BeginRecord();
            }
        }
    }

#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BasicRecordable))]
    public class BasicRecordableEditor : Editor {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Not readonly--used in Editor")]
        private bool isRecording = false;
        private BasicRecordable ourScript;

        void OnEnable() {
            ourScript = (BasicRecordable)target;
        }

        /// <summary>
        /// Adds a Stop/Start recording button in the inspector for manual invocation.
        /// </summary>
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if (EditorApplication.isPlaying) {
                string buttonText = "Start Recording";
                if (ourScript.isRecording) {
                    buttonText = "Stop Recording";
                }
                if (GUILayout.Button(buttonText)) {
                    ourScript.ToggleRecord();
                }
            }
        }
    }
#endif
}