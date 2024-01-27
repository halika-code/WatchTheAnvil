using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Script.Tools.ToolType;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Move;

/**
 * <date>17/06/2023</date>
 * <author>ciglos mikCiglos aka cigi migi</author>
 * <summary>A traditional character controller script, this class is supposed to
 * move the player in all cardinal directions alongside extra logic</summary>
 */
public class Character_Controller : MonoBehaviour {
    
    public const double MoveVel = 20; 
    protected static Rigidbody pBody;
    private static Transform pHand;
    /**
     * A boolean that keeps track if the player is air-borne
     * <para>Set to true the 1st frame execution starts for the jump routine, set to false when colliding with the ground</para>
     */
    public static bool isAscending; 
    public static float priorYVel;

    //todo note: within functions if I write a function that has an out <variable> keyword, I can RETURN more than one variable
    
    /**
     * <summary>Initialized the variables unique to the player</summary>
     */
    private static void init() {
        setPlayerBody();
        Physics.gravity = new Vector3(0, -30f);
        priorYVel = 0f;
        pHand = GameObject.Find("Hand").transform;
        isAscending = false;
    }

    /**
     * <summary>Prepares the player's RigidBody, moving it to the burrow if one can be found</summary>
     */
    private static void setPlayerBody() {
        pBody = GameObject.Find("Player").GetComponent<Rigidbody>();
        pBody.freezeRotation = true;
        var burrow = GameObject.Find("Enter");
        if (burrow != null) {
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
            movePlayer(InputController.move(calculateVel()));
        } if (InputController.checkForItemUse()) { //if the player wants to use the item and the cooldown flag is clear
            Toolbelt.getBelt().fetchItem();
        }
    }

    /**
     * <summary>Dampens the button press</summary>
     */
    private static Vector3 calculateVel() {
        var ret = new Vector3(0f, pBody.velocity.y, 0f);
        if (isAscending) {
            var pVel = pBody.velocity;
            for(var i = 0; i < 3; i+=2) {
                if (Math.Abs(Math.Round(pVel[i] / 1.5, 2)) > 0.1f) {
                    ret[i] = pVel[i];
                }
            } return new Vector3(pVel.x * .9f, pVel.y, pVel.z * .9f);
        } return ret;
    }

    private void FixedUpdate() {
        var pVel = pBody.velocity;
        if (getMove() is not CanMove.CantJump && InputController.checkForJump()) {
            if ((Toolbelt.getBelt().checkForTool("Umbrella", out var umbrella))) {
                if (((Umbrella)umbrella).checkIfOpen()) {
                    movePlayer(new Vector3(pVel.x, (float)MoveVel * 1.4f, pVel.z));
                    return;
                } 
            } GravAmplifier.gravity.falling(new Vector3(pVel.x, (float)MoveVel*3, pVel.z));
        } else {
            updatePriorVel();
        }
    }

    public static bool checkForDistance() {
        if (ShadowController.findColPoint(out var hit)) {
            return hit.distance > (getPlayerBody().transform.localScale.y/2)+1f; //the idea here is with the localScale I can get the height of the player from this data
        } return false;
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
    public static void movePlayer(Vector3 movement) {
        pBody.velocity = movement;
        ShadowController.moveShadow(pBody.transform.position); //todo find every instance of an object with a rigidbody being moved with position and replace with
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
    
    /**
     * <summary>Saves the previously calculated velocity calculated in the last physics update</summary>
     */
    private static void updatePriorVel() { 
        priorYVel = getPlayerBody().velocity.y;
    }
}
