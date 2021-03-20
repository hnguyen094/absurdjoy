using UnityEngine;

namespace absurdjoy {
    public abstract class TransformManipulator_Abstract : MonoBehaviour {
        public bool useLocalSpace;
        private Transform target;

        public virtual void SetTarget(Transform target) {
            this.target = target;
        }

        public virtual void Input(Transform newState) {
            Input(TransformValues.FromTransform(newState, false));
        }

        public abstract void Input(TransformValues newState);

        protected virtual void Output(TransformValues newState) {
            if (target == null) {
                return;
            }

            newState.ApplyTo(target, useLocalSpace);
        }
    }
}