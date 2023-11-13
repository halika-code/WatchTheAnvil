using UnityEngine;

namespace Script.Tools.ToolType {
    public class Equipment : global::Tools {
        private int durability;

        public Equipment(string name) {
            this.name = name;
            durability = 2;
        }

        public static void useItem(Equipment drip, out bool broken) { 
            broken = (drip.durability--) <= 0;
            Debug.Log("Durability is at " + drip.durability + " for " + drip.name + ",have it broke: " + (broken ? "It did" : "Not yet"));
        }
    }
}