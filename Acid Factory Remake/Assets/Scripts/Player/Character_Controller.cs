using System;
using System.Collections;
using UnityEngine;
using static Move;

public class Character_Controller : MonoBehaviour
{
    private const double MoveVel = 20;
    private static Rigidbody pBody;
    public static int hitPoints; //usually 2 todo implement a hitpoint system later

    /**
     * <summary>Initialized the variables unique to the player</summary>
     */
    private static void init() {
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
        if (Move.getMove() is not CanMove.Cant) {
            move();
        }
    }

    private void FixedUpdate() {
        if (getMove() is not CanMove.CantJump) {
            jump();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.name is "Platform") {
            Move.updateMovement(CanMove.Freely);
        }
    }

    /**
     * <summary><para>Evaluates the movement vector of the player</para>
     * Based on the keys supplied by the currently active gimmick.</summary>
     * <remarks>I wish I could implement this into a switch statement</remarks>
    */
    private static void move() {
        var vel = Vector3.zero;
        if (Input.GetKey(KeyCode.A)) { //left
            vel.x -= (float)MoveVel;
        } if (Input.GetKey(KeyCode.D)) { //right
            vel.x += (float)MoveVel;
        } if (Input.GetKey(KeyCode.W)) { //up
            vel.z += (float)MoveVel;
        } if (Input.GetKey(KeyCode.S)) { //down
            vel.z -= (float)MoveVel;
        } movePlayer(vel);
    }
    
    /**
     * <summary><para>Gives the player a jump force upwards</para>
     * if the player presses the desired key</summary>
    */
    private void jump() {
        var hop = new Vector3(pBody.velocity.x, 0, pBody.velocity.z);
        if (grounded()) {
            if (Input.GetKeyDown(KeyCode.Space)) { 
                Move.updateMovement(CanMove.CantJump);
                StartCoroutine(flying(hop));
            } 
        }
    }

    /**
     * Giving the player an arch (hopefully)
     */
    private static IEnumerator flying(Vector3 hop) {
        for (var i = 1; i < 4; i++) {
            hop.y += (float)MoveVel*2*i;//todo this isn't perfect
            yield return null;
            movePlayer(hop);
        }
        
        
        yield return new WaitForFixedUpdate();
        hop.y = -(float)MoveVel;
        while (Move.getMove() is not CanMove.Freely) { //todo this is a good downwards arch
            movePlayer(hop);
            yield return null;
        } 
    }
    
    private static void movePlayer(Vector3 movement) {
        pBody.velocity = movement;
    }

    /**
     * <summary><para>Evaluates if the player should be considered on the ground</para></summary>
     * <returns>true if the player is stationary, false otherwise</returns>
     * <remarks>Works remarkably but only on flat objects or slopes smaller than 60 degrees</remarks>
     */
    private static bool grounded() {
        return Math.Abs(Math.Round(pBody.velocity.y, 1)) < 0.01f; //checks if the player isn't flying in the air
    }
}
