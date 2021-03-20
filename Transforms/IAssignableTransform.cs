using UnityEngine;

namespace absurdjoy {
    public interface IAssignableTransform {
        bool AssignTransform(Transform newTransform);

        bool RemoveAssignedTransform(Transform toRemove);
    }
}