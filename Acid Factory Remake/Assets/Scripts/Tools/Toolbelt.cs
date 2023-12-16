using System.Collections;
using System.Collections.Generic;
using Script.Tools.ToolType;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Toolbelt : MonoBehaviour {
    public List<Equipment> belt;
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
        tool.GameObject().GetComponent<Collider>().enabled = false;
        if (checkForCorrectToolType(tool.name)) {
            if (canPickup || toolInHand == null) { //if the hand is empty of the hand just have been emptied
                if (toolInHand != null && !tool.name.Equals(toolInHand.gameObject.name)) {
                    throwToolFromHand();
                } addTool(tool);
            }
        } else {      //todo create a state machine that keeps track of the player's toolset and changes equipments when necessary
            if (!belt.Contains((Equipment)tool)) { //brainstorming: this could be done by having a sprite / 3D version of the object kept as an apallel like the player's shadow 
                belt.Add((Equipment)tool); tool.GameObject().transform.parent = Character_Controller.getPlayerBody().transform;
                if (tool.name is not "Vest") {  //then playing an animation of the player getting it on then toggling the mesh renderer of the object (or pane housing the sprite)
                    tool.GameObject().transform.localPosition = tool.name is "Helmet" ? new Vector3(0f, 0.7f, 0f) : new Vector3(0f, -0.5f, -0.4f);
                    return;
                } tool.GameObject().transform.localPosition = new Vector3(0f, 0f, -0.6f);
            }
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
     * <summary>Puts the tool in the player's hand after modifying it's flags to not interact with anything while in the player's hand</summary>
     * <param name="tool">The tool desired to be added</param>
     */
    private void addTool(Object tool) {
        if (tool.name.Contains("Flower")) {
            FlowerController.pullFlower(FlowerController.findFlower(tool.name));
        } toolInHand = tool.GetComponent<Tools>();
        transferToolState(false);
        toolInHand.transform.position = Character_Controller.getPlayerHand().position;
    }

    /**
     * <summary>Lets the tool kept in the hand go from the player's letting it fall down</summary>
     */
    private void throwToolFromHand() {
        if (toolInHand.GetComponent<Tools>().GetComponent<Rigidbody>() == null) {
            Debug.Log("Whoopy, tried to throw an item from hand that doesn't have a rigidbody");
            return;
        } transferToolState(true);
        StartCoroutine(pickupDelay());
    }

    /**
     * <summary>Changes the variables of a tool the player can have in its hand in the map.
     * <para>By defining the <see cref="whereTo"/> variable,
     * this function can make the given tool to be admitted into the player's hand or thrown to the ground</para></summary>
     * <param name="whereTo">True: to the ground,
     * <para>False: to the player's hand</para></param>
     */
    private void transferToolState(bool whereTo) {
        var handBody = toolInHand.GetComponent<Tools>().GetComponent<Rigidbody>();
        handBody.useGravity = whereTo;
        handBody.isKinematic = !whereTo;
        handBody.GetComponent<Collider>().enabled = handBody.useGravity;
        if (handBody.useGravity) {
            toolInHand.transform.parent = GameObject.Find("Tools").transform;
        } else {
            toolInHand.transform.parent = Character_Controller.getPlayerHand();
        } if (handBody.useGravity) {
            removeTool(toolInHand.gameObject.name);
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
    private void removeTool(string toolName) {
        if (toolInHand != null && toolName.Equals(toolInHand.gameObject.name)) {
            toolInHand.transform.parent = null;
            toolInHand = null;
        } else {
            checkIfToolIsObtained(toolName, out var tool);
            belt.Remove((Equipment)tool);
            Destroy(tool.GameObject());
        } 
    }

    /**
     * <summary>Stops the player from being able to pickup an another tool immediately</summary>
     * <remarks>The stop is 1 seconds long</remarks>
     */
    private IEnumerator pickupDelay() {
        yield return new WaitForSeconds(1);
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
            case "Helmet" or "Vest" or "Slipper": { //if an instance of the equipment is found in the overworld, it is destroyed and the code acts like a new instance is purchased
                ((Equipment)(tool = gObj.GetComponent<Equipment>())).initTool(gObj.name);
                break;
            } case "StopWatch": {
                if (!collided) {
                    //todo potentially create spawner for any item for the rest of the places down except flower
                }  ((StopWatch)(tool = gObj.GetComponent<StopWatch>())).prepStopWatch();
                break;
            } case "Dynamite": {
                ((Dynamite)(tool = gObj.GetComponent<Dynamite>())).prepDynamite(tool.GameObject());
                break;
            } case "Umbrella": {
                ((Umbrella)(tool = gObj.GetComponent<Umbrella>())).prepUmbrella();
                break; 
            } case "Flower": {
                tool = FlowerController.findFlower(gObj.name);
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
        if (toolInHand != null && toolName.Equals(toolInHand.gameObject.name)) {
            foundTool = toolInHand;
            return true;
        } foreach (var tool in belt) {
            if (tool.name.Equals(toolName)) {
                foundTool = tool;
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
