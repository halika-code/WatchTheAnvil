using System;
using System.Collections.Generic;
using UnityEngine;
using static VegetableVisibility;
using static Character_Controller;
using static RootVeg;

public class VegStateController : MonoBehaviour {
    private static Rigidbody pBody;

    private void Start() {
        pBody = getPlayerBody();
        init(false); //as in initialise the lists
        init(gameObject.GetComponentsInChildren<Rigidbody>(), out var terminate); //as in, feed in the data to the lists
        if (terminate) {
            Destroy(this);
        }
    }

    private void FixedUpdate() {
        for (var i = 0; i < getBodyCollective().Count; i++) {
            checkForPlayerDistance(getBodyCollective()[i], getVegStates()[i]);
        }
    }
    
    /**
     * <summary>Handles down what to do when the player is in the aura of a vegetable</summary>
     * <param name="cBody">A link to the vegetable in question</param>
     * <param name="state">The corresponding state to the vegetable</param>
     * <remarks>It is assumed here that the given state parameter corresponds to the object linked to the cBody parameter</remarks>
     */
    private void checkForPlayerDistance(Rigidbody cBody, VegState state) {
        if (checkIfPlayerIsInBorder(new []{cBody.transform.position.x, cBody.transform.position.z}, 
                new []{pBody.transform.position.x, pBody.transform.position.z}, 20)) { 
            if (state is VegState.Hidden) { //the idea here is if the player is close, the player will be inside a border
               StartCoroutine(VeggieAnim.animateCarrot(cBody, state));
               updateCollective(getIndexOfVeg(cBody), VegState.Visible);
            } 
        } else if (state is VegState.Visible) { 
            StartCoroutine(VeggieAnim.animateCarrot(cBody, state));
            updateCollective(getIndexOfVeg(cBody), VegState.Hidden);
        }
    }

    /**
     * <summary>Attempts to find the index of the vegetable in question</summary>
     * <param name="cObj">The vegetable in question</param>
     * <returns>The found index inside the cBodyCollective</returns>
     * <remarks>If an exact match in the hierarchy wasn't found, the 1st index is returned</remarks>
     */
    private static int getIndexOfVeg(Component cObj) {
        for (var i = 0; i < getBodyCollective().Count; i++) {
            if (cObj.name.Equals(getBodyCollective()[i].name)) { //preliminary check if the Veggie3 is a Veggie3
                if (checkIfEqual(getParentName(cObj.gameObject), getParentName(getBodyCollective()[i].gameObject))) {
                    return i; //a more detailed check based on the hierarchy of the veggie, mainly to improve efficiency
                }
            }
        } return 0;
    }

    /**
     * <summary>Checks properly if two list-string's match</summary>
     */
    private static bool checkIfEqual(List<string> cObj, List<string> vObj) {
        var retVal = true;
        for (var i = 0; i < cObj.Count; i++) {
            if (!cObj[i].Equals(vObj[i])) {
                retVal = false;
            }
        } return retVal;
    }

    /**
     * <summary>Check if the player have entered the vicinity of a vegetable</summary>
     * <param name="cPos">The array of the positions for the vegetable to be inspected</param>
     * <param name="pPos">The array of the positions for the player to be inspected</param>
     * <param name="borderLenght">The radius of the border (from the center)</param>
     * <returns>A flag for each side of the vegetable (not including the Y axis)
     * <para>true if it is close, false otherwise</para></returns>
     */
    private static List<bool> pInBorder(float[] cPos, float[] pPos, int borderLenght) {
        var border = new List<bool>();
        for (var i = 0; i < cPos.Length; i++) {
            border.Add(Math.Abs(cPos[i] - pPos[i]) < borderLenght); //the idea here is if the player is 3 meters in the vicinity 
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

    /**
     * <summary>Checks if the player is close to a border</summary>
     * <remarks>This is a shorthand using <see cref="pClose"/> and <see cref="pInBorder"/> functions</remarks>
     */
    public static bool checkIfPlayerIsInBorder(float[] objPos, float[] pPos, int borderLength) {
        return pClose(pInBorder(objPos, pPos, borderLength));
    }
}
