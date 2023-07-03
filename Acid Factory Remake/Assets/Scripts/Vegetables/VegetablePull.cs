using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VegetablePull : MonoBehaviour {

    /**
     * <summary></summary>
     * <remarks>Incomplete yet</remarks>
     */
    public static void pullVegetable(string name) {
        
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
     * <summary>Checks if the vegetable's name contains a letter that can only be in vegetables used in this game</summary>
     */
    public static bool validateVegetable(string name) {
        return name.Contains('e') || name.Contains('C'); //Beetroot or Carrot
    }
}
