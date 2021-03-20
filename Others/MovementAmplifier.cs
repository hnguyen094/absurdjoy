// https://www.notion.so/Movement-Amplifier-624650bf671e4d72ba8b44de9a7e9d51
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace absurdjoy {
    /// <summary>
    /// A simple script that takes motion from one object, amplifies it, and applies it to another object.
    /// Credit to Kayla Kinnunen for the idea. 
    /// </summary>
    public class MovementAmplifier : MonoBehaviour {
        [Tooltip("How much additional movement should we apply, as a percentage? Eg: 0.1 = extra 10cm per meter.")]
        public Vector3 axisAmplification = new Vector3(0.2f, 0, 0.2f);

        [Tooltip("What transform should we monitor for motion? Uses 0,0 centered localPosition to determine deflection. Defaults to this gameobject if unset.")]
        public Transform sourceOfMovement; // Usually something like the MainCamera childed to this

        [Tooltip("What transform should we apply the amplified amount to? Defaults to parent if unset.")]
        public Transform amplificationTarget; // Usually this, if the source is a child

        [Tooltip("Should we use the Update or FixedUpdate?")]
        public bool useFixedUpdate;

        // Local variable to prevent garbage collection.
        private Vector3 movementDeflection;

        private void OnEnable() {
            Initialize();
        }

        private void Initialize() {
            // Just do some basic housekeeping...
            if (sourceOfMovement == null) {
                sourceOfMovement = transform;
            }

            if (amplificationTarget == null) {
                amplificationTarget = transform.parent;
                if (amplificationTarget == null) {
                    Debug.LogError("Can't have null amplificationTarget or parentless object, disabling script", gameObject);
                    this.enabled = false;
                    return;
                }
            }

            if (sourceOfMovement == amplificationTarget) {
                Debug.LogError("Source and target being the same will result in an infinite acceleration loop. Disabling script.", gameObject);
                this.enabled = false;
            }
        }

        private void Update() {
            if (!useFixedUpdate) {
                AmplifyMovement();
            }
        }

        private void FixedUpdate() {
            if (useFixedUpdate) {
                AmplifyMovement();
            }
        }

        private void AmplifyMovement() {
            movementDeflection = sourceOfMovement.localPosition;
            movementDeflection.x *= axisAmplification.x;
            movementDeflection.y *= axisAmplification.y;
            movementDeflection.z *= axisAmplification.z;
            amplificationTarget.localPosition = movementDeflection;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MovementAmplifier))]
    public class MovementAmplifierEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            var ourScript = (MovementAmplifier)target;
            if (ourScript.amplificationTarget == null && ourScript.transform.parent == null) {
                EditorGUILayout.HelpBox("This script requires either: (1) A defined amplificationTarget, or (2) this GameObject must be a child of another GameObject.", MessageType.Error);
            }

            if (ourScript.axisAmplification == Vector3.zero) {
                EditorGUILayout.HelpBox("Zero axisAmplification is the same as not having this script enabled.", MessageType.Warning);
            }
            else if (ourScript.axisAmplification.x == ourScript.axisAmplification.y && ourScript.axisAmplification.x == ourScript.axisAmplification.z) {
                EditorGUILayout.HelpBox("Symmetric axisAmplification behaves/feels as if it were a scale change.", MessageType.Info);
            }

            var origTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            // Magic number is roughly > 20% on all three axes.
            if (ourScript.axisAmplification.magnitude > 0.347f && UnityEditorInternal.VR.VREditor.GetVREnabledOnTargetGroup(origTargetGroup)) {
                EditorGUILayout.HelpBox("Large amplification values can affect XR user balance and/or dizzyness.", MessageType.Warning);
            }

            if (ourScript.sourceOfMovement != null && ourScript.sourceOfMovement == ourScript.amplificationTarget) {
                EditorGUILayout.HelpBox("Source and Target cannot be the same.", MessageType.Error);
            }
        }
    }
#endif
}