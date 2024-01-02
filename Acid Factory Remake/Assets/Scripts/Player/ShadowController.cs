using System;
using System.Collections;
using UnityEngine;
using static Character_Controller;

public class ShadowController : MonoBehaviour {
    private new static MeshRenderer renderer;
    private static Rigidbody sBody;
    
    private void Start() {
        sBody = gameObject.GetComponent<Rigidbody>();
        renderer = gameObject.GetComponent<MeshRenderer>();
        StartCoroutine(findPlatform());
    }

    /**
     * <summary>Attempts to find the closest solid object that exists underneath the player
     * <para>If one is found, the pane will be placed on the surface of it</para></summary>
     * <remarks>This is designed to run forever and is supposed to get killed as soon as the player lands</remarks>
     */
    public static IEnumerator findPlatform() { //var hit is the container of the collider of the object that was hit 
        if (!renderer.enabled) {
            renderer.enabled = true;
        } do {
            if (!findColPoint(out var hit) || hit.collider.gameObject.name is "DeathPane") {
                break;
            } if (getParentName(hit.collider.gameObject) is "Platforms" or "Anvils") {
                setShadowPosition(new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z));
            } yield return null;
        } while (!checkIfStandingOnPlayer());
        renderer.enabled = false;
    }

    private static bool checkIfStandingOnPlayer() {
        var asd = getPlayerBody().position.y;
        var asd2 = Math.Round(asd - sBody.position.y) < 1.5f;
        return Math.Round(getPlayerBody().position.y - sBody.position.y, 2) < 1.5f; //1.4 is the distance found using debug.log + 0.6 to account for Unity weirdness + padding 
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
