using System;
using System.Collections;
using Script.Tools.ToolType;
using UnityEngine;
using static Character_Controller;
using static Move;
using Object = UnityEngine.Object;
using Task = System.Threading.Tasks.Task;

/**
 * <date>27/10/2023</date>
 * <author>Gyula Attila Kovacs(gak8)</author>
 * <summary>A class that is responsible for the collision processing for the player.
 * This includes platform logic, item trigger handling and more</summary>
 */
public class Collide : MonoBehaviour {

    /**
     * <summary>Processes platforms, Death-Planes and Equipment type tools</summary>
     * <remarks>If the object is something uncounted for, the player is hurt and respawned at center coordinates</remarks>
     */
    private void processCollision(string parentName, GameObject obj) {
        switch (parentName) {
            case "Platforms": {
                processPlatforms(); 
                break;
            } case "Walls": { //in case I need to add stuff in here
                //processWalls(obj);
                break;
            } case "Anvils": { //updates the flag
                if (!processAnvil()) {
                    goto case "Platforms"; //this will make the anvil act like a platform
                } break;
            } case "DeathPane" /*when !invincibility *//*this here adds a simple extra condition to the case to match*/: {
                failSafe();
                hurtPlayer();
                break;
            } case "Tools": {
                processTools(obj); //tools without triggers include helmet, vest, slippers ...
                break;
            } case "Burrow": {
                if (obj.name is "Exit") {
                    LevelManager.advanceLevel();
                } else {
                    goto case "Platforms";
                } break;
            } default: {
                Debug.Log("Doin some uncoded things for " + parentName + "s");
                goto case "Platforms";
            } 
        }
    }

    #region PlatformCollision
    /**
     * <summary>handles the logic behind solid object collision</summary>
     */
    private void OnCollisionEnter(Collision collision) {
        var cObj = collision.gameObject;
        if (!VegetablePull.validateVegetable(cObj)) {
            processCollision(getParentName(cObj.gameObject), cObj.gameObject);
        } StopCoroutine(nameof(ShadowController.findPlatform)); //turns off ray-casting while the y coordinate will not change
    }
    
    /**
     * <summary>Attempts to reset the player's state into "should fall"</summary>
     */
    private void OnCollisionExit(Collision other) {
        if (checkForDistance()) {
            if (getParentName(other.gameObject) is "Platforms" or "Walls" && Move.getMove() is not Move.CanMove.CantJump) {
                Move.updateMovement(Move.CanMove.CantJump); 
                GravAmplifier.gravity.falling(getPlayerBody().velocity);
            } StartCoroutine(ShadowController.findPlatform()); 
        }
    }
    
    private void processPlatforms() {
        /*Debug.Log("Current y vel: "+ pBody.velocity.y + ", prior y vel: " + priorYVel);*/
        if (getMove() is CanMove.CantJump) { //if the player is flying
            GravAmplifier.gravity.slapPlayerDown();
        } if (priorYVel < 0f) {
            Move.updateMovement(CanMove.Freely);
        }
    }

    /**
     * <summary>Decides which direction the player gets locked from moving</summary>
     * <remarks>If the player is found to be grounded, the wall will be treated as a platform</remarks>
     */
    private void processWalls(GameObject obj) {
        for (var i = 1; i <= 4; i++) {  //todo the idea here is: the enum is a glorified integer array, so if I know what integer I want to update with, I should be able to use a for loop
            if (gameObject.transform.position[i] > obj.transform.position[i]) { //todo player pos bigger than object: coming from the left towards the right
                Enum.TryParse<CanMove>(i.ToString(), out var restriction); //todo note: this might not work
                Move.updateMovement(restriction); //todo update this to reflect on up-down AND to test what position the up-down is supposed to be (not the jump)
            }
        }
        
    }
    
    #endregion

    #region ToolCollision
    /** 
     * <summary>Attempts to pick-up item from the floor that are pickup-able</summary>
     * <remarks>Added an override that jumps straight to OnTriggerStay instead of processing logic</remarks>
     */
    private void OnTriggerEnter(Collider other) {
        /*
        if (Toolbelt.checkForCorrectToolType(other.name) || getParentName(other.transform) is "Tools") {
            var flow = FlowerController.findFlower(other.name);
            if (flow == null || !flow.gameObject.GetComponent<Collider>().enabled && flow.havePulled) { //if the item have not been acquired OR is a flower and have been pulled (need to check against the collider being turned)
                Toolbelt.getBelt().putToolInHand(Toolbelt.getBelt().getTool(other, true));
            } 
        }*/ //removed function: ensures the player will not have annoying moments of unintentionally juggling items
        OnTriggerStay(other);
    } 

    /**
     * <summary>Handles items that aren't supposed to be vacuumed up immediately</summary>
     * <remarks>It is assumed that an object with a trigger flag set is a kind of tool</remarks>
     */
    private void OnTriggerStay(Collider other) {
        if (InputController.checkForActionButton()) {
            if (VegetablePull.validateVegetable(other.gameObject)) {
                processVegetables(other);
            } else if (Toolbelt.checkForCorrectToolType(other.name)) {
                processTools(other.gameObject);
            } 
        }
    }
    
    #endregion
    
    /**
     * <summary>Initiates the logic behind the vegetable pulls
     * <para>A beetroot is twice as valuable</para></summary>
     * <remarks>This might break (or get exploited) if the vegetable's name is not correctly parsed</remarks>
     */
    private static void processVegetables(Collider veggie) {
        UI.updatePoints(VegetablePull.getProfileOfVeggie(getParentName(veggie.transform)));
        VegetablePull.pullVegetable(veggie);
    }

    /**
     * <summary>Processes collision with an anvil.
     * <para>If the anvil is in the air a check is performed for a helmet.
     * If one is found said helmet takes a durability-hit</para>
     * Otherwise the player takes a single point of damage</summary>
     * <remarks>If the anvil is stationary on the ground, it will be treated as a platform</remarks>
     * <returns>True if the anvil have hit the player in a flying state
     * <para>False otherwise</para></returns>
     */
    private static bool processAnvil() {
        if (AnvilManager.isFlyin()) {
            if (Toolbelt.getBelt().checkForTool("Helmet", out var foundTool)) { //the helmet stops the player from getting hurt
                Toolbelt.getBelt().checkForDurability((Equipment)foundTool);
            } else {
                hurtPlayer();
            } AnvilManager.disableAnvil();
            return true;
        } return false;
    }

    /**
     * <summary></summary>
     * <remarks>It is assumed that an item exists in the field for this function to trigger</remarks>
     */
    private static void processTools(Object obj) {
        var tool = Toolbelt.getBelt().findTool(obj, true); //problem here, tool gets set as null
        if (tool != null) {
            Toolbelt.getBelt().handleTool(tool);
        } else {
            Debug.Log("Whoopy, tried to process " + obj.name + " as a tool");
        }
    }
    

    
    /**
     * <summary>Warps the player back in bounds if the player ever manages to fall under the map</summary>
     */
    private static void failSafe() {
        getPlayerBody().MovePosition(new Vector3(0f, 3f, 0f));
    }
}