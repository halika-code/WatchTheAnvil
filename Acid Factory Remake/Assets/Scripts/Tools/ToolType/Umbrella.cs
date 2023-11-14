using UnityEngine;

namespace Script.Tools.ToolType {
    public class Umbrella : global::Tools {
        public bool isOpen;
        public void prepUmbrella() {
            lifeSpanTimer = -1;
            isOpen = false;
            this.name = "Umbrella";
        }

        public override void useItem() {
            isOpen = !isOpen;
        }
    }
}