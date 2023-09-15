using System;
using UnityEngine;
using static Character_Controller;

public class ShadowController : MonoBehaviour {
    private static Rigidbody sBody;
    
    private void Start() {
        sBody = gameObject.GetComponent<Rigidbody>();
        findPlatform(getPlayerBody());
    }

    private void FixedUpdate() {
        if (!getShadowBody().activeSelf) { //if the shadow-pane have been disabled
            findPlatform(getPlayerBody());
        }
    }

    private void OnCollisionEnter() {
        movePlayer(Vector3.zero);
    }

    private void OnCollisionExit() {
        findPlatform(getPlayerBody());
    }

    /**
     * <summary>Attempts to find the closest object that exists underneath the player
     * <para>If one is found, the pane will be placed to the surface of it</para></summary>
     */
    private static void findPlatform(Rigidbody pBody) { //var hit is the container of the collider of the object that was hit 
        if (!Physics.Raycast(pBody.velocity, pBody.transform.TransformDirection(Vector3.down), out var hit, 50f)) {
            getShadowBody().SetActive(false);
            return;
        }  if (getParentName(hit.collider.gameObject).name is "Platforms") {
            setShadowPosition(hit.point + new Vector3(0f, 0.5f, 0f));
        } getShadowBody().SetActive(true);
    }

    private static void setShadowPosition(Vector3 pos) {
        sBody.position = pos;
    }

    /**
     * <summary>Adds the given position to the shadow pane</summary>
     * <remarks>Uses plain addition</remarks>
     */
    private static void moveShadow(Vector3 pos) {
        var originalPos = sBody.position; //todo connect this to the Character_Controller
        sBody.position = originalPos + pos;
    }

    private static void movePlayer(Vector3 movement) {
        sBody.velocity = movement;
    }

    private static GameObject getShadowBody() {
        return sBody.gameObject;
    }
}
