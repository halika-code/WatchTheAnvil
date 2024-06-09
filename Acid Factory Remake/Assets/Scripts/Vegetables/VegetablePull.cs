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
        findNearestStableGround(veggie.attachedRigidbody);
    }

    private static void findNearestStableGround(Rigidbody veggie) {
        var vegPos = veggie.position;
        var pPos = Character_Controller.getPlayerBody().position;
        veggie.useGravity = true;
        veggie.velocity = new Vector3(pPos.x - (vegPos.x + 5 * getRNG()), vegPos.y + 25f, pPos.z - (vegPos.z + 5 * getRNG()));
    }

    private static float getRNG() {
        var next = (float)new Random().Next(2, 6);
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
        foreach (var parents in parentList) { //checks the entire hiearchy of the veggie, applying a multiplier usually once or multiple times.
            switch (parents) {
                case "Small" or "Carrot" or "Vegetables": {
                    break; //returns usually a score of 1
                } case "Beetroot": { //if this case is entered the calculations are hijacked a bit
                    RootVeg.updateBeetPoints(1 + score); //score diverted to here
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
