using System.Collections.Generic;
using UnityEngine;

public class VegetablePull : MonoBehaviour {

    private static Dictionary<string, bool> veggieBank; //keeps the veggie's names in order to keep them accessable, note, might be obsolete

    private void Start() {
        prepareVeggies(); 
    }

    /**
     * <summary> The vegetable names are all shuffled into list
     * </summary>
     * <remarks>This works if there is a grouping empty for the vegetables</remarks>
     */
    private static void prepareVeggies() {
        var veggies = GameObject.Find("Vegetables");
        veggieBank = new Dictionary<string, bool>(veggies.transform.childCount);
        for (var i = 0; i < veggieBank.Count; i++) {
            veggieBank.Add(veggies.GetComponentsInChildren<CapsuleCollider>()[i].name, true);
        }
    }

    /**
     * <summary>Updates the veggie-bank then sets it's trigger property.
     * Disables the vegetable as a final step</summary>
     * <remarks></remarks>
     */
    public static void pullVegetable(Collider veggie) {
        veggieBank[veggie.name] = false; //here I access the boolean of the dictionary by using the name as an index (veggieBank<string, bool>)
        veggie.GetComponent<CapsuleCollider>().isTrigger = veggieBank[veggie.name];
        veggie.gameObject.SetActive(veggieBank[veggie.name]);
    }

    /**
     * <summary>Resets the visibility of the vegetables that needs it</summary>
     * <remarks>A quick null-check is also done here, just in case</remarks>
     */
    public static void resetVegetables() {
        if (veggieBank == null) { //failsafe
            Debug.Log("Whoopy, attempted to reset vegetables with an empty bank");
            return;
        } foreach (var veggies in veggieBank) {
            if (!veggies.Value) {
                var veg = GameObject.Find(veggies.Key);
                veg.GetComponent<CapsuleCollider>().isTrigger = false;
                veg.SetActive(true);
            }
        }
    }

    /**
     * <summary> Attempts to get the value of a desired vegetable the player have picked up
     * <returns>An integer of 4-2-1 based on the flag embedded inside the name of the vegetable </returns>
     * <remarks>Example: BigCarrot</remarks></summary>
     */
    public static int getPoints(string name) {
        if (name.Contains('g')) { //either Big or Large
            return 5;
        } return name.Contains('M') ? 2 : 1; //Medium
    }
    
    /**
     * <summary>Checks if the vegetable has a valid parent</summary>
     */
    public static bool validateVegetable(GameObject veg) {
        var parent = Character_Controller.getParentName(veg); //breakdown of the chain in an intended state: Veggie1 -> Small -> Carrot -> Vegetable
        return parent != null && parent.name.Contains("Vegetables");
    }

    public static List<string> getParents(GameObject obj) {
        var parentList = new List<string>();
        do {
            parentList.Add(obj.name);
            obj = obj.transform.parent.gameObject;
        } while (obj.transform.parent != null);
        return parentList;
    }

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
                    Debug.Log("Whoopy while trying to decide the score of the object of name" + parentList[0]);
                    break;
                }
            }
        } return score;
    }
}
