using System;
using System.Collections;
using Unity.VisualScripting;
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
        if (validatePlatform()) {
            StartCoroutine(followPlayer());
        }
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
            while (GravAmplifier.isAscending) { //loop until the renderer is visible (or the last hit object is the deathpane). This will exit when the player is too close to the ground
                if (checkForPlatform(counter, out counter)) { //repositions the shadow to better follow the player
                    var pBodyPos = getPlayerBody().position;
                    setShadowPosition(new Vector3(pBodyPos.x, lastHitObj.point.y + 0.02f, pBodyPos.z)); //move the shadow's position to be exactly underneath the player, smugly on top the floor
                } yield return null;
            } renderer.enabled = false; //needs to have this here in case the player lands outside the checkForPlatform cycle
            isRunning = false;
        }
    }

    /**
     * <summary>Checks if there is ground underneath the player
     * <para>If so, the lastHitObj will get updated</para></summary>
     */
    private static bool checkForPlatform(int counter, out int count) {
        counter++;
        if (lastHitObj.collider.IsUnityNull() || counter > 9) { //every 10th loop OR if the lastHitObj have been dumped mid-loop, update y position
            count = 0;
            return validatePlatform(); //return true as long as the floor underneath the player is a valid one and is far down (but not too far)
        } count = counter;
        return getParentName(lastHitObj.collider.gameObject) is not "Tools" || 
               !getParentName(lastHitObj.collider.gameObject).Contains("Text");
        //returns true as long as the object underneath is not a tool or a type of text
    }

    /**
     * <summary>Attempts to validate the current platform underneath the player
     * <para>If the platform is too far or is meant to kill the player, the shadow will get disabled</para></summary>
     * <remarks>Note: the lastHitObj is, under normal circumstances, updated here</remarks>
     */
    private static bool validatePlatform() {
        var pSpeed = getPlayerBody().velocity;
        if (findColPoint(out lastHitObj, getSp(pSpeed.x), -1f, getSp(pSpeed.z)) && !lastHitObj.collider.name.Contains("Death")) { //if the player is above valid ground that is not the deathpane
            if (Math.Sign(getPlayerBody().velocity.y) is not -1 || isFarFromGround(lastHitObj)) { //if the player is gaining altitude or far from the ground
                renderer.enabled = true;
                return true;
            } 
        } renderer.enabled = false;
        return false;
    }

    private static float getSp(float speed, float defVal = 0f) {
        return VelocityManipulation.absRound(speed) > 0.2f ? Math.Sign(speed) * 0.2f : defVal;
    }

    private static bool shootRay(Ray ray, out RaycastHit hit) {
        return Physics.Raycast(ray, out hit, 200f);
    }

    /**
     * <summary>Fetches the collision point the Raycast finds underneath the player</summary>
     * <param name="hit">A container of the object struck by the ray</param>
     * <param name="x">A default value that can manipulate the raycast's firing direction</param>
     * <param name="y">A default value that can manipulate the raycast's firing direction</param>
     * <param name="z">A default value that can manipulate the raycast's firing direction</param>
     * <returns>True if a hit was detected within 30f distance, false otherwise</returns>
     */
    public static bool findColPoint(out RaycastHit hit, float x = 0, float y = -1, float z = 0) {
        return shootRay(!renderer.enabled
            ? new Ray(getPlayerBody().position, Vector3.down)
            : /*Use a different angle if the shadow is enabled (to not re-enable while above the death-pit)*/
            new Ray(getPlayerBody().position, new Vector3(x, y, z)), out hit);
    }

    /**
     * <summary>Fetches the collision point the Raycast finds underneath the desired GameObject for a shorter distance</summary>
     * <param name="parentObj">The object the ray should shoot underneath from</param>
     * <param name="hit">A container for the object struck by the ray, including exact collision position</param>
     * <returns>True if a hit is detected within 20f distance, false otherwise</returns>
     */
    public static bool findColPoint(GameObject parentObj, out RaycastHit hit) {
        return shootRay(new Ray(parentObj.transform.position, Vector3.down), out hit);
    }
    
    /**
     * <summary>Fetches the collision point the Raycast finds towards a desired position for the desired GameObject for a longs distance</summary>
     * <param name="parentObj">The object the ray should shoot underneath from</param>
     * <param name="hit">A container for the object struck by the ray, including exact collision position</param>
     * <param name="x">A default value that can manipulate the raycast's firing direction</param>
     * <param name="y">A default value that can manipulate the raycast's firing direction</param>
     * <param name="z">A default value that can manipulate the raycast's firing direction</param>
     * <returns>True if a hit is detected within 20f distance, false otherwise</returns>
     */
    public static bool findColPoint(GameObject parentObj, out RaycastHit hit, float x = 0, float y = -1, float z = 0) {
        return shootRay(new Ray(parentObj.transform.position, new Vector3(x, y, z)), out hit);
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
