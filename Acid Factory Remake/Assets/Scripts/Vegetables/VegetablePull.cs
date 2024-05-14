using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class VegetablePull : MonoBehaviour {
    private static readonly int padding = 2;
    
    /**
     * <summary>Disables the vegetable</summary>
     */
    public static void pullVegetable(Collider veggie) {
        RootVeg.getRoot().removeVeg(veggie.attachedRigidbody, out _);
         //todo find the facing angle the player is looking at the vegetables,
                                            //todo deviate from that slightly then launch the veggies with MoveVel
        findNearestStableGround(veggie.attachedRigidbody);
    }

    private static void findNearestStableGround(Rigidbody veggie) {
        var vegPos = veggie.position;
        veggie.useGravity = true;
        veggie.velocity = Character_Controller.getPlayerBody().position -
                          new Vector3(vegPos.x + getRNG(), vegPos.y + 2f, vegPos.z + getRNG());
        //var distanceAngle = (Character_Controller.getPlayerBody().position - new Vector3(vegPos.x + getRNG(), vegPos.y+ 2f, vegPos.z + getRNG())) * (float)Character_Controller.MoveVel;
        //veggie.position += distanceAngle; //todo create a formula that launches the veggie using velocity (and gravity, toggling it on and off) instead of targeting a spot
        //todo the idea here is there should be a script that listens when a given veggie makes a collision, if that happens turn off the gravity
        //var ray = new Ray(vegPos, distanceAngle); //in theory this is the distance the veggie needs to go

    }

    private static float getRNG() {
        var rand = new Random();
        var next = (float)rand.Next(1, 5);
        if (next % 2 != 0) {
            next *= -0.1f;
        } return next;
    }

    /**
     * <summary>Checks if the vegetable has a valid parent</summary>
     */
    public static bool validateVegetable(GameObject veg) {
        var parent = Character_Controller.getParentName(veg.transform); //breakdown of the chain in an intended state: Veggie1 -> Small -> Carrot -> Vegetable
        return parent != null && parent.Contains("Vegetables");
    }

    /**
     * <summary>Based on the parent-list of a given vegetable,
     * calculates a score of what the object the player have pulled is worth</summary>
     * <param name="parentList">The list (of type string) of gameObjects that are parent to the vegetable</param>
     * <returns>A point value based on the worth of the vegetable</returns>
     * <remarks>It is assumed that the parentList contains the "family tree" of a vegetable</remarks>
     */
    public static int getProfileOfVeggie(List<string> parentList) {
        var score = 1;
        foreach (var parents in parentList) {
            switch (parents) {
                case "Small" or "Carrot" or "Vegetables": {
                    break; //tryin to avoid default with basic plants
                } case "Beetroot": { //if this case is entered the calculations are hijacked a bit
                    score++;
                    RootVeg.updateBeetPoints(score); //score diverted to here
                    score = 0;  //and a 0 is sent to keep the score to a pure all-carrots
                    break;      //it is assumed that if a beetroot is pulled the parentList MUST contain a "Beetroot" entry
                } case "Medium": {
                    score *= 2;
                    break;
                } case "Large": {
                    score *= 5;
                    break;
                } default: {
                    if (parents.Contains("Veggie")) {
                        continue; //making sure the loop will not break out thanks to a rouge Veggie
                    } Debug.Log("Whoopy while trying to decide the score of the object of name " + parentList[0]);
                    break;
                }
            }
        } return score;
    }

    /**
     * <summary>A replica of the <see cref="getProfileOfVeggie(System.Collections.Generic.List{string})"/> but for single vegetables</summary>
     */
    public static int getProfileOfVeggie(string parent) {
        return getProfileOfVeggie(new List<string>{ parent });
    }
}
