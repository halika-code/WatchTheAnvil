using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Move;
using static VelocityManipulation;
using static JumpController;

/**
 * <date>17/06/2023</date>
 * <author>ciglos mikCiglos aka cigi migi</author>
 * <summary>A traditional character controller script, this class is supposed to
 * move the player in all cardinal directions alongside extra logic</summary>
 * <remarks>This is also one of the center-most scripts in the project</remarks>
 */
public class Character_Controller : MonoBehaviour {
    
    public const double MoveVel = 25D; 
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
    }

    /**
     * <summary>Prepares the player's RigidBody, moving it to the burrow if one can be found</summary>
     */
    private static void setPlayerBody() {
        pBody = GameObject.Find("Player").GetComponent<Rigidbody>();
        pBody.freezeRotation = true;
        if (GameObject.Find("Enter")) {
            var burrowPos = GameObject.Find("Enter").gameObject.transform.position;
            pBody.position = new Vector3(burrowPos.x, burrowPos.y + 4f, burrowPos.z);
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
            movePlayer(InputController.checkForButtonPress());
        } if (InputController.checkForItemUse()) { //if the player wants to use the item
            Toolbelt.getBelt().fetchItem();
        } //Debug.Log("canMove is " + Move.getMove() + ", Player's prior y vel is: " + priorYVel); //note, just comment this debug out when not in use
    }

    
    private void FixedUpdate() {
        if (InputController.checkForJump()) {
            if (Toolbelt.getBelt().checkForTool("Umbrella", out _)) {
                if (checkAgainstUmbrella()) { //should be a normal jump-arch until 0 then fall slowly 
                    jump(desiredSpeedCap: -10f); 
                    return; //this needs to be here to not have the player frozen in place while having the umbrella in hand (but closed)
                } 
            } jump();
            StartCoroutine(ShadowController.followPlayer());
        } velocityDecay(); //needs to be here to have a fixed rate of slowdown
    }
    
    /**
     * <summary>Calculates if the player is standing on the closest solid object</summary>
     * <returns>True if the player's distance is too far from approximately 4.6 units of distance</returns>
     */
    public static bool isFarFromGround(GameObject targetBody, float x = 0f, float y = -1f, float z = 0f) {
        return ShadowController.findColPoint(targetBody, out var hit, x, y, z) && isFarFromGround(hit);
    }
    
    /**
     * <summary>An overloaded version from <see cref="checkForDistance(UnityEngine.GameObject)"/> where the rayCastHit is given</summary>
     * <returns>True if the player is far enough away from the closest ground (4.6f)</returns>
     * <remarks>4.59f is roughly the player's height plus some extra deviations. Anything above and the player is soaring</remarks>
     */
    public static bool isFarFromGround(RaycastHit hit) {
        return hit.distance > 4.6f;
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
     * <remarks>Note: for the function that gives a List-string, please supply a Transform</remarks>
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
        var parentList = new List<string>() {obj.name};
        do {
            obj = obj.transform.parent;
            parentList.Add(obj.name);
        }  while (obj.transform.parent); //while the next object's parent is not null
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

    // ReSharper disable Unity.PerformanceAnalysis
    /**
     * <summary>Alternative player movement function where only one angle is modified in the player's current velocity</summary>
     * <param name="velocity">Desired vector given to the player</param>
     * <param name="index">The desired index where the velocity will be changed.
     * <para>By default this parameter is 1 (denoting the y axis)</para></param>
     * <remarks>If the index falls outside the bounds of a traditional Vector3, this function will exit early</remarks>
     */
    public static void movePlayer(float velocity, int index = 1) {
        var desiredVel = pBody.velocity;
        for (var i = 0; i < 2; i++) {
            desiredVel[i] = index.Equals(i) ? velocity : pBody.velocity[i];
        } movePlayer(desiredVel);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /**
     * <summary>Finds the rigidbody attached to the player</summary>
     * <remarks>The component will always be found in a cheap way</remarks>
     */
    public static Rigidbody getPlayerBody() {
        if (pBody) { //pbody is checked against it being null
            return pBody;
        } init(); //this is only reached IF the player's body is null 
        return getPlayerBody(); //this statement will never be called under normal circumstances
    }

    /**
     * <summary>Attempts to find the player's hand</summary>
     * <returns>The empty used to store the objects designated to be "in the player's hand", the player's body's transform otherwise</returns>
     */
    public static Transform getPlayerHand() {
        return pHand;
    }
}
