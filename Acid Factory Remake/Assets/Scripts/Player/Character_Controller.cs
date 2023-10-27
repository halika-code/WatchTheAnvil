using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Move;

public class Character_Controller : MonoBehaviour {
    public const double MoveVel = 20;
    private static Rigidbody pBody;
    private static Toolbelt belt;

    //todo note: within functions if I write a function that has an out <variable> keyword, I can RETURN more than one value
    
    /**
     * <summary>Initialized the variables unique to the player</summary>
     */
    private static void init() {
        pBody = GameObject.Find("Player").GetComponent<Rigidbody>();
        pBody.freezeRotation = true;
        Physics.gravity = new Vector3(0, -30f);
        Collide.init();
    }

    private void OnEnable() { //singleton pattern, just in case
        if (pBody != null) {
            Destroy(this); //destroys the instance of this script playing if a player with an initialized pBody component can be found (playing in a separate script)
        }
    }

    private void Start() {
        init();
        belt = GetComponent<Toolbelt>();
    }

    // Update is called once per frame
    private void Update() {
        if (getMove() is not CanMove.Cant) {
            move();
            checkForItemUse();
        }
    }

    public static bool checkForDistance() {
        if (ShadowController.findColPoint(out var hit)) {
            return hit.distance > (getPlayerBody().GetComponent<Transform>().localScale.y/2)+1f; //the idea here is with the localScale I can get the height of the player from this data
        } return false;
    }

    /**
     * <summary><para>Evaluates the movement vector of the player</para>
     * Based on the keys supplied by the currently active gimmick.</summary>
     * <remarks>I wish I could implement this into a switch statement</remarks>
    */
    private static void move() {
        var vel = new Vector3(0f, pBody.velocity.y, 0f);
        if (Input.GetKey(KeyCode.A)) { //left
            vel.x = -(float)(MoveVel*1.5);
        } if (Input.GetKey(KeyCode.D)) { //right
            vel.x = (float)(MoveVel*1.5);
        } if (Input.GetKey(KeyCode.W)) { //up
            vel.z = (float)(MoveVel*1.5);
        } if (Input.GetKey(KeyCode.S)) { //down
            vel.z = -(float)(MoveVel*1.5);
        } movePlayer(vel);
    }
    
    /**
     * <summary>Checks if the player have pressed the E key in a non-mashy style</summary>
     */
    public static bool checkForVeggiePulls() {
        return Input.GetKey(KeyCode.E);
    }

    private void checkForItemUse() {
        if (Input.GetKey(KeyCode.F)){
            if (belt.checkIfToolExists("StopWatch", out var watch)) {
                StartCoroutine(belt.runStopWatch(watch, !belt.stopWatchInUse));
            }
        }
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
    private static void killPlayer() {
        GameObject.Find("Player").GetComponent<MonoBehaviour>().StopAllCoroutines(); 
        RootVeg.init(true);
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
    public static List<string> getParentName(GameObject obj) {
        var parentList = new List<string>();
        if (obj.name.Contains("Veggie")) {
            obj = obj.transform.parent.gameObject;
        } if (obj.transform.parent is null) {
            return new List<string> {obj.name};
        } do {
            parentList.Add(obj.name);
            obj = obj.transform.parent.gameObject;
        } while (obj.transform.parent != null);
        parentList.Add(obj.name);
        return parentList;
    }

    /**
     * <summary>Applies the desired vector of movement onto the player</summary>
     */
    public static void movePlayer(Vector3 movement) {
        pBody.velocity = movement;
        ShadowController.moveShadow(pBody.transform.position);
    }

    /**
     * <summary>Finds the rigidbody attached to the player</summary>
     * <remarks>The component will always be found</remarks>
     */
    public static Rigidbody getPlayerBody() {
        if (pBody != null) {
            return pBody;
        } init();
        return getPlayerBody();
    }

    public static bool isAscending() {
        return pBody.velocity.y > 0.05f;
    }
}
