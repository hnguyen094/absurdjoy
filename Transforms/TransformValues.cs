using UnityEngine;

namespace absurdjoy {
    public struct TransformValues {
        public Vector3 position;
        public Quaternion rotation;

        public TransformValues(Vector3 position, Quaternion rotation) {
            this.position = position;
            this.rotation = rotation;
        }

        public void ApplyTo(Transform target, bool localSpace) {
            if (localSpace) {
                target.localPosition = position;
                target.localRotation = rotation;
            }
            else {
                target.position = position;
                target.rotation = rotation;
            }
        }

        public static TransformValues FromTransform(Transform target, bool localSpace) {
            if (localSpace) {
                return new TransformValues(target.localPosition, target.localRotation);
            }
            else {
                return new TransformValues(target.position, target.rotation);
            }
        }
    }
}