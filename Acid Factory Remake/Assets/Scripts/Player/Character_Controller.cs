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
        if (getMove() is not CanMove.Cant) {
            move();
        }
    }

    private void FixedUpdate() {
        if (getMove() is not CanMove.CantJump) {
            StopCoroutine(gravAmplifier(Vector3.zero));
            jump();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        switch (collision.gameObject.name) {
            case "Platform": {
                Move.updateMovement(CanMove.Freely);
                break;
            } case "DeathPlane": {
                failSafe();
                break;
            }
        }
    }

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
     * Giving the player an arch (hopefully)
     */
    private static IEnumerator flying() {
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
        } GameObject.Find("Player").GetComponent<Character_Controller>() /*This part here is needed to find the correct instance inside this static function*/
            .GetComponent<MonoBehaviour>().StartCoroutine(gravAmplifier(hop)); //idea here is to have the gravity work specifically when the player is not jumping 
    }
    private static IEnumerator gravAmplifier(Vector3 hop) {
        while (Move.getMove() is not CanMove.Freely) { //here the arch is kept at a downwards angle
            movePlayer(hop);
            yield return new WaitForFixedUpdate();
        } 
    }

    private static void movePlayer(Vector3 movement) {
        pBody.velocity = movement;
    }
}
