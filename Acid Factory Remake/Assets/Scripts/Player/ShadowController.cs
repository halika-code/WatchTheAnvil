using System.Collections;
using UnityEngine;
using static Character_Controller;

/**
 * <date>03/07/2023</date>
 * <author>Gyula Attila Kovacs(gak8)</author>
 * <summary>A simple class that keeps the shadow under the player's feet</summary>
 */
public class ShadowController : MonoBehaviour {
    private new static MeshRenderer renderer;
    private static Rigidbody sBody;
    private static RaycastHit lastHitObj;
    private static bool isRunning;
    
    private void Start() {
        sBody = gameObject.GetComponent<Rigidbody>();
        renderer = gameObject.GetComponent<MeshRenderer>();
        if (!findPlatform()) { //terminate early if the player is grounded
            return;     //save some performance
        } StartCoroutine(followPlayer()); 
    }

    /**
     * <summary>Attempts to find the closest solid object that exists underneath the player
     * <para>If one is found, the pane will be placed on the surface of it</para></summary>
     * <remarks>This is designed to run forever and is supposed to get stopped as soon as the player lands</remarks>
     */
    public static IEnumerator followPlayer() { //var hit is the container of the collider of the object that was hit 
        if (!isRunning) { //if an instance is not running yet
            var counter = 0;
            isRunning = true;
            do {
                if (checkForPlatform(counter, out counter)) { //repositions the shadow to better follow the player
                    var pBodyPos = getPlayerBody().position;
                    setShadowPosition(new Vector3(pBodyPos.x, lastHitObj.point.y + 0.02f, pBodyPos.z)); //move the shadow's position to be exactly underneath the player, smugly on top the floor
                } yield return null;
            } while (renderer.enabled || lastHitObj.collider.name is "DeathPane"); //loop until the renderer is visible (or the last hit object is the deathpane). This will exit when the player is too close to the ground
            isRunning = false; //todo the while statement will not end normally, check why the findPlatform won't work
        }
    }

    /**
     * <summary>Checks if there is ground underneath the player
     * <para>If so, the lastHitObj will get updated</para></summary>
     */
    private static bool checkForPlatform(int counter, out int count) {
        counter++;
        if (lastHitObj.collider || counter > 9) { //every 10th loop OR if the lastHitObj have been dumped mid-loop, update y position
            count = 0;
            return findPlatform(); //return true as long as the floor underneath the player is a valid one and is far down (but not too far)
        } count = counter;
        return getParentName(lastHitObj.collider.gameObject) is not "Tools" || 
               !getParentName(lastHitObj.collider.gameObject).Contains("Text");
        //returns true as long as the object underneath is not a tool or a type of text
    }

    /**
     * <summary>Updates the point the player is directly under
     * <para>If no valid floor is found, the shadow is terminated instead</para></summary>
     */
    private static bool findPlatform() {
        renderer.enabled = findColPoint(out lastHitObj) || checkForDistance(lastHitObj); //if no valid floor is found or the distance between the floor and player is insignificant
        return renderer.enabled; 
    }

    /**
     * <summary>Fetches the collision point the Raycast finds underneath the player</summary>
     * <param name="hit">A container of the object struck by the ray</param>
     * <returns>True if a hit was detected within 30f distance, false otherwise</returns>
     */
    public static bool findColPoint(out RaycastHit hit) {
        var ray = new Ray(getPlayerBody().position, Vector3.down);
        return Physics.Raycast(ray, out hit, 50f);
    }

    /**
     * <summary>Fetches the collision point the Raycast finds underneath the desired GameObject for a shorter distance</summary>
     * <param name="parentObj">The object the ray should shoot underneath from</param>
     * <param name="hit">A container for the object struck by the ray, including exact collision position</param>
     * <returns>True if a hit is detected within 20f distance, false otherwise</returns>
     */
    public static bool findColPoint(GameObject parentObj, out RaycastHit hit) {
        var ray = new Ray(parentObj.transform.position, Vector3.down);
        return Physics.Raycast(ray, out hit, 20f);
    }

    private static void setShadowPosition(Vector3 pos) {
        sBody.position = pos;
    }

    /**
     * <summary>Moves the shadow horizontally only</summary>
     * <remarks>Uses plain addition</remarks>
     */
    public static void moveShadow(Vector3 pos) {
        sBody.transform.position = new Vector3(pos.x, sBody.position.y, pos.z);
    }
}
