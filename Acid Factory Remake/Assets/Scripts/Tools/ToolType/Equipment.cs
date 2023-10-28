using UnityEngine;

namespace Script.Tools.ToolType {
    public class Equipment : global::Tools {
        private int durability;

        public Equipment(string name) {
            this.name = name;
        }

        public static void useItem(Equipment drip, out bool broken) { 
            broken = (drip.durability--) <= 0;
            Debug.Log("Durability is at " + drip.durability + " for " + drip.name + ", broken: " + broken);
        }
    }
}