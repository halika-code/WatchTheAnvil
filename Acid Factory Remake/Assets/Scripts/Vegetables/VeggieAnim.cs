using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RootVeg;
using static Character_Controller;
using static VegetableVisibility;

public static class VeggieAnim {

    private static Animator anim = GameObject.Find("LeafHolder").GetComponent<Animator>();
    /**
     * <summary>The RigidBody is the reference to the veggie being animated,
     * the int is it's state: 0: not running, 1: going up, 2: going down</summary>
     */
    private static readonly Dictionary<string, int> IsAnimRunning = new();
    private static readonly float[] RaiseHeights = {2.58f*2, 1.33f*2, 0.52f*2, 3.2f}; //big, medium, small veggie and flower

    /**
     * <summary>The idea here is to have the carrot pop out of the ground then abruptly stop
     * <para>The carrot is supposed to shoot up then slow slightly (and naturally) before abruptly stopping, all popped out or hidden</para></summary>
     */
    public static IEnumerator animateCarrot(Rigidbody targetCBody, VegState state) { //note: the opposite effect will be applied to the veggie compared to the state
        toggleAnimState(targetCBody, state, true);
        if (state is VegState.Hidden) {
            changeSprites(targetCBody, VegState.Hidden);
        } var y = targetCBody.position.y; //original y speed, needs to stay outside otherwise the veggies will ascend uncontrollably
        for (var i = state is VegState.Hidden ? 10 : 6 ; i > 0; i--) {
            //Debug.Log($"Looping {targetCBody.name}, loop is: {i}, y is: {y}, getModifier: {getModifier(targetCBody, state) / i}");
            targetCBody.position = new Vector3(targetCBody.position.x, y + (getModifier(targetCBody, state) / i), targetCBody.position.z);
            yield return new WaitForSeconds(0.008f);
        } stopAnim(targetCBody, state);
    }
    
    /**
     * <summary>Helper function of <see cref="animateCarrot"/>
     * <para>Winds down the animation for the given vegetable</para></summary>
     */
    private static void stopAnim(Component targetCBody, VegState state) {
        if (state is VegState.Visible) {
            changeSprites(targetCBody, VegState.Visible);
        } toggleAnimState(targetCBody, state, false);
        Enum.TryParse<VegState>((((int)state + 1) % 2).ToString(), out var vegState); //this here flips the state to the opposite (from hidden to visible for example)
        getRoot().updateCollective(getIndexOfVeg(targetCBody), vegState);
    }

    private static void toggleAnimState(Component vegBody, VegState state, bool isInit) {
        if (isInit) {
            IsAnimRunning.Add(getKey(vegBody), state is VegState.Hidden ? 1 : 2);
        } else {
            IsAnimRunning.Remove(getKey(vegBody));
        }
    }

    /**
     * <summary>Toggles the enabled status of the spriteHolders for a given veggie</summary>
     */
    private static void changeSprites(Component vegBody, VegState state) {
        var sprites = new List<Transform>();
        for (var i = 0; i < vegBody.transform.childCount; i++) { //a touch bit overkill
            sprites.Add(vegBody.transform.GetChild(i));
        } if (vegBody.name is not "Flower") {
            sprites[1].gameObject.SetActive(state is VegState.Visible); //toggles the bush to disable if needed 
            sprites[0].gameObject.SetActive(!sprites[1].gameObject.activeSelf); //spriteHolder
        } 
    }
    
    /**
     * <summary>Attempts to find the index of the vegetable in question</summary>
     * <param name="cObj">The vegetable in question</param>
     * <returns>The found index inside the cBodyCollective</returns>
     * <remarks>If an exact match in the hierarchy wasn't found, the 1st index is returned</remarks>
     */
    private static int getIndexOfVeg(Component cObj) {
        for (var i = 0; i < getRoot().getBodyCollective().Count; i++) {
            if (cObj.name.Equals(getRoot().getBodyCollective()[i].name)) { //preliminary check if the Veggie3 is a Veggie3
                if (checkIfEqual(getParentName(cObj.transform), getParentName(getRoot().getBodyCollective()[i].transform))) {
                    return i; //a more detailed check based on the hierarchy of the veggie, mainly to improve efficiency
                }
            }
        } return 0;
    }
    
    /**
     * <summary>Checks properly if two list-string's match</summary>
     */
    private static bool checkIfEqual(IReadOnlyList<string> cObj, IReadOnlyList<string> vObj) {
        var retVal = true;
        for (var i = 0; i < cObj.Count; i++) {
            if (!cObj[i].Equals(vObj[i])) {
                retVal = false;
            }
        } return retVal;
    }


    /**
     * <summary> 
     * </summary>
     */
    private static float getModifier(Component targetBody, VegState state) {
        int index;
        switch (getParentName(targetBody.transform)[1]) {
            case "Large": {
                index = 0;
                break;
            } case "Medium": {
                index = 1;
                break;
            } case "Small": {
                index = 2;
                break;
            } case "Flowers": {
                index = 3;
                break;
            } default: {
                goto case "Small";
            }
        } return state is VegState.Visible ? RaiseHeights[index] * -1 : RaiseHeights[index];
    }

    /**
     * <summary>A faster alternative to <see cref="getKey(Component)"/>, returns the 1st two letters given from the list</summary>
     * <remarks>The list supplied is expected to contain the hierarchy of a plant that is animatable
     * <para>It is also expected that the list has more than one entry</para></remarks>
     */
    private static string getKey(IReadOnlyList<string> cBodyList) {
        return new string(cBodyList[0] + cBodyList[1] + cBodyList[^1]);
    }

    /**
     * <summary>A quick way to make a key from the hierarchy of the given RigidBody</summary>
     * <returns>A key (that should be) unique to each plant that gets animated</returns>
     * <remarks>Slower than <see cref="getKey(System.Collections.Generic.IReadOnlyList{string})"/> but easy to call</remarks>
     */
    private static string getKey(Component cBody) {
        var list = getParentName(cBody.transform);
        list.Add(cBody.name);
        return getKey(list);
    }
    
    /**
     * <summary>Checks if a given vegetable is in the list of running vegetables</summary>
     * 
     */
    public static bool checkIfAnimIsRunning(Rigidbody key) {
        return IsAnimRunning.ContainsKey(getKey(key));
    }

    public static bool shouldStopAnim(Rigidbody key) {
        return checkIfAnimIsRunning(key);
    }
}