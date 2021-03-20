using UnityEngine;

namespace absurdjoy {
    public class TransformManipulator_Delay : TransformManipulator_Abstract {
        public bool useSync = true;

        [MyBox.ConditionalField("useSync", false, false)]
        [Tooltip("How many frames you want to delay the output for.")]
        public uint delayFrames;

        [MyBox.ConditionalField("useSync", false, false)]
        [Tooltip("How many frames to randomly jitter the delay value by on startup. Set to 0 for no random.")]
        public uint delayFramesRandomJitter;

        private TransformValues[] history;
        private int index;

        private uint GetHistoryLength() {
            if (useSync) {
                var sync = GetComponentInParent<TransformManipulator_DelaySync>();
                if (sync != null) {
                    return sync.GetDelayFrames();
                }
                else {
                    Debug.LogWarning("UseSync was set, but no sync object was found.", gameObject);
                }
            }

            return delayFrames + (uint)Random.Range(0, delayFramesRandomJitter);
        }

        public override void Input(TransformValues newState) {
            if (history == null) {
                history = new TransformValues[GetHistoryLength()];
                // So there aren't null values in history, just replicate this first value to the entire thing:
                for (int i = 0; i < history.Length; i++) {
                    history[i] = newState;
                }
                index = 0;
            }

            if (history.Length == 0) {
                Output(newState);
            }

            // Because the array is a fixed size, the current cell is also the oldest cell.
            // So let's apply the change now:
            Output(history[index]);

            // Now overwrite this spot in history:
            history[index] = newState;

            // and increment the index.
            index++;
            if (index >= history.Length) {
                index = 0;
            }
        }
    }
}