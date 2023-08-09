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
    public static void pullVegetable(string name) {
        veggieBank[name] = false; //here I access the boolean of the dictionary by using the name as an index (veggieBank<string, bool>)
        var obj = GameObject.Find(name);
        obj.GetComponent<CapsuleCollider>().isTrigger = veggieBank[name];
        obj.SetActive(veggieBank[name]);
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
        var parent = veg.GetComponentInParent<GameObject>().GetComponentInParent<GameObject>()
            .GetComponentInParent<GameObject>(); //breakdown of the chain in an intended state: Veggie1 -> Small -> Carrot -> Vegetable
        return parent != null && parent.name.Contains("Vegetables");
    }
}
