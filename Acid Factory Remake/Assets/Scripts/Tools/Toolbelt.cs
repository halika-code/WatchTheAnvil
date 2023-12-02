using System.Collections;
using System.Collections.Generic;
using Script.Tools.ToolType;
using Unity.VisualScripting;
using UnityEngine;
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

    private bool canPickup;
    
    private void Start() {
        belt = new List<Equipment>();
        canPickup = true;
    }

    /**
     * <summary>Attempts to place the tool in the player's hand
     * <para>If the tool is found to be safety equipment, the tool is placed on the player instead</para></summary>
     * <remarks>This function is equipped to handle flowers properly as well</remarks>
     */
    public void putToolInHand(Object tool) {
        tool.GameObject().GetComponent<Collider>().enabled = false; //todo for some reason the trigger flag refuses to get disabled fast enough
        if (checkForCorrectToolType(tool.name)) {
            if (canPickup || toolInHand == null) { //if the hand is empty of the hand just have been emptied
                if (toolInHand != null && !tool.name.Equals(toolInHand.gameObject.name)) {
                    throwToolFromHand();
                } addTool(tool);
            }
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
            tool.GetComponent<Rigidbody>().isKinematic = true;
        } toolInHand = tool.GetComponent<Tools>();
        toolInHand.gameObject.transform.SetParent(Character_Controller.getPlayerHand());
        toolInHand.gameObject.GetComponent<Collider>().enabled = false;
        toolInHand.transform.position = Character_Controller.getPlayerHand().position;
    }

    /**
     * <summary>Gives the object a velo</summary>
     */
    private void throwToolFromHand() {
        var handBody = toolInHand.GetComponent<Tools>().GetComponent<Rigidbody>();
        if (handBody == null) {
            Debug.Log("Whoopy, tried to throw away an object with no rigidbody");
            return;
        } handBody.useGravity = true;
        handBody.isKinematic = false;
        removeTool(toolInHand.gameObject.name);
        StartCoroutine(pickupDelay());
        handBody.GetComponent<Collider>().enabled = true;
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
    private void removeTool(string toolName) {
        if (toolName.Equals(toolInHand.gameObject.name)) {
            toolInHand.transform.parent = null;
            toolInHand = null;
        } else {
            checkIfToolIsObtained(toolName, out var tool);
            belt.Remove((Equipment)tool);
        } 
    }

    private IEnumerator pickupDelay() {
        yield return new WaitForSeconds(3);
        canPickup = true;
    }

    /**
     * <summary>Attempts to fetches an existing instance of a tool</summary>
     * <returns>The newly added tool, or null if the tool is found to exists already</returns>
     * <remarks>If a flower is attempted to be created, the instance of the flower found will be returned</remarks>
     */
    public Object getTool(Object gObj, bool collided) {
        if (getBelt().checkIfToolIsObtained(gObj.name, out var tool)) {
            return null;
        } switch (gObj.name) {
            case "Helmet" or "Vest" or "Slippers": {
                tool = !collided ? getBelt().gameObject.AddComponent<Equipment>() : 
                    gObj.GetComponent<Equipment>(); 
                ((Equipment)tool).initTool(gObj.name); //either get the tool-in-ground or create a new tool
                putToolInHand(gObj);
                break;
            } case "StopWatch": {
                if (!collided) {
                    //todo potentially create spawner for any item for the rest of the places down except flower
                } else {
                    tool = gObj.GetComponent<StopWatch>();
                } ((StopWatch)tool).prepStopWatch();
                putToolInHand(gObj);
                break;
            } case "Dynamite": {
                tool = gObj.GetComponent<Dynamite>();
                ((Dynamite)tool).prepDynamite(((Dynamite)tool).gameObject);
                putToolInHand(gObj);
                break;
            } case "Umbrella": {
                tool = gObj.GetComponent<Umbrella>();
                ((Umbrella)tool).prepUmbrella();
                putToolInHand(gObj);
                break; 
            } case "Flower": {
                tool = FlowerController.findFlower(gObj.name);
                putToolInHand(gObj);
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
        if (toolInHand != null && toolName.Equals(toolInHand.gameObject.name)) {
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
