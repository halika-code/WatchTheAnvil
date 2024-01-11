using UnityEngine;

namespace Script.Tools.ToolType {
    
    /**
     * <date></date>
     * <author></author>
     * <summary></summary>
     */
    public class Flower : global::Tools {
        public GameObject flowerBody;
        public bool havePulled;

        /**
         * <summary>Adds a flower to the bouquet</summary>
         */
        public void prepFlower(GameObject flowerObj) {
            flowerBody = flowerObj;
            lifeSpanTimer = -1;
            havePulled = false;
            this.name = flowerObj.name;
        }

        public override void useItem() {
            FlowerController.useItem();
        }
    }
}