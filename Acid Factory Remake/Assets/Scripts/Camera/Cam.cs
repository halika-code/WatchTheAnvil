using System;
using System.Collections;
using UnityEngine;
using static Character_Controller;
public class Cam : MonoBehaviour {
    
    private static Camera cam;
    private static readonly Vector3 NormalDistance = new (-1.23f, 50f, -120f); //this is the distance relative to the player where the camera has the player in the center

    private void Start() {
        if (cam != null) {
            cam.transform.position = NormalDistance;
        } else {
            cam = Camera.main;
        }
    }
    
    private void Update() {
        moveCam();
    }

    private static void moveCam() {
        if (!isMoving() || !haveLeftBorder()) { //if the player is stationary or inside the border
            moveFollowPlayer();
        } 
    }
    /**
     * <summary>Follows quickly the player</summary>
     */
    private static void moveFollowPlayer() {
        var pBody = getPlayerBody().position;
        var camPos = cam.transform.position;
        var leftBorderAt = checkIfInBorder();
        if (leftBorderAt[0]) { //todo now the border-check is correct, it returns all fine, this ...
                               //todo perhaps needs to be inside a coroutine with a yield return waitforfixedupdate
            camPos.x = NormalDistance.x + pBody.x - (Math.Sign(pBody.x) * getBorder().x);
        } if (leftBorderAt[1]) {
            camPos.y = NormalDistance.y + pBody.y - (Math.Sign(pBody.y) * getBorder().y);
        } if (leftBorderAt[2]) {
            camPos.z = NormalDistance.z + pBody.z - (Math.Sign(pBody.z) * getBorder().z);
        } cam.transform.position = camPos;
    }

    private static Vector3 getBorder() { //todo add extra padding to the x axis to be able to see the player properly
        return cam.transform.position + new Vector3(5f, 5f, 5f);
    }

    /**
     * <summary>Checks if the player is inside the border placed in the center of the camera</summary>
     * <returns>Based on player's relative position,
     * <para>Evaluates if the player have left any of the boundaries set on the camera</para>
     * </returns>
     * <remarks>Calculates with an extra buffer in mind for the z axis</remarks>
     */
    private static bool[] checkIfInBorder() {
        var pBodyPos = new Vector3(Math.Abs(getPlayerBody().position.x), Math.Abs(getPlayerBody().position.y), Math.Abs(getPlayerBody().position.z));
        var camPos = new Vector3(Math.Abs(cam.transform.position.x), Math.Abs(cam.transform.position.y), Math.Abs(cam.transform.position.z));
        return new []{pBodyPos.x + camPos.x >= 50f, pBodyPos.y + camPos.y >= 50f + 50f, pBodyPos.z + camPos.z >= 120f + 50f};
    }

    /**
     * <summary>Returns if the player have left any side of the border</summary>
     * <returns>true, if the player have left any side of the border attached to the camera, false otherwise</returns>
     */
    public static bool haveLeftBorder() {
        foreach (var borderSide in checkIfInBorder()) {
            if (borderSide) {
                return true;
            }
        } return false;
    }
}
