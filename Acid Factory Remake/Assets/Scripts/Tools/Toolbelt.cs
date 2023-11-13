using System;
using System.Collections;
using System.Collections.Generic;
using Script.Tools.ToolType;
using UnityEngine;
using static AnvilManager;
using Object = UnityEngine.Object;

public class Toolbelt : MonoBehaviour {
    private List<Tools> belt;
    public Tools toolInHand;

    
    private void Start() {
        belt = new List<Tools>();
    }

    /**
     * <summary>After checking if the player have already has a tool of the same type
     * <para>(can't have multiple helmets), the new tool will be added to the list</para></summary>
     * <param name="toolName">The name of the tool desired to be added</param>
     */
    public void addTool(string toolName) {
        if (toolName is "Flower" || !checkIfToolExists(toolName, out _) /*The underscore is used as I don't have a use for the out variable*/) { //if the tool is flower (can be stacked infinitely) or is a unique tool
            Equipment t = new(toolName);
            if (toolName is "Dynamite" or "StopWatch" or "Flower") {
                toolInHand = t;
                t.gameObject.transform.parent = Character_Controller.getPlayerHand(); //todo make this have the bouquet
            } else {
                belt.Add(t);
            }
        }
    }

    public void checkForDurability(Equipment tool) {
        Equipment.useItem(tool, out var isBroken);
        if (isBroken) {
            removeTool(tool.name);
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
                belt.Remove((Tools)foundTool);
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
    public bool checkIfToolExists(string toolName, out Object foundTool) {
        foundTool = null;
        if (toolName.Equals(toolInHand.name)) {
            foundTool = toolInHand;
            return true;
        } foreach (var tool in belt) {
            if ((foundTool = tool).name.Equals(toolName)) {
                return true;
            }
        } return false;
    }

    /**
     * <summary>Fetches the belt attached to the player</summary>
     */
    public static Toolbelt getBelt() {
        return Character_Controller.getPlayerBody().GetComponent<Toolbelt>();
    }
}
