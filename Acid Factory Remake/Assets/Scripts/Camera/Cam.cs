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

    private static void moveCam() {
        if (checkIfLeftBorder()) { //if the player is stationary or inside the border
            moveFollowPlayer();
        } 
    }
    
    /**
     * <summary>Follows quickly the player</summary>
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

    private static bool[] haveLeftBorder(Vector3 camPos, Vector3 pBody) {
        var camPosPar = Math.Sign(camPos.x) is 1; //cameraPositionParity
        var pBodyPar = Math.Sign(pBody.x) is 1; //playerBodyParity
        if (camPosPar && !pBodyPar || !camPosPar && pBodyPar) { //checking here to not have a case of (45[camPos] + (-5)[pBody])
            camPos.x *= Math.Sign(camPos.x); //doesn't matter which variable I multiply, since multiplying by 1 doesn't ruin anything
            pBody.x *= Math.Sign(pBody.x); 
        } return new[] { Math.Abs(camPos.x - pBody.x) > NormalDistance.x + 2f, 
                Math.Abs(camPos.y - pBody.y) > NormalDistance.y + 2f, Math.Abs(camPos.z - pBody.z) > NormalDistance.z + 2f };
    }

    private static bool checkIfLeftBorder() {
        foreach (var borderSide in haveLeftBorder(cam.transform.position, getPlayerBody().position)) {
            if (borderSide) {
                return true;
            }
        } return false;
    }
}
