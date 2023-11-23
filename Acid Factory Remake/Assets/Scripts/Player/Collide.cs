using System.Collections;
using Script.Tools.ToolType;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Character_Controller;
using static Move;

public class Collide : MonoBehaviour {
    
    private static float priorYVel; //todo !!! ADD THIS SCRIPT TO THE PLAYER

    public static void init() {
        priorYVel = 0f;
    }
    
    private void FixedUpdate() {
        if (getMove() is not CanMove.CantJump) {
            if (Input.GetKey(KeyCode.Space) && !isAscending()) {
                Move.updateMovement(CanMove.CantJump);
                StartCoroutine(flying());
            } 
        } else {
            updatePriorVel();
        }
    }
    
    /**
     * <summary>Attempts to reset the player's state into "should fall"</summary>
     */
    private void OnCollisionExit(Collision other) {
        if (checkForDistance()) {
            if (getParentName(other.gameObject.transform) is "Platforms" or "Walls" && Move.getMove() is not Move.CanMove.CantJump) {
                Move.updateMovement(Move.CanMove.CantJump); 
                StartCoroutine(falling(getPlayerBody().velocity));
            } StartCoroutine(ShadowController.findPlatform()); 
        }
    }
    
    /**
     * <summary>handles the collision behind interactible objects</summary>
     */
    private void OnTriggerEnter(Collider other) {
        if (other.name.Contains("Flower")) {
            if (FlowerController.haveFlowerBeenPulled(other.name)) {
                FlowerController.addFlower(FlowerController.findFlower(other.name));
            }
        } if (VegetablePull.validateVegetable(other.gameObject)) {
            OnTriggerStay(other);
        }
    }

    /**
     * <remarks>It is assumed that an object with a trigger flag set is a kind of tool</remarks>
     */
    private void OnTriggerStay(Collider other) {
        if (checkForActionButton()) {
            if (VegetablePull.validateVegetable(other.gameObject)) {
                processVegetables(other);
            } else if (Toolbelt.checkForCorrectToolType(other.name)) {
                Toolbelt.getBelt().putToolInHand(other); //only flowers have triggers assigned to them yet
            } 
        }
    }

    /**
     * <summary>handles the logic behind solid object collision</summary>
     */
    private void OnCollisionEnter(Collision collision) {
        var cObj = collision.gameObject;
        if (!VegetablePull.validateVegetable(cObj)) {
            processCollision(getParentName(cObj.transform));
        } StopCoroutine(ShadowController.findPlatform());
    }
    
    /**
     * <summary>Initiates the logic behind the vegetable pulls
     * <para>A beetroot is twice as valuable</para></summary>
     * <remarks>This might break (or get exploited) if the vegetable's name is not correctly parsed</remarks>
     */
    private static void processVegetables(Collider veggie) {
        UI.updatePoints(VegetablePull.getProfileOfVeggie(getParentName(veggie.gameObject)));
        VegetablePull.pullVegetable(veggie);
    }

    /**
     * <summary>Processes platforms and Death-Planes</summary>
     * <remarks>If the object is something uncounted for, the player is hurt and respawned at center coordinates</remarks>
     */
    private void processCollision(string name) {
        switch (name) {
            case "Platforms": {
                processPlatforms(); 
                break;
            } case "Walls" or "Player": { //in case I need to add stuff in here
                break;
            } case "Anvils": { //updates the flag
                if (!processAnvil()) {
                    goto case "Platforms"; //this will make the anvil act like a platform
                } break;
            } case "DeathPane" /*when !invincibility *//*this here adds a simple extra condition to the case to match*/: {
                hurtPlayer();
                failSafe();
                break;
            } case "Tools": {
                processTools(name); //tools without triggers include helmet, vest, slippers ...
                break;
            } default: {
                Debug.Log("Doin some uncoded things for " + name + "s");
                break;
            } 
        }
    }

    private void processPlatforms() {
        /*Debug.Log("Current y vel: "+ pBody.velocity.y + ", prior y vel: " + priorYVel);*/
        if (getMove() is CanMove.CantJump) { //if the player is flying
            slapPlayerDown();
        } if (priorYVel < 0f) {
            Move.updateMovement(CanMove.Freely);
        }
    }

    private static bool processAnvil() {
        if (AnvilManager.isFlyin()) {
            if (Toolbelt.getBelt().checkIfToolIsObtained("Helmet", out var foundTool)) { //the helmet stops the player from getting hurt
                Debug.Log("Helmet used!");
                Toolbelt.getBelt().checkForDurability((Equipment)foundTool);
            } else {
                hurtPlayer();
            } AnvilManager.disableAnvil();
            return true;
        } return false;
    }

    /**
     * <summary></summary>
     */
    private static void processTools(string name) {
        var desiredTool = Toolbelt.createTool(name);
        if (desiredTool != null) {
            Toolbelt.getBelt().putToolInHand(desiredTool);
            Debug.Log(name + " added!");
        } 
    }
    
    /**
     * <summary>Handles the player's y velocity for the whole duration of the player's flight</summary>
     * <remarks>it has a decent arch</remarks>
     */
    private IEnumerator flying() {
        var hop = new Vector3(getPlayerBody().velocity.x, (float)(MoveVel / 2*6f), getPlayerBody().velocity.z);
        movePlayer(hop);
        return falling(hop);
    }

    /**
     * <summary>Calculates the falling velocity during the downward arch before reaching terminal (desired) velocity</summary>
     */
    private IEnumerator falling(Vector3 hop) {
        while (hop.y > -50f) { //here the arch goes from ~50 to -30
            hop.y -= (float)MoveVel;
            yield return new WaitForSeconds(0.1f); //this is needed with the time being optimal
            movePlayer(hop);
        } var hand = Toolbelt.getBelt().toolInHand;
        if (hand == null || !((Umbrella)hand).isOpen) { //if the player's hand is not empty the umbrella isn't open
            StartCoroutine(gravAmplifier(hop)); //idea here is to have the gravity work specifically when the player is not jumping 
        } 
    }
    
    /**
     * <summary>Amplifies the gravity applied onto the player in a logarithmic arch (capped)</summary>
     * <remarks>I have no idea how this works while falling...</remarks>
     */
    private static IEnumerator gravAmplifier(Vector3 hop) {
        while (Move.getMove() is not CanMove.Freely) { //here the arch is kept at a downwards angle
            movePlayer(hop);
            yield return new WaitForFixedUpdate();
        }
    }
    
    private void slapPlayerDown() {
        StopAllCoroutines();
        var pBody = getPlayerBody();
        getPlayerBody().velocity = new Vector3(pBody.velocity.x, -5f, pBody.velocity.z);
    }
    
    /**
     * <summary>Saves the previously calculated velocity calculated in the last physics update</summary>
     */
    private static void updatePriorVel() {
        priorYVel = getPlayerBody().velocity.y;
    }
    
    /**
     * <summary>Warps the player back in bounds if the player ever manages to fall under the map</summary>
     */
    private static void failSafe() {
        getPlayerBody().position = new Vector3(0f, 3f, 0f);
    }
}