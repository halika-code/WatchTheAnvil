using System;
using Script.Tools.ToolType;
using UnityEngine;
using static Character_Controller;
using static InputController;
using Task = System.Threading.Tasks.Task;

public static class VelocityManipulation {
    
    private static float xSlowDown = 0.08f;
    
    #region InputSpecificCalculations
        private static float[] dampenedSpeed = {0f, 0f};
        private static bool[] isDampening = {false, false};
    
        /**
         * <summary>Slowly decreases the player's velocity IF the player is in the air</summary>
         * <remarks>Breaks early if the player is on the ground</remarks>
         */
        public static void velocityDecay() {
            var pVel = getPlayerBody().velocity;
            for (var i = 0; i < 2; i+=2) {
                if (absRound(pVel[i]) > 0.05f) {
                    pVel[i] -= checkAgainstUmbrella() ? Math.Sign(pVel[i]) * xSlowDown : 
                        Math.Sign(pVel[i]) * xSlowDown + 0.05f; //normal velocity slowdown : umbrella slowdown
                } else {
                    pVel[i] = 0;
                }
            } getPlayerBody().velocity = pVel;
        }
        
        /**
         * <summary>Adds an initial speed penalty to the player during the first handful of frames of movement</summary>
         */
        public static float processInitialDampening(int i, float velocity) {
            if (absRound(dampenedSpeed[truncateIndex(i, false)]) < 0.1f || absRound(getPlayerBody().velocity[truncateIndex(i)]) < 10f) {
                dampenSpeed(i, velocity * (float)MoveVel + 2f);
                return dampenedSpeed[truncateIndex(i, false)];
            } return incrementPlayerSpeed(velocity * (float)MoveVel + 2f); 
        }
            
        /**
         * <summary>Increments the dampening component of the player's speed</summary>
         */
        private static void dampenSpeed(int i, float speed) {
            if (!isDampening[truncateIndex(i, false)]) { //if the player have zero speed
                isDampening[truncateIndex(i, false)] = true;
                dampenedSpeed[truncateIndex(i, false)] = speed * 0.2f;
            } dampenedSpeed[truncateIndex(i, false)] *= 1.07f;
        }
        
        /**
         * <summary>Hard-resets the player's dampening components</summary>
         */
        public static void resetDampening(int i) {
            isDampening[truncateIndex(i, false)] = false;
            dampenedSpeed[truncateIndex(i, false)] = 0f;
        }

        /**
         * <summary>Sets the dampening to a given speed</summary>
         */
        public static void setDampening(float val, int i) {
            isDampening[truncateIndex(i, false)] = true;
            dampenedSpeed[truncateIndex(i, false)] = val;
        }
        
    #endregion
    
    /**
     * <summary>Increments then checks the player's speed</summary>
     */
    public static float incrementPlayerSpeed(float desiredSpeed, float limit = (float)MoveVel * 1.5f) {
        return Math.Abs((int)desiredSpeed) > (int)limit ? Math.Sign(desiredSpeed) * limit : desiredSpeed; //casting to int equals to a Math.Floor statement
    }

    /**
     * <summary>Checks if the umbrella is open</summary>
     * <returns>False if the hand is empty, if the tool in the hand is not an umbrella or if the umbrella is not open.
     * <para>True if all the above are true</para></returns>
     */
    public static bool checkAgainstUmbrella() {
        var tool = Toolbelt.getBelt().toolInHand;
        return Toolbelt.checkHand() && tool.name.Contains("Umbrella") && ((Umbrella)tool).isOpen;
    }

    /**
     * <summary>Increments the speed-down slowly in a given time-frame</summary>
     * <param name="wait">Any float second</param>
     * <remarks>Update is every second, the time-frame decreased by 0.8f
     * <para>Decrease the minus statement to increase the strength of the speed down</para></remarks>
     */
    public static async void incrementXSpeedDown(float wait) {
        while (wait > 0f || GravAmplifier.isAscending) {
            xSlowDown -= 0.0005f;
            await Task.Delay(300);
            wait -= 0.8f;
        } xSlowDown = 0.03f;
    }
    
    /**
     * <summary>Removes the residual velocity the player may have in select circumstances</summary>
     * <param name="placement">The index of the given axis.
     * <para>Can only range from 0 to 2</para>
     * Will do no operation when given an index of 1</param>
     * <remarks>See: Player colliding with a wall in the air</remarks>
     */
    public static void stopPlayerVelocity(int placement) {
        var pBody = getPlayerBody().velocity;
        if (pBody[placement] != 0) { //can only check x and z. Checks in order to avoid unnecessary writes
            getPlayerBody().velocity = new Vector3((placement is 0 ? pBody.x: 0), pBody.y, (placement is 2 ? pBody.z : 0)); //doesn't matter which parity the 
        }
    }

    /**
     * <summary>Returns a rounded (to the 2nd digit) and absolute value</summary>
     */
    public static double absRound(float num) {
        return Math.Abs(Math.Round(num, 2));
    }
}
