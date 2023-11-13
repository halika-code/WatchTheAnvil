using UnityEngine;

namespace Script.Tools.ToolType {
    public class Flower : global::Tools {
        public GameObject flowerBody;
        public bool havePulled;

        /**
         * <summary>Adds a flower to the bouquet</summary>
         */
        public Flower(GameObject flowerObj) {
            flowerBody = flowerObj;
            lifeSpanTimer = -1;
            havePulled = true;
            this.name = flowerObj.name;
        }
    }
}