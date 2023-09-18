using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Move;

public class Character_Controller : MonoBehaviour {
    private const double MoveVel = 20;
    private static Rigidbody pBody;
    private static bool invincibility;

    //todo note: within functions if I write a function that has an out <variable> keyword, I can RETURN more than one value
    
    /**
     * <summary>Initialized the variables unique to the player</summary>
     */
    private static void init() {
        pBody = GameObject.Find("Player").GetComponent<Rigidbody>();
        pBody.freezeRotation = true;
        invincibility = false;
        Physics.gravity = new Vector3(0, -30f);
    }

    private void OnEnable() { //singleton pattern, just in case
        if (pBody != null) {
            Destroy(this); //destroys the instance of this script playing if a player with an initialized pBody component can be found (playing in a separate script)
        }
    }

    private void Start() {
        init();
    }

    // Update is called once per frame
    private void Update() {
        if (getMove() is not CanMove.Cant) {
            move();
            checkForVeggiePulls();
        }
    }

    private void FixedUpdate() {
        if (getMove() is not CanMove.CantJump) {
            jump();
        }
    }

    /**
     * <summary>handles the collision behind interactible objects</summary>
     */
    private void OnTriggerEnter(Collider other) {
        if (VegetablePull.validateVegetable(other.gameObject)) {
            OnTriggerStay(other);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (checkForVeggiePulls() && VegetablePull.validateVegetable(other.gameObject)) {
            processVegetables(other);
        }
    }

    /**
     * <summary>handles the logic behind solid object collision</summary>
     */
    private void OnCollisionEnter(Collision collision) {
        var cObj = collision.gameObject;
        if (!VegetablePull.validateVegetable(cObj)) {
            if (getMove() is CanMove.CantJump) {
                StopCoroutine(gravAmplifier(Vector3.zero));
            } processCollision(getParentName(cObj.transform));
        } StopCoroutine(ShadowController.findPlatform());
    }

    /**
     * <summary>Attempts to reset the player's state into "should fall"</summary>
     */
    private void OnCollisionExit(Collision other) {
        if (getParentName(other.gameObject.transform) is "Platforms" or "Walls" && getMove() is not CanMove.CantJump) {
            updateMovement(CanMove.CantJump);
            StartCoroutine(falling(getPlayerBody().velocity));
        } StartCoroutine(ShadowController.findPlatform()); 
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
                Move.updateMovement(CanMove.Freely);
                break;
            } case "Walls" or "Player": { //in case I need to add stuff in here
                break;
            } case "Anvils": {
                StartCoroutine(hurtPlayer());
                break;
            } case "DeathPane" /*when !invincibility *//*this here adds a simple extra condition to the case to match*/: {
                if (!invincibility) {
                    StartCoroutine(hurtPlayer());
                    failSafe();
                } break;
            } default: {
                Debug.Log("Doin some uncoded things for " + name + "s");
                break;
            } 
        }
    }

    /**
     * <summary>Warps the player back in bounds if the player ever manages to fall under the map</summary>
     */
    private static void failSafe() {
        pBody.position = new Vector3(0f, 3f, 0f);
    }

    /**
     * <summary><para>Evaluates the movement vector of the player</para>
     * Based on the keys supplied by the currently active gimmick.</summary>
     * <remarks>I wish I could implement this into a switch statement</remarks>
    */
    private static void move() {
        var vel = new Vector3(0f, pBody.velocity.y, 0f);
        if (Input.GetKey(KeyCode.A)) { //left
            vel.x -= (float)(MoveVel*1.5);
        } if (Input.GetKey(KeyCode.D)) { //right
            vel.x += (float)(MoveVel*1.5);
        } if (Input.GetKey(KeyCode.W)) { //up
            vel.z += (float)(MoveVel*1.5);
        } if (Input.GetKey(KeyCode.S)) { //down
            vel.z -= (float)(MoveVel*1.5);
        } movePlayer(vel);
    }
    
    /**
     * <summary><para>Gives the player a jump force upwards</para>
     * if the player presses the desired key</summary>
    */
    private void jump() {
        if (Input.GetKey(KeyCode.Space)) {
            Move.updateMovement(CanMove.CantJump);
            StartCoroutine(flying());
        } 
    }

    /**
     * <summary>Checks if the player have pressed the E key in a non-mashy style</summary>
     */
    private static bool checkForVeggiePulls() {
        return Input.GetKey(KeyCode.E);
    }

    /**
     * <summary>Handles the player's y velocity for the whole duration of the player's flight</summary>
     * <remarks>it has a decent arch</remarks>
     */
    private IEnumerator flying() {
        var hop = new Vector3(pBody.velocity.x, 0, pBody.velocity.z);
        hop.y += (float)(MoveVel / 2*6f); //x/(y*i)*MoveVel. x increases the height, y
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
        } StartCoroutine(gravAmplifier(hop)); //idea here is to have the gravity work specifically when the player is not jumping 
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

    /**
     * <summary>Attempts to remove one health-point. If that fails, the player is killed</summary>
     */
    private static IEnumerator hurtPlayer() {
        if (UI.getHealthPoints() is not 1) {
            invincibility = true;
            UI.updateHealthPoint(-1);
            yield return new WaitForSeconds(2f);
            invincibility = false;
        } else {
            killPlayer();
        }
    }

    /**
     * <summary>A simple kill-switch that reloads the game</summary>
     */
    private static void killPlayer() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /**
     * <summary>Fetches the name of the root parent of the gameObject</summary>
     * <param name="obj">The transform of the original object</param>
     * <returns>The name of the root parent</returns>
     * <remarks>Will find the name no matter how deep the object is in the hierarchy</remarks>
     */
    public static string getParentName(Transform obj) {
        return getParentName(obj.gameObject)[^1];
    }
    
    /**
     * <summary>Assembles every parent for the given object into a list up to the root object (not inclusive)</summary>
     * <param name="obj">The object that should be examined</param>
     * <returns>The list (of type string) of the "family tree"</returns>
     * <remarks>Works with objects that doesn't "normally" have a gameObject attached</remarks>
     */
    private static List<string> getParentName(GameObject obj) {
        var parentList = new List<string>();
        if (obj.name.Contains("Veggie")) {
            obj = obj.transform.parent.gameObject;
        } do {
            parentList.Add(obj.name);
            obj = obj.transform.parent.gameObject;
        } while (obj.transform.parent != null);
        return parentList;
    }

    /**
     * <summary>Applies the desired vector of movement onto the player</summary>
     */
    private static void movePlayer(Vector3 movement) {
        pBody.velocity = movement;
        ShadowController.moveShadow(pBody.transform.position);
    }

    public static Rigidbody getPlayerBody() {
        return pBody;
    }

    public static bool isMoving() { //idea here is find a threshold that can tell me if the player is moving. check what velocity correlates with 0
        return Math.Round(pBody.velocity.x, 2) > 0.05 || Math.Round(pBody.velocity.y) > 0.05 || Math.Round(pBody.velocity.z) > 0.05;
    }
}
