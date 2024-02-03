using System.Collections.Generic;
using System.Threading.Tasks;
using Script.Tools.ToolType;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

/**
 * <name>ToolBelt</name>
 * <summary>A collection of functions responsive to any general purpose mechanisms for the tools.
 * This include tools processing, storage and management</summary>
 * <author>Gyula Attila Kovacs (gak8)</author>
 * <date>21/10/2023</date>
 */
public class Toolbelt : MonoBehaviour {
    public List<Equipment> belt;
    public static Toolbelt toolBelt;
    /**
     * <summary>
     * <para>A variable that can only hold 1 item at a time (except flowers, those are kept in a bouquet)</para>
     * <para>Possible items held includes: Flowers (bouquet), Dynamite, Magnet, Umbrella, StopWatch</para>
     * <para>Items that should never be kept in hand: Any type of Equipment (as in Helmet, Vest or Slippers), Carrots, Beetroot</para>
     * </summary>
     */ 
    public Tools toolInHand;

    private bool canPickup;
    
    private void OnEnable() {
        belt = new List<Equipment>();
        canPickup = true;
        toolBelt = this;
    }

    /**
     * <summary>Attempts to place the tool in the player's hand
     * <para>If the tool is found to be safety equipment, the tool is placed on the player instead</para></summary>
     * <remarks>This function is equipped to handle flowers properly as well</remarks>
     */
    public void handleTool(Object tool) {
        if (checkForCorrectToolType(tool.name)) {
            putToolInHand((Tools)tool);
        } else {
            putToolOnBelt((Equipment)tool);
        }
    }

    /**
     * <summary>Puts tool into an object that is child of the player to be used as a tool</summary>
     */
    private void putToolInHand(Object tool) {
        if (canPickup || toolInHand == null) { //if the hand is empty of the hand just have been emptied
            canPickup = false;
            if (toolInHand != null) {
                throwToolFromHand(); 
                runItemCoolDown();
            } addTool(tool);
            canPickup = true;
        }
    }
    
    /**
     * <summary>Puts item on the player as armor</summary>
     */
    private void putToolOnBelt(Equipment tool) {
        if (!belt.Contains(tool)) { //brainstorming: this could be done by having a sprite / 3D version of the object kept as an apallel like the player's shadow 
            tool.GetComponent<Collider>().enabled = false; //needs to be disabled otherwise the player's hitbox gets massively elongated
            belt.Add(tool); 
            tool.gameObject.transform.parent = Character_Controller.getPlayerBody().transform;
            if (tool.name is not "Vest") {  //then playing an animation of the player getting it on then toggling the mesh renderer of the object (or pane housing the sprite)
                tool.GameObject().transform.localPosition = tool.name is "Helmet" ? new Vector3(0f, 0.7f, 0f) : new Vector3(0f, -0.5f, -0.4f);
                return;
            } tool.GameObject().transform.localPosition = new Vector3(0f, 0f, -0.6f);
        }
    }

    public void fetchItem() {
        switch (toolInHand.gameObject.name) {
            case "Dynamite": {
                ((Dynamite)toolInHand).useItem();
                break;
            } case "Umbrella": {
                ((Umbrella)toolInHand).useItem();
                break;
            } case "StopWatch": {
                ((StopWatch)toolInHand).useItem();
                break;
            }
        } runItemCoolDown();
    }
    
    private static async void runItemCoolDown() {
        InputController.itemCoolDown = true;
        await Task.Delay(500);
        InputController.itemCoolDown = false;
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
     * <summary>Swaps the tool's state from being in the player's hand
     * <para>to on the ground with all the necessary flag changes</para></summary>
     */
    private void throwToolFromHand() { 
        Debug.Log("Throwing " + toolInHand.name); //todo then have an OnCollisionEnter that swaps the trigger back with the gravity turned off
        if (toolInHand.name.Contains("StopWatch") && ((StopWatch)toolInHand).stopWatchInUse) {
            ((StopWatch)toolInHand).useItem();
        } else if (toolInHand.name.Contains("Umbrella") && ((Umbrella)toolInHand).checkIfOpen()) { //if the umbrella is in the player's hand AND is open
            ((Umbrella)toolInHand).useItem();
        } transferToolState(true);
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
        handBody.GetComponent<Collider>().enabled = whereTo;
        toolInHand.transform.parent = handBody.useGravity ? GameObject.Find("Tools").transform : Character_Controller.getPlayerHand(); 
        if (whereTo) {
            removeTool(toolInHand.gameObject.name);
        }
    }
    
    public void checkForDurability(Equipment tool) {
        Equipment.useItem(tool, out var isBroken);
        if (isBroken) {
            toolInHand.transform.parent = null;
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
            toolInHand.transform.position = Character_Controller.getPlayerBody().position; //putting the given item to the feet of the player
            toolInHand = null;
        } else {
            checkForTool(toolName, out var tool);
            belt.Remove((Equipment)tool);
            Destroy(tool.GameObject());
        } 
    }

    /**
     * <summary>Attempts to fetches an existing instance of a tool after initialization</summary>
     * <param name="gObj">The instance of the tool's component. Any type will work as long as a valid tool can be found using the <see cref="checkIfToolIsObtained"/></param>
     * <param name="collided">A flag denoting if the player have purchased the tool or found it in the field</param>
     * <returns>The newly added tool, or null if the tool is found to exists already</returns>
     * <remarks>When expanding with a new tool, keep in mind: the gObj is used to find the instance of the tool,
     * for the switch-statement the found instance named tool must be used</remarks>
     */
    public Object findTool(Object gObj, bool collided) {
        if (checkForTool(gObj.name, out var tool)) {
            return null;
        } switch (gObj.name) {
            case "Helmet" or "Vest" or "Slipper": { //if an instance of the equipment is found in the overworld, it is destroyed and the code acts like a new instance is purchased
                ((Equipment)(tool = gObj.GetComponent<Equipment>())).initTool(gObj.name);
                break;
            } case "StopWatch": {
                if (((StopWatch)(tool = gObj.GetComponent<StopWatch>())).text == null) { //if the text is Unity null
                    ((StopWatch)tool).prepStopWatch();
                } break;
            } case "Dynamite": {
                if (((Dynamite)(tool = gObj.GetComponent<Dynamite>())).getText() == null) {
                    ((Dynamite)tool).prepDynamite();
                } break;
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
    public bool checkForTool(string toolName, out Object foundTool) {
        foundTool = null;
        if (toolInHand /*if it is not NULL*/ && toolName.Equals(toolInHand.gameObject.name)) {
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
        return toolBelt;
    }

    /**
     * <summary>Checks the toolInHand against null</summary>
     * <returns>True if is NOT null, False if it is</returns>
     */
    public static bool checkHand() {
        return getBelt().toolInHand;
    }
}
