using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.Tools.ToolType;
using Unity.VisualScripting;
using UnityEngine;
using static AnvilManager;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Toolbelt : MonoBehaviour {
    private List<Equipment> belt;
    /**
     * <summary>
     * <para>A variable that can only hold 1 item at a time (except flowers, those are kept in a bouquet)</para>
     * <para>Possible items held includes: Flowers (bouquet), Dynamite, Magnet, Umbrella, StopWatch</para>
     * <para>Items that should never be kept in hand: Any type of Equipment (as in Helmet, Vest or Slippers), Carrots, Beetroot</para>
     * </summary>
     */ 
    public Tools toolInHand;

    
    private void Start() {
        belt = new List<Equipment>();
    }

    /**
     * <summary>Attempts to place the tool in the player's hand
     * <para>If the tool is found to be safety equipment, the tool is placed on the player instead</para></summary>
     */
    public void putToolInHand(Object tool) {
        if (checkForCorrectToolType(tool.name)) {
            if (toolInHand != null) {
                throwToolFromHand();
            } addTool(tool);
        } else {
            belt.Add((Equipment)tool);
            Destroy(tool);//removes the tool from the field
        }
    }

    /**
     * <summary>Checks the given name against the set type of tools that can fit into the player's hand</summary>
     * <returns>true if the item's name is any of the following: Flower, Dynamite, Magnet, Umbrella, StopWatch
     * <para>false otherwise</para></returns>
     */
    public static bool checkForCorrectToolType(string name) {
        return name.Contains("Flower") || name.Contains("Dynamite") || name.Contains("Magnet") ||
               name.Contains("Umbrella") || name.Contains("StopWatch");
    }

    /**
     * <summary>After checking if the player have already has a tool of the same type
     * <para>(can't have multiple helmets), the new tool will be added to the list</para></summary>
     * <param name="tool">The tool desired to be added</param>
     */
    private void addTool(Object tool) {
        if (tool.name.Contains("Flower")) {
            FlowerController.pullFlower(FlowerController.findFlower(tool.name));
        } toolInHand = (Tools)tool;
        var handTrans = toolInHand.gameObject.transform;
        toolInHand.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        handTrans.SetParent(Character_Controller.getPlayerHand());
    }

    /**
     * <summary>Gives the object a velo</summary>
     */
    public void throwToolFromHand() {
        var handBody = toolInHand.gameObject.GetComponent<Rigidbody>();
        if (handBody == null) {
            Debug.Log("Whoopy, tried to throw away an object with no rigidbody");
        } handBody.useGravity = true;
        handBody.transform.position = new Vector3(Random.Range(-3f, 3f), 5f, Random.Range(-3f, 3f));
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
     * <summary>Attempts to fetches an existing instance of a tool</summary>
     * <returns>The newly added tool, or null if the tool is found to exists already</returns>
     * <remarks>If a flower is attempted to be created, the instance of the flower found will be returned</remarks>
     */
    public static Object createTool(string toolName) {
        var gObj = GameObject.Find(toolName);
        if (getBelt().checkIfToolIsObtained(toolName, out var tool)) {
            return null;
        } switch (toolName) {
            case "Helmet" or "Vest" or "Slippers": {
                tool = gObj == null ? getBelt().gameObject.AddComponent<Equipment>() : 
                    gObj.GetComponent<Equipment>(); ((Equipment)tool).initTool(toolName); //either get the tool-in-ground or create a new tool
                break;
            } case "StopWatch": {
                if (gObj == null) {
                    //todo create code that makes a brand new stopwatch, do the same for the rest
                } else {
                    tool = gObj.GetComponent<StopWatch>();
                } ((StopWatch)tool).prepStopWatch();
                break;
            } case "Dynamite": {
                tool = gObj.GetComponent<Dynamite>();
                ((Dynamite)tool).prepDynamite(((Dynamite)tool).gameObject);
                break;
            } case "Umbrella": {
                tool = gObj.GetComponent<Umbrella>();
                ((Umbrella)tool).prepUmbrella();
                break; 
            } case "Flower": {
                tool = FlowerController.findFlower(toolName);
                break;
            }
        } return tool;
    }

    /**
     * <summary>Removes any number found in the name of the objects</summary>
     * <returns>The purified number</returns>
     * <remarks>Will return the name itself if no number is in the name</remarks>
     */
    private static string refineObjectName(string name) {
        var index = 1;
        while (int.TryParse(name[^index].ToString(), out _)) { //the idea here is the int.TryParse should return true as long as there is a number in the name
            index++;                                            //the ^index will reach farther back for as long as only numbers are reached
        } return name.Substring(0, name.Length-index); //then the same logic is applied but in the reverse
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
        if (toolInHand != null && toolName.Equals(toolInHand.name)) {
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
