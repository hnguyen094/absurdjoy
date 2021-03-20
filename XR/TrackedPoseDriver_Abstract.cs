using UnityEngine;

namespace absurdjoy {
    /// <summary>
    /// This class mimics Unity's XR Legacy package, but allows
    /// similar interaction with any source via extensions.
    /// </summary>
    public abstract class TrackedPoseDriver_Abstract : XRNode_Abstract {
        public enum TrackingType {
            RotationAndPosition,
            Position,
            Rotation
        }

        [Header("Tracking")]
        [Tooltip("What type of tracking should we be applying?")]
        public TrackingType trackingType;

        public enum UpdateType {
            AllButRigidbody,
            Update,
            FixedUpdate,
            BeforeRender,
            Rigidbody
        }

        [Tooltip("When should we apply the tracking?")]
        public UpdateType updateType;

        private TransformValues newTransformValue;
        private TransformManipulator_Abstract transformManipulator;
        private Rigidbody ourRigidbody;

        protected override void OnEnable() {
            base.OnEnable();

            // Configure the transform manipulator (if any)
            newTransformValue = TransformValues.FromTransform(transform, false);
            transformManipulator = GetComponent<TransformManipulator_Abstract>();
            if (transformManipulator != null) {
                transformManipulator.SetTarget(transform);
            }

            ourRigidbody = GetComponentInParent<Rigidbody>();

            Application.onBeforeRender += OnBeforeRender;
        }

        protected override void OnDisable() {
            base.OnDisable();
            Application.onBeforeRender -= OnBeforeRender;
        }

        protected void Update() {
            if (updateType == UpdateType.Update || updateType == UpdateType.AllButRigidbody) {
                PerformUpdate();
            }
        }

        protected void FixedUpdate() {
            if (updateType == UpdateType.FixedUpdate || updateType == UpdateType.AllButRigidbody || updateType == UpdateType.Rigidbody) {
                PerformUpdate();
            }
        }

        protected void OnBeforeRender() {
            if (updateType == UpdateType.BeforeRender || updateType == UpdateType.AllButRigidbody) {
                PerformUpdate();
            }
        }

        protected void PerformUpdate() {
            if (!enabled) {
                // Required because the OnBeforeRender delegate registration
                // will happen even if this is disabled.
                return;
            }

            if (IsDeviceValid()) {
                SetLocalTransform(GetPosition(), GetRotation());
            }
        }

        protected virtual void SetLocalTransform(Vector3 newPosition, Quaternion newRotation) {
            // Only apply values if they are valid for this config:
            if (trackingType == TrackingType.Rotation || trackingType == TrackingType.RotationAndPosition) {
                newTransformValue.rotation = newRotation;
            }

            if (trackingType == TrackingType.Position || trackingType == TrackingType.RotationAndPosition) {
                newTransformValue.position = newPosition;
            }

            if (transformManipulator != null) {
                // Use the transform manipulator:
                transformManipulator.Input(newTransformValue);
            }
            else {
                if (updateType == UpdateType.Rigidbody) {
                    // TODO: these are in local space. fix!
                    ourRigidbody.MovePosition(newTransformValue.position);
                    ourRigidbody.MoveRotation(newTransformValue.rotation);
                    ourRigidbody.velocity = GetVelocity();
                    ourRigidbody.angularVelocity = GetAngularVelocity();
                }
                else {
                    // Direct set:
                    newTransformValue.ApplyTo(transform, true);
                }
            }
        }

        public Vector3 TransformToLocal(Vector3 vec3) {
            if (transform.parent == null) {
                return vec3;
            }
            return transform.parent.TransformVector(vec3);
        }

        public Vector3 GetLocalVelocity() {
            return TransformToLocal(GetVelocity());
        }

        public Vector3 GetLocalAngularVelocity() {
            return TransformToLocal(GetAngularVelocity());
        }

        public Vector3 GetLocalAcceleration() {
            return TransformToLocal(GetAcceleration());
        }

        public Vector3 GetLocalAngularAcceleration() {
            return TransformToLocal(GetAngularAcceleration());
        }

        // Tracking data:
        public abstract bool IsTrackingValid();

        public abstract Vector3 GetPosition();
        public abstract Quaternion GetRotation();

        public abstract Vector3 GetVelocity();
        public abstract Vector3 GetAngularVelocity();
        public abstract Vector3 GetAcceleration();
        public abstract Vector3 GetAngularAcceleration();
    }
}