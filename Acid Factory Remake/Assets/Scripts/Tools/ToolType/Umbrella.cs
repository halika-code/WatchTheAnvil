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

        /**
         * <summary>Checks the status of the hand of the player</summary>
         * <returns>True if: the hand is empty,
         * <para>the name of the object is not the umbrella,</para>
         * the umbrella is not open
         * <para>False only if the umbrella is open</para></returns>
         */
        public static bool checkIfInHand() {
            var hand = Toolbelt.getBelt().toolInHand;
            return hand == null || !hand.name.Contains("Umbrella") ||
                   hand.name.Contains("Umbrella") && !((Umbrella)hand).isOpen;
        }
    }
}