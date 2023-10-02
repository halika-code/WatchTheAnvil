using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static VegetableVisibility;
using static Character_Controller;

public class VegStateController : MonoBehaviour {
    private static List<Rigidbody> cBodyCollective;
    private static List<VegState> vegStateCollective;
    private static Rigidbody pBody;
    
    private void Start() {
        cBodyCollective = gameObject.GetComponentsInChildren<Rigidbody>().ToList();
        if (cBodyCollective == null) { //idea here is if there is no vegetables under this parent script, the script should disable
            Destroy(this);
        } init();
    }

    /**
     * <summary>Initializes all the crucial variables</summary>
     */
    private static void init() {
        pBody = getPlayerBody();
        vegStateCollective = new List<VegState>();
        for (var i = 0; i < cBodyCollective.Count; i++) {
            vegStateCollective.Add(VegState.Hidden);
        }
    }
    
    private void FixedUpdate() {
        for (var i = 0; i < cBodyCollective.Count-1; i++) {
            checkForPlayerDistance(cBodyCollective[i], vegStateCollective[i]);
        }
    }
    
    /**
     * <summary>Handles down what to do when the player is in the aura of a vegetable</summary>
     * <param name="cBody">A link to the vegetable in question</param>
     * <param name="state">The corresponding state to the vegetable</param>
     * <remarks>It is assumed here that the given state parameter corresponds to the object linked to the cBody parameter</remarks>
     */
    private static void checkForPlayerDistance(Component cBody, VegState state) {
        if (pClose(pInBorder(new []{cBody.transform.position.x, cBody.transform.position.z}, 
                new []{pBody.transform.position.x, pBody.transform.position.z}))) { 
            if (state is VegState.Hidden) { //the idea here is if the player is close, the player will be inside a border
               Debug.Log("The player is close to a carrot named " + cBody.name);
                vegStateCollective[getIndexOfVeg(cBody)] = VegState.Visible; 
            }
        } else if (state is VegState.Visible) { 
            Debug.Log("The carrot hides away " + cBody.name);
            vegStateCollective[getIndexOfVeg(cBody)] = VegState.Hidden;
        }
    }

    /**
     * <summary>Attempts to find the index of the vegetable in question</summary>
     * <param name="cObj">The vegetable in question</param>
     * <returns>The found index inside the cBodyCollective</returns>
     * <remarks>If an exact match in the hierarchy wasn't found, the 1st index is returned</remarks>
     */
    private static int getIndexOfVeg(Component cObj) {
        for (var i = 0; i < cBodyCollective.Count; i++) {
            if (cObj.name.Equals(cBodyCollective[i].name)) { //preliminary check if the Veggie3 is a Veggie3
                if (getParentName(cObj.gameObject).Equals(getParentName(cBodyCollective[i].gameObject))) {
                    return i; //a more detailed check based on the hierarchy of the veggie, mainly to improve efficiency
                }
            }
        } return 0;
    }

    /**
     * <summary>Check if the player have entered the vicinity of a vegetable</summary>
     * <param name="cPos">The array of the positions for the vegetable to be inspected</param>
     * <param name="pPos">The array of the positions for the player to be inspected</param>
     * <returns>A flag for each side of the vegetable (not including the Y axis)
     * <para>true if it is close, false otherwise</para></returns>
     */
    private static List<bool> pInBorder(float[] cPos, float[] pPos) { //todo what it looks like, when the player gets close to the empty housing the carrots, this returns both true
        var border = new List<bool>();                              //todo far from the vegetables ... I need to have individual vegetables trigger
        for (var i = 0; i < cPos.Length; i++) {
            border.Add((Math.Abs(cPos[i]) - (Math.Abs(pPos[i]) + 2f)) < 5f); //the idea here is if the player is 3 meters in the vicinity 
        } return border;
    }

    /**
     * <summary>Decides if the player is in the complete boundary of a select vegetable</summary>
     * <param name="result">The return item from <see cref="pInBorder"/></param>
     * <returns>True, if both of the instances of the parameter is true
     * <para>If a false is found, may that be the first or last instance in the top parameter,
     * the return value will be locked to being false</para></returns>
     * <remarks>This function is setup to handle an infinite amount of axis</remarks>
     */
    private static bool pClose(List<bool> result) {
        var retVal = true;
        foreach (var borderFinding in result) {
            if (!borderFinding) {
                retVal = false;
            }
        } return retVal;
    }
}
