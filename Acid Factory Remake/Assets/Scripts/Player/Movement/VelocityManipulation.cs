using System;
using Script.Tools.ToolType;
using UnityEngine;
using static Character_Controller;
using Task = System.Threading.Tasks.Task;

public static class VelocityManipulation {

    private static float xSlowDown = 0.04f;
    private static float ySpeed = 0f;
    
    /**
     * <summary>Calculates what speed the player should have</summary>
     * <remarks>Will only get applied to the player's speed if the player haven't pressed buttons mid-processing</remarks>
     */
    public static Vector3 calculateDampenedVelocity() { 
        if (GravAmplifier.isAscending) {
            var pVel = getPlayerBody().velocity;
            if (flyingVector == new Vector3(0f, pVel.y, 0f)) { //if the player just jumped
                return pVel;
            } calculateFlyingVelocity(pVel);  //if the player is flying in the air
        } else {
            flyingVector = new Vector3(0f, getPlayerBody().velocity.y, 0f);
        } return flyingVector;
    }

    /**
     * <summary>Modifies the calculated velocity for the player to be in it's airborne state</summary>
     */
    private static void calculateFlyingVelocity(Vector3 pVel) { //todo tinker with the movement script in the InputController
        var haveChanged = false; 
        var diff = flyingVector - pVel; //getting the change in direction and it's angle 
        for (var i = 0; i < 3; i+=2) {  //todo the dampening isn't doing isn't proper thing, for some reason its overachieving
            if (checkForDifference(diff[i], i)) { //filtering for small changes //1.6f for single and 4.38f for multipress
                flyingVector[i] = Math.Sign(flyingVector[i]) * (Math.Abs(flyingVector[i]) / 
                    (float)DampeningCoefficient * (checkAgainstUmbrella() ? 1f : 1.5f)); //this multiplier crops the gliding distance of the umbrella 
                haveChanged = true;
                Debug.Log(flyingVector[i]);
            } //dividing makes sure the value gets smaller than the original velocity
        } if (!haveChanged) { 
            flyingVector = pVel; //gets the default values in case no significant change is detected
        }
    }

    private static bool checkForDifference(float diff, int i) {
        return absRound(diff) > 1.5f && absRound(flyingVector[i]) > 0.2f;
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
    * <summary>Slowly decreases the player's velocity</summary>
    * <remarks>Breaks early if the player is on the ground</remarks>
    */
    public static void velocityDecay() {
        if (GravAmplifier.isAscending) { //if false, breaks early
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
