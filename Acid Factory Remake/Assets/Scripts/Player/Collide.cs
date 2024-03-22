using System;
using System.Collections;
using Script.Tools.ToolType;
using Unity.VisualScripting;
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
    private void processCollision(string parentName, Collision obj) {
        switch (parentName) {
            case "Platforms": {
                if (Math.Floor(Math.Abs(obj.impulse[1])) is 0) { //normally 0 if grounded
                    goto case "Walls";
                } processPlatforms(); 
                break;
            } case "Walls": { //in case I need to add stuff in here
                processWalls(obj);
                break;
            } case "Foliage": {
                goto case "Platforms";
            } case "Anvils": { //updates the flag
                if (!processAnvil()) {
                    goto case "Platforms"; //this will make the anvil act like a platform
                } break;
            } case "DeathPane" /*when !invincibility *//*this here adds a simple extra condition to the case to match*/: {
                failSafe();
                hurtPlayer();
                break;
            } case "Tools": {
                processTools(obj.gameObject); //tools without triggers include helmet, vest, slippers ...
                break;
            } case "Burrow": {
                if (obj.gameObject.name is "Exit") {
                    LevelManager.getLevelLoader().advanceLevel();
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
            if (!VegetablePull.validateVegetable(collision.gameObject)) {
                processCollision(getParentName(collision.gameObject), collision);
            } 
        }
        
        /**
         * <summary>Attempts to reset the player's state into "should fall"</summary>
         */
        private void OnCollisionExit(Collision other) {
            if (checkForDistance()) { //if the player have left the ground without jumping
                if (!GravAmplifier.isAscending) {
                    if (getParentName(other.gameObject) is "Platforms" or "Walls") {
                        var pBody = getPlayerBody().velocity;
                        InputController.toggleToJumpingState(); 
                        GravAmplifier.gravity.falling(new Vector3(pBody.x, -10f, pBody.z)); 
                    } 
                } else {
                    updateMovement(CanMove.Freely);
                } StartCoroutine(ShadowController.followPlayer()); //this must have been disabled
            } else {
                updateMovement(CanMove.Freely); //case when the player touches the wall affectionately then breaks up with it
            }
        }
        
        private static void processPlatforms() {
            GravAmplifier.isAscending = false;
            InputController.updateButtonPress(-1);
        }

        /**
         * <summary>Decides which direction the player gets locked from moving</summary>
         * <remarks>If the player is found to be grounded, the wall will be treated as a platform</remarks>
         */
        private void processWalls(Collision obj) {
            //Collision.impulse is used here since it is a nice big number that is reliable
            for (var i = 0; i < 3; i+=2) { //designed to run twice
                if (Math.Abs(obj.contacts[0].normal[i]) is not 0) { //IF this is true, we have the side the player collides with
                    Enum.TryParse<CanMove>(obj.contacts[0].normal[i] > 0 ? (1 + i).ToString() : (2 + i).ToString(), out var restriction);
                    Move.updateMovement(restriction); //note: by design, the restriction will always be between 1-4
                    VelocityManipulation.stopPlayerVelocity(i);
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

        private void OnTriggerExit(Collider other) {
            if (InputController.itemCoolDown) { //this re-enables the player to pick-up the tool dropped to the floor
                InputController.itemCoolDown = false;
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
        if (tool != null) { //if the tool found is not null AND the player isn't attempting to spam-pickup the same tool
            if (!InputController.itemCoolDown) {
                Toolbelt.getBelt().handleTool(tool);
            }
        } else {
            Debug.Log("Whoopy while trying to process " + obj.name + " as a tool");
        }
    }
    
    /**
     * <summary>Warps the player back in bounds if the player ever manages to fall under the map</summary>
     */
    private static void failSafe() {
        getPlayerBody().MovePosition(new Vector3(0f, 3f, 0f));
    }
}