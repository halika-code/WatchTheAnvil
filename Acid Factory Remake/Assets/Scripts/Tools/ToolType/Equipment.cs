using UnityEngine;

namespace Script.Tools.ToolType {
    public class Equipment : global::Tools{
        private int durability;

        public void initTool(string name) {
            this.name = name;
            durability = 2;
        }

        public static void useItem(Equipment drip, out bool broken) {
            broken = (drip.durability--) <= 1; //note: has to check for 1 since drip.durability is first checked in the logic before the -- is stored
            Debug.Log("Durability is at " + drip.durability + " for " + drip.name + ",have it broke: " + (broken ? "It did" : "Not yet"));
        }

        public override void useItem() {
            throw new System.NotImplementedException();
        }
    }
}