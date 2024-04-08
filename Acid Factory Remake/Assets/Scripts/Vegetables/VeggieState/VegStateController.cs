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
        getRoot().init(gameObject.GetComponentsInChildren<Rigidbody>(), out var terminate); //as in, feed in the data to the lists
        if (terminate) {
            Destroy(this);
        }
    }

    private void FixedUpdate() {
        for (var i = 0; i < getRoot().getBodyCollective().Count; i++) {
            checkForPlayerDistance(getRoot().getBodyCollective()[i], getRoot().getVegStates()[i]);
        }
    }
    
    /**
     * <summary>Prepares the animation of a given plant</summary>
     * <param name="cBody">A link to the plant in question</param>
     * <param name="state">The corresponding state to the plant</param>
     * <remarks>It is assumed here that the given state parameter corresponds to the object linked to the cBody parameter</remarks>
     * note: the idea with this if tree is if the vegetable selected for animation is
     * in a wrong state given the current state of the environment, leave early.
     * This way only the vegetable that tries to get animated in an intended environment gets animated
     */
    public void checkForPlayerDistance(Rigidbody cBody, VegState state) {
        if (!VeggieAnim.checkIfAnimIsRunning(cBody)) { //if an animation is playing don't do anything
            if (checkIfPlayerIsInBorder(new[] { cBody.transform.position.x, cBody.transform.position.z }, new[] { pBody.transform.position.x, pBody.transform.position.z }, 20)) {
                if (state is VegState.Visible) { //if the veggie is inside the border BUT is visible
                    return;
                } 
            } else if (state is VegState.Hidden) { //if the player is outside the activation border AND the veggie is hidden
                return;
            } StartCoroutine(VeggieAnim.animateCarrot(cBody, state)); //this statement is only reached when the veggie AND the environment is in the proper state
        }
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

    // ReSharper disable Unity.PerformanceAnalysis note: this function is rarely called in VeggieAnim.runStuckCountdown in a faulty instance
    public static VegStateController getController() {
        return GameObject.Find("Vegetables").GetComponent<VegStateController>();
    }
}
