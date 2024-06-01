using UnityEngine;

namespace Script.Tools.ToolType {
    public class Umbrella : global::Tools { 
        public bool isOpen;
        private Transform head;
        private static Vector3 open;
        private static Vector3 closed;
        public void prepUmbrella() {
            lifeSpanTimer = -1;
            isOpen = false;
            name = "Umbrella";
            head = GameObject.Find("UmbrellaHead").transform;
            closed = head.localScale;
            open = new Vector3(20f, 0.005f, 20f);
        }

        public override void useItem() {
            isOpen = !isOpen; 
            GravAmplifier.gravity.updateSpeedCap(isOpen);
            head.localScale = isOpen ? open : closed;
            head.localPosition = isOpen ? new Vector3(0, 0.5f, 0) : new Vector3(0, 0.23f, 0) ;
        }
        
        /**
         * <summary>Checks the status of the umbrella in the hands of the player</summary>
         * <returns>Only true when the umbrella is in use, false otherwise</returns>
         * <remarks>Multiple integrity checks are performed to ensure the player doesn't try floating with a stopwatch for example</remarks>
         */
        public bool checkIfOpen() {
            var hand = Toolbelt.getBelt().toolInHand;
            return hand == null || !hand.name.Contains("Umbrella") || isOpen;
        }
    }
}