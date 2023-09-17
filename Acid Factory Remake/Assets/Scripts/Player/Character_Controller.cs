using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Move;

public class Character_Controller : MonoBehaviour {
    private const double MoveVel = 20;
    private static Rigidbody pBody;

    //todo note: within functions if I write a function that has an out <variable> keyword, I can RETURN more than one value
    
    /**
     * <summary>Initialized the variables unique to the player</summary>
     */
    private static void init() {
        pBody = GameObject.Find("Player").GetComponent<Rigidbody>();
        pBody.freezeRotation = true;
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

    private void OnCollisionEnter(Collision collision) {
        var cObj = collision.gameObject;
        if (!VegetablePull.validateVegetable(cObj)) {
            if (getMove() is CanMove.CantJump) {
                StopCoroutine(gravAmplifier(Vector3.zero));
            } processCollision(getParentName(cObj).name);
        } else {
            OnCollisionStay(collision);
        } StopCoroutine(ShadowController.findPlatform(pBody));
    }
    
    /**
     * <summary>Listens for the player to pull a vegetable</summary>
     */
    private void OnCollisionStay(Collision collisionInfo) {
        if (checkForVeggiePulls() && VegetablePull.validateVegetable(collisionInfo.gameObject)) {
            processVegetables(collisionInfo);
        }
    }

    /**
     * <summary>Attempts to reset the player's state into "should fall"</summary>
     */
    private void OnCollisionExit(Collision other) {
        if (getParentName(other.gameObject).name is "Platforms" or "Walls" && getMove() is not CanMove.CantJump) {
            updateMovement(CanMove.CantJump);
            StartCoroutine(falling(getPlayerBody().velocity));
        } StartCoroutine(ShadowController.findPlatform(pBody)); //todo add a coroutine to constantly findPlatform until collision is entered
    }

    /**
     * <summary>Initiates the logic behind the vegetable pulls
     * <para>A beetroot is twice as valuable</para></summary>
     * <remarks>This might break (or get exploited) if the vegetable's name is not correctly parsed</remarks>
     */
    private static void processVegetables(Collision veggie) {
        UI.updatePoints(VegetablePull.getProfileOfVeggie(VegetablePull.getParents(veggie.gameObject)));
        VegetablePull.pullVegetable(veggie.collider);
    }

    /**
     * <summary>Processes platforms and Death-Planes</summary>
     * <remarks>If the object is something uncounted for, the player is hurt and respawned at center coordinates</remarks>
     */
    private static void processCollision(string name) {
        switch (name) {
            case "Platforms": {
                Move.updateMovement(CanMove.Freely);
                break;
            } case "Walls" or "Shadow" or "Player": { //in case I need to add stuff in here
                break;
            } case "DeathPane": {
                failSafe();
                hurtPlayer();
                break;
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
        while (hop.y > -30f) { //here the arch goes from ~50 to -30
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
    private static void hurtPlayer() {
        if (UI.getHealthPoints() is not 1) {
            UI.updateHealthPoint(-1);
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
     * <returns>The root parent (including empties)</returns>
     * <remarks>Will find the name no matter how deep the object is in the hierarchy</remarks>
     */
    public static GameObject getParentName(GameObject obj) {
        while (obj.transform.parent != null) { //todo here is where I'm checking for the object's parent
            obj = obj.transform.parent.gameObject;
        } return obj;
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
