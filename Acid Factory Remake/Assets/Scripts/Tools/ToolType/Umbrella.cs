namespace Script.Tools.ToolType {
    public class Umbrella : global::Tools { 
        public bool isOpen;
        public void prepUmbrella() {
            lifeSpanTimer = -1;
            isOpen = false;
            this.name = "Umbrella";
        }

        public override void useItem() {
            GravAmplifier.gravity.updateSpeedCap(!isOpen);
            isOpen = !isOpen; 
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