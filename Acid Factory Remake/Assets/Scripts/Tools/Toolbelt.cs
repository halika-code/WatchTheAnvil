using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AnvilManager;

public class Toolbelt : MonoBehaviour {
    private List<Tools> belt;
    public Tools toolInHand;
    public bool stopWatchInUse;
    
    private void Start() {
        belt = new List<Tools> { new () };
        stopWatchInUse = false;
    }

    /**
     * <summary>After checking if the player have already has a tool of the same type
     * <para>(can't have multiple helmets), the new tool will be added to the list</para></summary>
     * <param name="toolName">The name of the tool desired to be added</param>
     */
    public void addTool(string toolName) {
        if (toolName is "Flower" || !checkIfToolExists(toolName, out _) /*The underscore is used as I don't have a use for the out variable*/) { //if the tool is flower (can be stacked infinitely) or is a unique tool
            var t = new Tools(toolName);
            if (toolName is "Dynamite" or "StopWatch") {
                toolInHand = t;
            } else {
                belt.Add(t);
            }
        }
    }

    /**
     * <summary>Attempts to find the tool the player have used
     * <para>If the tool is found it will be removed, an error message will be thrown otherwise</para></summary>
     * <param name="toolName">The name of the tool desired to be deleted</param>
     */
    public void removeTool(string toolName) {
        if (checkIfToolExists(toolName, out var foundTool)) {
            if (foundTool.name.Equals(toolInHand.name)) {
                toolInHand = null;
            } else {
                belt.Remove(foundTool);
            }
        }
        else {
            Debug.Log("Whoopy while attempting to remove an item I couldn't find with a name of " + toolName);
        }
    }

    /**
     * <summary>Attempts to find an instance of a tool found inside the tool-belt or in the player's hand</summary>
     * <param name="toolName">The requested tool</param>
     * <param name="foundTool">The instance of the tool found
     * <para>Will be null, if nothing is found</para></param>
     * <returns>True, if an instance is found, false otherwise</returns>
     */
    public bool checkIfToolExists(string toolName, out Tools foundTool) {
        foundTool = null;
        if (toolName.Equals(toolInHand.name)) {
            return true;
        } foreach (var tool in belt) {
            if ((foundTool = tool).name.Equals(toolName)) {
                return true;
            }
        } return false;
    }
    
    /**
     * <summary>Find which state the anvil is in, saves that state and attempts to continue execution where it was left off</summary>
     * <param name="stopWatch">The instance of the watch</param>
     * <param name="shouldContinue">The flag that flips intended function of this script, unfreezing the anvil and terminating early</param>
     */
    public IEnumerator runStopWatch(Tools stopWatch, bool shouldContinue) {
        var wait1Sec = new WaitForSeconds(1);
        var targetScript = getTargetScript();
        if (!shouldContinue) {
            StartCoroutine(targetScript);
            stopWatchInUse = false;
            yield break;
        } StopCoroutine(targetScript);
        stopWatchInUse = true;
        while (stopWatch.lifeSpanTimer is not 0) { //actual wait
            yield return wait1Sec;
            stopWatch.lifeSpanTimer--;
        } StartCoroutine(targetScript);
        stopWatchInUse = false;
    }

    /**
     * <summary>A sister-script to <see cref="runStopWatch"/>, finds the script that is currently running concerning the anvil</summary>
     * <returns>The instance of that script in IEnumerator form</returns>
     */
    private IEnumerator getTargetScript() {
        if (currentAnvil.aTimer is not 0) { //if the anvil is falling
            currentAnvil.freezeAnvil();
            return currentAnvil.dropAnvil();
        } if (waitTimer is 0) { //if the anvil have spawned
            return helpRunTimer(currentAnvil, currentAnvil.aTimer is 3 ? 3 : 0); //deciding if after the freeze, the anvil should target or not
        } return startInitialWait(); //if the anvil is just waiting to spawn
    }
}
