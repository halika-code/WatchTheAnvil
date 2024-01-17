using UnityEngine;

namespace Script.Tools.ToolType {
    
    /**
     * <date>28/10/2023</date>
     * <author>Gyula Attila Kovacs</author>
     * <summary>A script that handles individual interactions for flowers</summary>
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