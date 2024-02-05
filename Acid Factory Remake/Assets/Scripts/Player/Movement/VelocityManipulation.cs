using System;
using Script.Tools.ToolType;
using UnityEngine;
using static Character_Controller;

public static class VelocityManipulation {
    
    /**
     * <summary>Decides what speed the player should be going at the start of the frame</summary>
     */
    public static Vector3 calculateVelocity() { //new Vector3(0f, pBody.velocity.y, 0f)
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
    private static void calculateFlyingVelocity(Vector3 pVel) {
        var diff = flyingVector - pVel; //getting the change in direction
        flyingVector = pVel; //gets the default values in case no change is detected
        for (var i = 0; i < 3; i+=2) {
            if (Math.Abs(Math.Round(diff[i], 1)) > 1.5f) { //filtering for small changes //1.6f for single and 4.38f for multipress
                flyingVector[i] = Math.Sign(flyingVector[i]) * (Math.Abs(flyingVector[i]) / 
                    (float)DampeningCoefficient * (checkAgainstUmbrella() ? 0.8f : 1.5f)); //this multiplier crops the gliding distance of the umbrella 
            } //dividing makes sure the value gets smaller than the original velocity
        }
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
                if (Math.Abs(Math.Round(pVel[i])) > 0.05f) {
                    pVel[i] -= checkAgainstUmbrella() ? Math.Sign(pVel[i]) * 0.038f: Math.Sign(pVel[i]) * 0.08f; //normal velocity slowdown : umbrella slowdown
                } else {
                    pVel[i] = 0;
                }
            } getPlayerBody().velocity = pVel;
        }
    }
}
