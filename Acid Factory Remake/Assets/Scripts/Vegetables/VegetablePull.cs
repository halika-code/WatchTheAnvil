using System.Collections.Generic;
using UnityEngine;

public class VegetablePull : MonoBehaviour {
    
    /**
     * <summary>Disables the vegetable</summary>
     */
    public static void pullVegetable(Collider veggie) {
        veggie.gameObject.SetActive(false);
    }

    /**
     * <summary>Checks if the vegetable has a valid parent</summary>
     */
    public static bool validateVegetable(GameObject veg) {
        var parent = Character_Controller.getParentName(veg); //breakdown of the chain in an intended state: Veggie1 -> Small -> Carrot -> Vegetable
        return parent != null && parent.name.Contains("Vegetables");
    }

    /**
     * <summary>Assembles every parent for the given object into a list up to the root object (not inclusive)</summary>
     * <param name="obj">The object that should be examined</param>
     * <returns>The list (of type string) of the "family tree"</returns>
     * <remarks>Works with objects that doesn't "normally" have a gameObject attached</remarks>
     */
    public static List<string> getParents(GameObject obj) {
        var parentList = new List<string>();
        if (obj.name.Contains("Veggie")) {
            obj = obj.transform.parent.gameObject;
        } do {
            parentList.Add(obj.name);
            obj = obj.transform.parent.gameObject;
        } while (obj.transform.parent != null);
        return parentList;
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
                case "Small" or "Carrot": {
                    break; //tryin to avoid default with basic plants
                } case "Beetroot": {
                    score++;
                    break;
                } case "Medium": {
                    score *= 2;
                    break;
                } case "Large": {
                    score *= 5;
                    break;
                } default: {
                    Debug.Log("Whoopy while trying to decide the score of the object of name " + parentList[0]);
                    break;
                }
            }
        } return score;
    }
}
