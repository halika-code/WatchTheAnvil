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
    
    private void Start() {
        sBody = gameObject.GetComponent<Rigidbody>();
        renderer = gameObject.GetComponent<MeshRenderer>();
        findPlatform(); //equal to "lastHitObj = new RayCastHit();"
        StartCoroutine(followPlayer());
    }

    /**
     * <summary>Attempts to find the closest solid object that exists underneath the player
     * <para>If one is found, the pane will be placed on the surface of it</para></summary>
     * <remarks>This is designed to run forever and is supposed to get stopped as soon as the player lands</remarks>
     */
    public static IEnumerator followPlayer() { //var hit is the container of the collider of the object that was hit 
        var counter = 0;
        if (!renderer.enabled) {
            renderer.enabled = true;
        } do {
            if (checkForPlatform(counter, out counter)) {
                var pBodyPos = getPlayerBody().position;
                setShadowPosition(new Vector3(pBodyPos.x, lastHitObj.point.y + 0.02f, pBodyPos.z));
            } yield return null;
        } while (Character_Controller.checkForDistance(lastHitObj));
        renderer.enabled = false;
    }

    /**
     * <summary>A helper function for <see cref="followPlayer()"/>
     * Decides if the platform in memory have been purged or not.
     * <para>If so, the said distance is refreshed</para></summary>
     * <remarks>Also checks if the the last object hit is an expected one or not</remarks>
     */
    private static bool checkForPlatform(int counter, out int count) {
        counter++;
        if (lastHitObj.collider || counter > 9) { //every 10th loop OR if the lastHitObj have been dumped mid-loop, update y position
            findPlatform();
            counter = 0;
        } count = counter;
        return getParentName(lastHitObj.collider.gameObject) is not "Tools" || !getParentName(lastHitObj.collider.gameObject).Contains("Text");
    }

    /**
     * <summary>Updates the point the player is directly under</summary>
     */
    private static void findPlatform() {
        if (!findColPoint(out lastHitObj)) {
            renderer.enabled = false;
        }
    }

    /**
     * <summary>Fetches the collision point the Raycast finds underneath the player</summary>
     * <param name="hit">A container for the object struck by the ray</param>
     * <returns>True if a hit was detected within 30f distance, false otherwise</returns>
     */
    public static bool findColPoint(out RaycastHit hit) {
        var ray = new Ray(getPlayerBody().position, Vector3.down);
        return Physics.Raycast(ray, out hit, 100f);
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

    /**
     * <summary>Checks if the player is standing on it's shadow</summary>
     */
    public static bool checkForDistance() {
        return Character_Controller.checkForDistance(lastHitObj);
    }
}
