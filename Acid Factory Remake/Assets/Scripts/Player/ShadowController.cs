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
     * <summary>Attempts to find the closest object that exists underneath the player
     * <para>If one is found, the pane will be placed to the surface of it</para></summary>
     */
    public static IEnumerator findPlatform(Rigidbody pBody) { //var hit is the container of the collider of the object that was hit 
        while (true) {
            var ray = new Ray(pBody.position, Vector3.down);
            if (!Physics.Raycast(ray, out var hit, 50f)) {
                getShadowBody().SetActive(false);
            } if (getParentName(hit.collider.gameObject).name is "Platforms") {
                setShadowPosition(new Vector3(hit.point.x, hit.point.y+0.1f, hit.point.z));
            } if (!getShadowBody().activeSelf) {
                getShadowBody().SetActive(true);
            } yield return null;
        }
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
