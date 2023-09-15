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
    
    private void FixedUpdate() {
        moveCam();
    }

    /**
     * <summary>Moves the cam if it is deemed to have left a border</summary>
     */
    private static void moveCam() {
        if (checkIfLeftBorder()) { //if the player is stationary or inside the border
            moveFollowPlayer();
        } 
    }
    
    /**
     * <summary>Follows quickly the player using smooth scrolling</summary>
     * <remarks><see cref="Mathf.Lerp(float, float, float)"/> is used here to scroll smoothly</remarks>
     */
    private static void moveFollowPlayer() {
        var pBody = getPlayerBody().position;
        var camPos = cam.transform.position;
        var leftBorderAt = haveLeftBorder(camPos, pBody);
        if (leftBorderAt[0]) {
            camPos.x = Mathf.Lerp(camPos.x, NormalDistance.x + pBody.x, 0.02f);
        } if (leftBorderAt[1]) {
            camPos.y = Mathf.Lerp(camPos.y, NormalDistance.y + pBody.y, 0.02f);
        } if (leftBorderAt[2]) {
            camPos.z = Mathf.Lerp(camPos.z, NormalDistance.z + pBody.z, 0.02f);
        } cam.transform.position = camPos;
    }

    /**
     * <summary>Checks if the player have left a border</summary>
     * <param name="camPos">The main camera's current position</param>
     * <param name="pBody">The player's current position</param>
     * <returns>A list of booleans corresponding to each cardinal sides that the player have left the border at</returns>
     */
    private static bool[] haveLeftBorder(Vector3 camPos, Vector3 pBody) {
        var camPosPar = Math.Sign(camPos.x) is 1; //cameraPositionParity
        var pBodyPar = Math.Sign(pBody.x) is 1; //playerBodyParity
        if (camPosPar && !pBodyPar || !camPosPar && pBodyPar) { //checking here to not have a case of (45[camPos] + (-5)[pBody])
            camPos.x *= Math.Sign(camPos.x); //doesn't matter which variable I multiply, since multiplying by 1 doesn't ruin anything
            pBody.x *= Math.Sign(pBody.x); 
        } return new[] { Math.Abs(camPos.x - pBody.x) > NormalDistance.x + 2f, 
                Math.Abs(camPos.y - pBody.y) > NormalDistance.y + 2f, Math.Abs(camPos.z - pBody.z) > NormalDistance.z + 2f };
    }

    /**
     * <summary>A simple check to check if the player have left a border at all</summary>
     * <returns>true if the player is deemed to have passed the border at any cardinal directions, false otherwise</returns>
     * <remarks>This function could be integrated (I imagine) with a LINQ style call into the <see cref="moveCam"/> function</remarks>
     */
    private static bool checkIfLeftBorder() {
        foreach (var borderSide in haveLeftBorder(cam.transform.position, getPlayerBody().position)) {
            if (borderSide) {
                return true;
            }
        } return false;
    }
}
