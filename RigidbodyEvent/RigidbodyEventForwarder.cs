// https://www.notion.so/Rigidbody-Event-Forwarding-7412ea7d963444f8a03af86b628a13ee
using System.Collections.Generic;
using UnityEngine;

namespace absurdjoy {
    /// <summary>
    /// This will take Rigidbody events (eg: OnCollisionEnter) and forward them to an arbitrary receiver, not necessarily a monobehaviour.
    /// Includes logic to automatically detect receivers in children, parents, or both.
    /// </summary>
    public class RigidbodyEventForwarder : LifecycleBase {
        public enum Mode {
            None,
            Children,
            Parent,
            Both
        }

        [Tooltip("If the hierarchy of this object changes, should we Trigger?")]
        public bool triggerOnChildChange = true;
        [Tooltip("On Trigger, where should we auto detect receivers from?")]
        public Mode autoGetFrom = Mode.Children;

        private List<IRigidBodyEventReceiver> receivers;

        /// <summary>
        /// Can be called from code or via the LifecycleBase dropdown.
        /// </summary>
        public override void Trigger() {
            SetReceivers(autoGetFrom);
        }

        private void OnTransformChildrenChanged() {
            if (triggerOnChildChange) {
                SetReceivers(autoGetFrom);
            }
        }

        /// <summary>
        /// Overwrites the current list of receivers given the supplied mode. 
        /// </summary>
        public void SetReceivers(Mode getFrom) {
            if (getFrom == Mode.None) {
                receivers = null;
            }
            else {
                List<IRigidBodyEventReceiver> r = new List<IRigidBodyEventReceiver>();
                if (getFrom == Mode.Children || getFrom == Mode.Both) {
                    r.AddRange(GetComponentsInChildren<IRigidBodyEventReceiver>());
                }

                if (getFrom == Mode.Parent || getFrom == Mode.Both) {
                    r.Add(GetComponentInParent<IRigidBodyEventReceiver>());
                }

                SetReceivers(r);
            }
        }

        /// <summary>
        /// Overwrite the current set of receivers with this new list.
        /// </summary>
        public void SetReceivers(List<IRigidBodyEventReceiver> to) {
            receivers = to;
        }

        /// <summary>
        /// Overwrite the current set of receivers with this new list.
        /// </summary>
        public void SetReceivers(IRigidBodyEventReceiver[] to) {
            receivers = new List<IRigidBodyEventReceiver>(to);
        }

        /// <summary>
        /// Adds a receiver to the list. Does not check for duplicates. 
        /// </summary>
        public void AddReceiver(IRigidBodyEventReceiver receiver) {
            receivers.Add(receiver);
        }

        /// <summary>
        /// Removes a receiver from the list. 
        /// </summary>
        /// <returns>true if the item was in the list, false otherwise</returns>
        public bool RemoveReceiver(IRigidBodyEventReceiver receiver) {
            return receivers.Remove(receiver);
        }


        // Events to forward:

        public void OnCollisionEnter(Collision collision) {
            if (receivers == null) {
                return;
            }
            foreach (var receiver in receivers) {
                receiver.OnCollisionEnter(collision);
            }
        }

        public void OnCollisionExit(Collision collision) {
            if (receivers == null) {
                return;
            }
            foreach (var receiver in receivers) {
                receiver.OnCollisionExit(collision);
            }
        }

        public void OnTriggerEnter(Collider other) {
            if (receivers == null) {
                return;
            }
            foreach (var receiver in receivers) {
                receiver.OnTriggerEnter(other);
            }
        }

        public void OnTriggerExit(Collider other) {
            if (receivers == null) {
                return;
            }
            foreach (var receiver in receivers) {
                receiver.OnTriggerExit(other);
            }
        }
    }
}