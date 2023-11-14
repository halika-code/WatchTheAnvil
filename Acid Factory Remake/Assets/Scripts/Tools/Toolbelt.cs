using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.Tools.ToolType;
using Unity.VisualScripting;
using UnityEngine;
using static AnvilManager;
using Object = UnityEngine.Object;

public class Toolbelt : MonoBehaviour {
    private List<Equipment> belt;
    public Tools toolInHand;

    
    private void Start() {
        belt = new List<Equipment>();
    }

    /**
     * <summary>After checking if the player have already has a tool of the same type
     * <para>(can't have multiple helmets), the new tool will be added to the list</para></summary>
     * <param name="tool">The tool desired to be added</param>
     */
    public void addTool(Object tool) {
        if (tool.name.Contains("Flower") || tool.name.Contains("Dynamite") || tool.name.Contains("StopWatch")) {
            if (tool.name.Contains("Flower")) {
                FlowerController.addFlower(FlowerController.findFlower(tool.name));
            } toolInHand = (Tools)tool;
            toolInHand.gameObject.transform.parent = Character_Controller.getPlayerHand();
        } else {
            belt.Add((Equipment)tool);
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
        if (checkIfToolIsObtained(toolName, out var foundTool)) {
            if (foundTool.name.Equals(toolInHand.name)) {
                toolInHand = null;
            } else {
                belt.Remove((Equipment)foundTool);
            }
        }
        else {
            Debug.Log("Whoopy while attempting to remove an item I couldn't find with a name of " + toolName);
        }
    }

    /**
     * <summary>Attempts to create a new instance of the tool</summary>
     * <returns>The newly added tool, or null if the tool is found to exists already</returns>
     * <remarks>If a flower is attempted to be created, the instance of the flower found will be returned</remarks>
     */
    public static Object createTool(string toolName) {
        if (getBelt().checkIfToolIsObtained(toolName, out var tool)) {
            return null;
        } switch (toolName) {
            case "Helmet" or "Vest" or "Slippers": {
                tool = getBelt().gameObject.AddComponent<Equipment>();
                ((Equipment)tool).initTool(toolName);
                break;
            } case "StopWatch": {
                tool = GameObject.Find("StopWatch").GetComponent<StopWatch>();
                ((StopWatch)tool).prepStopWatch();
                break;
            } case "Dynamite": {
                tool = GameObject.Find("Dynamite").GetComponent<Dynamite>();
                ((Dynamite)tool).prepDynamite(((Dynamite)tool).gameObject);
                break;
            } case "Umbrella": {
                tool = GameObject.Find("Umbrella").GetComponent<Umbrella>();
                ((Umbrella)tool).prepUmbrella();
                break; 
            } case "Flower": {
                tool = FlowerController.findFlower(toolName);
                break;
            }
        } return tool;
    }

    /**
     * <summary>Attempts to find an instance of a tool found inside the tool-belt or in the player's hand</summary>
     * <param name="toolName">The requested tool</param>
     * <param name="foundTool">The instance of the tool found
     * <para>Will be null, if nothing is found</para></param>
     * <returns>True, if an instance is found, false otherwise</returns>
     */
    public bool checkIfToolIsObtained(string toolName, out Object foundTool) {
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
