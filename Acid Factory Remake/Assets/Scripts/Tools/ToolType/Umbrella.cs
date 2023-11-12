using UnityEngine;

namespace Script.Tools.ToolType {
    public class Umbrella : global::Tools {
        public bool isOpen;
        public Umbrella() {
            lifeSpanTimer = -1;
            isOpen = false;
        }

        public void useItem() {
            isOpen = !isOpen;
        }
    }
}