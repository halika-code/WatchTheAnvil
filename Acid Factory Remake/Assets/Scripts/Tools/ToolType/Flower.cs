
using UnityEngine;

namespace Script.Tools.ToolType {
    public class Flower : global::Tools {
        private int count;
        public Flower() {
            count = 1;
            lifeSpanTimer = -1;
        }

        /**
         * <summary>Adds a single flower into the collection</summary>
         */
        public static void addFlower(Flower banquet) {
            banquet.count++;
        }

        public static void checkForFlowerDistance() {
            var pBody = Character_Controller.getPlayerBody().position;
            //if (VegStateController.pClose(VegStateController.pInBorder(new[] { pBody.x, pBody.y, pBody.z }, new[] {/*A specific flower supposed to be here*/ }))) {
                //todo get a flower-controller where all the flowers are kept and IF the player is close, get a timer up where if the flower is up
                //for 5 seconds, it will hide away
            //}
        }

        /**
         * <summary>Decides how many lives the player should receive for his/her flower bouquet</summary>
         */
        public static void useItem(Flower bouquet) {
            var hp = UI.getHealthPoints();
            switch (bouquet.count) {
                case 0 or 1: {
                    hp++;
                    break;
                } case 2: {
                    hp += 2;
                    break;
                } case >2: {
                    hp += 5;
                    break;
                } default: {
                    Debug.Log("Whoopy in trying to get hearts for " + bouquet.count + " flowers");
                    break;
                }
            } UI.updateHealthPoint(hp);
        }
    }
}