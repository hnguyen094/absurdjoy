using UnityEngine;

namespace absurdjoy {
    public interface IRigidBodyEventReceiver {
        void OnCollisionEnter(Collision collision);
        void OnCollisionExit(Collision collision);
        void OnTriggerEnter(Collider other);
        void OnTriggerExit(Collider other);
    }
}