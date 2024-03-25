using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VegetablePull : MonoBehaviour {
    
    /**
     * <summary>Disables the vegetable</summary>
     */
    public static void pullVegetable(Collider veggie) {
        RootVeg.getRoot().removeVeg(veggie.attachedRigidbody, out _);
        veggie.gameObject.SetActive(false); //todo create placement for the carrots to fly to
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
