using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static Move;

public class Character_Controller : MonoBehaviour {
    private const double MoveVel = 20;
    private static Rigidbody pBody;
    public static int hitPoints; //usually 2 todo implement a hitpoint system later
    private static bool canPull;

    /**
     * <summary>Initialized the variables unique to the player</summary>
     */
    private static void init() {
        canPull = false;
        pBody = GameObject.Find("Player").GetComponent<Rigidbody>();
        pBody.freezeRotation = true;
        Physics.gravity = new Vector3(0, -30f); //I should use gravity to enforce the player and objects to stick to the surface, slow but detectable when player is falling
        hitPoints = 2;
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
        if (VegetablePull.validateVegetable(cObj) && canPull) { //processes a vegetable
            processVegetables(cObj);
        } else {
            if (getMove() is CanMove.CantJump) {
                StopCoroutine(gravAmplifier(Vector3.zero));
            } processCollision(getParentName(cObj).name);
        }
    }

    /**
     * <summary>Prepares the desired characters for platforms the player can reach for further processing </summary>
     * <summary>Used to find the start and end of the name of the platform</summary>
     */
    private static char[] prepChars(string cParent) {
        if (cParent.Contains('V')) {
            return new[] { 'V', 'e' };
        } return cParent.Contains('P') ? new[] { 'P', 'm' } : new [] { 'D', 'e' };
    }

    /**
     * <summary>Initiates the logic behind the vegetable pulls
     * <para>A beetroot is twice as valuable</para></summary>
     * <remarks>This might break (or get exploited) if the vegetable's name is not correctly parsed</remarks>
     */
    private static void processVegetables(GameObject veggie) {
        UI.updatePoints(VegetablePull.getProfileOfVeggie(VegetablePull.getParents(veggie)));
        VegetablePull.pullVegetable(veggie.name);
    }

    /**
     * <summary>Processes platforms and Death-Planes</summary>
     * <remarks>If the object is something uncounted for, this just falls through as if nothing happened</remarks>
     */
    private static void processCollision(string name) {
        switch (name) {
            case "Platform": {
                Move.updateMovement(CanMove.Freely);
                break;
            }
            case "Wall": { //in case I need to add stuff in here
                break;
            }
            default: {
                failSafe();
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
    private void checkForVeggiePulls() {
        if (Input.GetKey(KeyCode.E) && !canPull) {
            StartCoroutine(doPullVeggies());
        }
    }

    /**
     * <summary>Allows the player to pull vegetables for 1 second each time</summary>
     */
    private static IEnumerator doPullVeggies() {
        canPull = true;
        yield return new WaitForSeconds(1f);
        canPull = false;
    }

    /**
     * <summary>Handles the player's y velocity for the whole duration of the player's flight</summary>
     * <remarks>it has a decent arch</remarks>
     */
    private IEnumerator flying() {
        var hop = new Vector3(pBody.velocity.x, 0, pBody.velocity.z);
        for (var i = 1; i < 2; i++) {//upward flying
            hop.y += (float)((float)5/(1.8*i) * MoveVel); //this is somewhat of an arbitrary value
            yield return new WaitForFixedUpdate();
            movePlayer(hop);
        } yield return new WaitForSeconds(0.3f); //gives it a bit of a time-out so the upward arch can actually play out
        while (hop.y > -30f) { //here the arch goes from ~50 to -30
            hop.y -= (float)(0.9f*MoveVel);
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
        ShadowController.movePlayer(new Vector3(movement.x, 0f, movement.z));
    }

    public static Rigidbody getPlayerBody() {
        return pBody;
    }

    public static bool isMoving() { //idea here is find a threshold that can tell me if the player is moving. check what velocity correlates with 0
        return Math.Round(pBody.velocity.x, 2) > 0.05 || Math.Round(pBody.velocity.y) > 0.05 || Math.Round(pBody.velocity.z) > 0.05;
    }
}
