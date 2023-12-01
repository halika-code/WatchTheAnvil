using System;
using System.Collections;
using UnityEngine;
using static Character_Controller;

public class ShadowController : MonoBehaviour {
    private static Rigidbody sBody;
    
    private void Start() {
        sBody = gameObject.GetComponent<Rigidbody>();
    }

    /**
     * <summary>Attempts to find the closest solid object that exists underneath the player
     * <para>If one is found, the pane will be placed on the surface of it</para></summary>
     * <remarks>This is designed to run forever and is supposed to get killed as soon as the player lands</remarks>
     */
    public static IEnumerator findPlatform() { //var hit is the container of the collider of the object that was hit 
        if (!getShadowBody().activeSelf) {
            getShadowBody().SetActive(true);
        } while (true) {
            if (!findColPoint(out var hit) || hit.collider.gameObject.name is "DeathPane") {
                getShadowBody().SetActive(false);
            } if (getParentName(hit.collider.transform) is "Platforms" or "Anvils") {
                setShadowPosition(new Vector3(hit.point.x, hit.point.y+0.1f, hit.point.z));
            } yield return null;
        }
    }

    /**
     * <summary>Fetches the collision point the Raycast finds underneath the player</summary>
     * <param name="hit">an alternate return for the container of info of the object the raycast have hit</param>
     * <returns>True if a hit was detected, false otherwise</returns>
     */
    public static bool findColPoint(out RaycastHit hit) {
        var ray = new Ray(getPlayerBody().position, Vector3.down);
        return Physics.Raycast(ray, out hit, 50f);
    }

    private static void setShadowPosition(Vector3 pos) {
        sBody.position = pos;
    }

    /**
     * <summary>Adds the given velocity to the shadow pane</summary>
     * <remarks>Uses plain addition</remarks>
     */
    public static void moveShadow(Vector3 pos) {
        sBody.transform.position = new Vector3(pos.x, sBody.position.y, pos.z);
    }

    public static GameObject getShadowBody() {
        return sBody.gameObject;
    }
}
