using System;
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
    public static RaycastHit lastHitObj;
    
    private void Start() {
        sBody = gameObject.GetComponent<Rigidbody>();
        renderer = gameObject.GetComponent<MeshRenderer>();
    }

    /**
     * <summary>Attempts to find the closest solid object that exists underneath the player
     * <para>If one is found, the pane will be placed on the surface of it</para></summary>
     * <remarks>This is designed to run forever and is supposed to get stopped as soon as the player lands</remarks>
     */
    public static IEnumerator findPlatform() { //var hit is the container of the collider of the object that was hit 
        if (!renderer.enabled) {
            renderer.enabled = true;
        } do {
            
            if (!findColPoint(out lastHitObj) || lastHitObj.collider.gameObject.name is "DeathPane") { //todo use lastHitObj's distance to decide if a raycast is needed. If not, divide between the player's body and the lasthitObj's distance
               yield break;
            }

            var asd = getPlayerBody().position;
            //the idea here is the player's position is the greatest, if I subtract the lastHitObj's position
            //then the shadow-body's local position (compared to the player)
            //I will get the distance between the ground and the shadow
            var difference = lastHitObj.point.y - asd.y; //todo have the Mathf.Lerp smoothly interpolate between sbody's and lastHitObj's position, 
                        //todo don't do anything with the difference since that would have the shadow lag behind the player instead of jittering, making it even worse
            if (getParentName(lastHitObj.collider.gameObject) is not "Tools" || !getParentName(lastHitObj.collider.gameObject).Contains("Text")) {
                setShadowPosition(new Vector3(lastHitObj.point.x, lastHitObj.point.y + 0.15f, lastHitObj.point.z));
            } yield return null;
        } while (checkForDistance());
        renderer.enabled = false;
    }

    /**
     * <summary>Fetches the collision point the Raycast finds underneath the player</summary>
     * <param name="hit">A container for the object struck by the ray</param>
     * <returns>True if a hit was detected within 30f distance, false otherwise</returns>
     */
    public static bool findColPoint(out RaycastHit hit) {
        var ray = new Ray(getPlayerBody().position, Vector3.down);
        return Physics.Raycast(ray, out hit, 30f);
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

    public static GameObject getShadowBody() {
        return sBody.gameObject;
    }
}
