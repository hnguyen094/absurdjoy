using UnityEngine;

namespace absurdjoy {
    public class TransformManipulator_DelaySync : MonoBehaviour {
        public uint delayFrames;
        public uint randomJitter;

        private bool isSet = false;
        private uint setValue;

        public uint GetDelayFrames() {
            if (!isSet) {
                setValue = delayFrames + (uint)Random.Range(0, randomJitter);
                isSet = true;
            }

            return setValue;
        }
    }
}