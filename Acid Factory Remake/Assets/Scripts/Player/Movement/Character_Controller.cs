using System;
using System.Collections.Generic;
using Script.Tools.ToolType;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Move;
using static VelocityManipulation;

/**
 * <date>17/06/2023</date>
 * <author>ciglos mikCiglos aka cigi migi</author>
 * <summary>A traditional character controller script, this class is supposed to
 * move the player in all cardinal directions alongside extra logic</summary>
 * <remarks>This is also one of the center-most scripts in the project</remarks>
 */
public class Character_Controller : MonoBehaviour {
    
    public const double MoveVel = 22D;                  //todo break the velocity manipulation logic from here into a new script called SpeedHandling
    public const double DampeningCoefficient = 1.7D;
    public static Vector3 flyingVector;
    protected static Rigidbody pBody;
    private static Transform pHand;


    //note: within functions if I write a function that has an out <variable> keyword, I can RETURN more than one variable
    
    /**
     * <summary>Initialized the variables unique to the player</summary>
     */
    private static void init() {
        setPlayerBody();
        Physics.gravity = new Vector3(0, -30f);
        pHand = GameObject.Find("Hand").transform;
        flyingVector = Vector3.zero;
    }

    /**
     * <summary>Prepares the player's RigidBody, moving it to the burrow if one can be found</summary>
     */
    private static void setPlayerBody() {
        pBody = GameObject.Find("Player").GetComponent<Rigidbody>();
        pBody.freezeRotation = true;
        if (GameObject.Find("Enter") != null) {
            var burrowPos = GameObject.Find("Enter").gameObject.transform.position;
            pBody.position = new Vector3(burrowPos.x, burrowPos.y + 2f, burrowPos.z);
        }
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
            movePlayer(InputController.checkForButtonPress(calculateVelocity()));
        } if (InputController.checkForItemUse()) { //if the player wants to use the item and the cooldown flag is clear
            Toolbelt.getBelt().fetchItem();
        } //Debug.Log("canMove is " + Move.getMove() + ", Player's prior y vel is: " + priorYVel); //note, just comment this debug out when not in use
    }

    
    private void FixedUpdate() {
        //Debug.Log(GravAmplifier.isAscending? "flyin" : "On the ground");
        //Debug.Log(getMove() is CanMove.CantJump? "StateFly" : "StateGround");
        if (getMove() is not CanMove.CantJump && InputController.checkForJump()) { //wall-jump: the Move state machine can only have 1 state, can be locked out IF I check for isAscending as well
            if (Toolbelt.getBelt().checkForTool("Umbrella", out _)) {
                if (checkAgainstUmbrella()) { //should be a normal jump-arch until 0 then fall slowly 
                    jump(desiredSpeedCap: 0f); //note this assigns a value in here
                    return;
                } 
            } jump();
        } velocityDecay(); //needs to be here to have a fixed rate of slowdown
    }
    
   
    /**
     * <summary>Gives the player a starting velocity then based on the desired speed cap, will apply increased gravity until the cap is reached / passed</summary>
     * <param name="speedUp">The starting Y velocity, by default is <see cref="MoveVel"/> * 3.2 cast into a float</param>
     * <param name="desiredSpeedCap">The desired terminal velocity, by default is set to -70f</param>
     */
    public static void jump(float speedUp = (float)MoveVel * 3.2f, float desiredSpeedCap = -70f) {
        var pVel = pBody.velocity;
        GravAmplifier.gravity.falling(new Vector3(pVel.x, speedUp, pVel.z), desiredSpeedCap);
        updateMovement(CanMove.CantJump);
        incrementXSpeedDown(wait: 3f); //called from here to start the async function
    }

    /**
     * <summary>Calculates if the player is standing on the closest solid object</summary>
     * <returns>True if the player's distance is too far from the raycast's length</returns>
     */
    public static bool checkForDistance() {
        if (ShadowController.findColPoint(out var hit)) {
            return hit.distance > (pBody.transform.localScale.y/2)+1f; //the idea here is with the localScale I can get the height of the player from this data
        } return false;
    }
    
    /**
     * <summary>An overloaded version from <see cref="checkForDistance()"/> where the rayCastHit is given</summary>
     * <returns>True if the player's distance is too far from the raycast's length</returns>
     */
    public static bool checkForDistance(RaycastHit hit) {
        return hit.distance > (pBody.transform.localScale.y/2)+1f;
    }
    
    /**
     * <summary>Attempts to remove one health-point. If that fails, the player is killed</summary>
     */
    public static void hurtPlayer() {
        if (UI.getHealthPoints() is not 1) {
            UI.updateHealthPoint(-1);
        } else {
            killPlayer();
        }
    }

    /**
     * <summary>A simple kill-switch that reloads the game</summary>
     */
    public static void killPlayer() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /**
     * <summary>Fetches the name of the root parent of the gameObject</summary>
     * <param name="obj">The transform of the original object</param>
     * <returns>The name of the root parent</returns>
     * <remarks>Will find the name no matter how deep the object is in the hierarchy</remarks>
     */
    public static string getParentName(GameObject obj) {
        return getParentName(obj.transform)[^1];
    }
    
    /**
     * <summary>Assembles every parent for the given object into a list up to the root object (not inclusive)</summary>
     * <param name="obj">The object that should be examined</param>
     * <returns>The list (of type string) of the "family tree"</returns>
     * <remarks>Works with objects that doesn't "normally" have a gameObject attached</remarks>
     */
    public static List<string> getParentName(Transform obj) {
        var parentList = new List<string>();
        do {
            parentList.Add(obj.name);
        }  while ((obj = obj.transform.parent));
        return parentList;
    }

    /**
     * <summary>Applies the desired vector of movement onto the player</summary>
     */
    private static void movePlayer(Vector3 movement) {
        //Debug.Log(movement.x is 26 ? "gravAmplified movement" : "normal movement");
        pBody.velocity = movement;
        ShadowController.moveShadow(pBody.transform.position);
    }

    /**
     * <summary>Alternative player movement function where only one angle is modified in the player's current velocity</summary>
     * <param name="velocity">Desired vector given to the player</param>
     * <param name="index">The desired index where the velocity will be changed.
     * <para>By default this parameter is 1 (denoting the y axis)</para></param>
     * <remarks>If the index falls outside the bounds of a traditional Vector3, this function will exit early</remarks>
     */
    public static void movePlayer(float velocity, int index = 1) {
        if (index <= 2) { //if the 
            Vector3 pVel;
            var desiredVel = pVel = pBody.velocity;
            for (var i = 0; i < 2; i++) {
                desiredVel[i] = index.Equals(i) ? velocity : pVel[i];
            } movePlayer(desiredVel);
        } else {
            Debug.Log("Whoopy, tried giving " + velocity + " to the " + index + "th element in the player's movement vector");
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /**
     * <summary>Finds the rigidbody attached to the player</summary>
     * <remarks>The component will always be found</remarks>
     */
    public static Rigidbody getPlayerBody() {
        if (pBody) { //pbody is checked against it being null
            return pBody;
        } init();
        return getPlayerBody();
    }

    /**
     * <summary>Attempts to find the player's hand</summary>
     * <returns>The empty used to store the objects designated to be "in the player's hand", the player's body's transform otherwise</returns>
     */
    public static Transform getPlayerHand() {
        return pHand;
    }
}