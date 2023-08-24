using System;
using UnityEngine;

public class Cam : MonoBehaviour {
    
    private static Camera cam; 
    private static Vector3 border;
    
    private void Start() {
        if (cam != null) {
            cam.transform.position = new Vector3(0f, 50f, -120f);
        } else {
            cam = Camera.main;
        } border = new Vector3(50f, 50f, 50f);
    }
    
    private void Update() {
        moveCam();
        updateBorder();
    }

    private static void moveCam() {
        if (!Character_Controller.isMoving() || !haveLeftBorder()) { //if the player is stationary or inside the border
            moveCatchup();
        } else {
            moveFollowPlayer();
        }
    }

    /**
     * <summary>Attempts to move the camera smoothly to the player</summary>
     */
    private static void moveCatchup() {
        var pBody = Character_Controller.getPlayerBody();
        while (haveLeftBorder()) {
            var camPos = cam.transform.position;
            var target = camPos + ((pBody.transform.position - camPos) / 2); //target is halfway between the player and the camera's position
            Vector3.Lerp(camPos, target, 0.2f);
        }
    }

    /**
     * <summary>Follows quickly the player</summary>
     */
    private static void moveFollowPlayer() {
        var pBody = Character_Controller.getPlayerBody().position;
        var camPos = cam.transform.position;
        var leftBorderAt = checkIfInBorder();
        if (leftBorderAt[0]) {
            camPos.x = pBody.x - border.x;
        } if (leftBorderAt[1]) {
            camPos.y = pBody.y - border.y;
        } if (leftBorderAt[2]) {
            camPos.z = pBody.z - border.z;
        } cam.transform.position = camPos;
    }

    private static void updateBorder() {
        border = cam.transform.position + new Vector3(20f, 20f, 20f); //todo check the border to have a good size
    }

    /**
     * <summary>Checks if the player is inside the border placed in the center of the camera</summary>
     * <returns>Based on player's relative position,
     * <para>1-3 if x, y or z boundary is passed</para> 
     * 0 if player sits inside the boundary
     * </returns>
     * <remarks>This function checks the relative x and y position of the player</remarks>
     */
    private static bool[] checkIfInBorder() {
        var returnValue = new[] {false, false, false};
        var pBodyPosRelative = cam.WorldToScreenPoint(Character_Controller.getPlayerBody().position);
        var camCenter = new Vector2(((float)cam.pixelWidth/2), (float)cam.pixelHeight/2);
        if (Math.Abs(pBodyPosRelative.x) > (camCenter + (Vector2)border).x) {
            returnValue[0] = true;
        } if (Math.Abs(pBodyPosRelative.y) > (camCenter + (Vector2)border).y) {
            returnValue[1] = true;
        } if (Math.Abs(pBodyPosRelative.z) > (cam.transform.position + border).z) {
            returnValue[2] = true;
        } return returnValue;
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
