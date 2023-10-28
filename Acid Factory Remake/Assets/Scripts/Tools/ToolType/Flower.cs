
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