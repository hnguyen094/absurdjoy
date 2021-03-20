using UnityEngine;

namespace absurdjoy {
    public class SmoothDampTransform : MonoBehaviour, IAssignableTransform {
        [Tooltip("What transform should we try to follow? (Optional)")]
        public Transform targetTransform;

        // TODO: Hide this section in editor if not tracking.
        [Header("Translation")] public bool trackPosition = true;

        [Tooltip("Overwritten if targetTransform is not null")]
        [MyBox.ReadOnly]
        public Vector3 targetPosition;

        [Tooltip("How far should we offset the transform position?")]
        public Vector3 positionOffset = Vector3.zero;

        [Tooltip("How much time should pass before you arrive at the target? (higher is more damped)")]
        [Range(0, 1)]
        public float translateSmoothTime = 0.3f;

        // TODO: Hide this section in editor if not tracking.
        [Header("Rotation")] public bool trackRotation = true;

        [Tooltip("Overwritten if targetTransform is not null")]
        [MyBox.ReadOnly]
        public Quaternion targetRotation;

        [Tooltip("How far should we offset the transform rotation?")]
        public Quaternion rotationOffset = Quaternion.identity;

        [Tooltip("What amount of slerping sould we do on the rotation? (lower is more damped)")]
        [Range(0, 1), UnityEngine.Serialization.FormerlySerializedAs("rotateSlerpAmount")]
        public float rotationSmoothTime = 0.3f;

        // Storing last values:
        protected Vector3 prevPosition = Vector3.zero;
        protected Quaternion prevRotation = Quaternion.identity;

        // Velocity values used by smoothdamp:
        protected Vector3 moveVel = Vector3.zero;
        protected Vector3 rotDirectionVel = Vector3.zero;
        protected float rotRollVel = 0;

        protected virtual void OnEnable() {
            prevPosition = transform.position;
            prevRotation = transform.rotation;
        }

        protected virtual void Update() {
            if (targetTransform == null) {
                // If the targetTransform is null, either:
                // (a) we are manually setting the destination in code elsewhere, or
                // (b) the chased object was destroyed and we're drifting to it's last spot.
                // this is fine.
            }
            else {
                targetPosition = targetTransform.position;
                targetRotation = targetTransform.rotation;
            }

            if (trackRotation) {
                Quaternion offsetRotTarget = targetRotation * rotationOffset;
                float angleDiff = Quaternion.Angle(transform.rotation, offsetRotTarget);
                float maxDegrees = angleDiff - Mathf.SmoothDamp(angleDiff, 0, ref rotRollVel, rotationSmoothTime);
                prevRotation = Quaternion.RotateTowards(transform.rotation, offsetRotTarget, maxDegrees);
                this.transform.rotation = prevRotation;
            }

            if (trackPosition) {
                Vector3 offsetPosTarget = targetPosition + (targetRotation * positionOffset);
                prevPosition = Vector3.SmoothDamp(prevPosition, offsetPosTarget, ref moveVel, translateSmoothTime);
                transform.position = prevPosition;
            }
        }

        #region IAssignableTransform		
        // For IAssignableTransform:
        public bool AssignTransform(Transform newTransform) {
            targetTransform = newTransform;
            return true;
        }

        public bool RemoveAssignedTransform(Transform toRemove) {
            targetTransform = null;
            return true;
        }

        public Transform GetAssignedTransform() {
            return targetTransform;
        }
        #endregion
    }
}