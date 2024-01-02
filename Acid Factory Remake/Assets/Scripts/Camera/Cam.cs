using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Character_Controller;
public class Cam : MonoBehaviour {
    private Camera cam;
    private readonly Vector3 normalDistance = new (-1.23f, 50f, -120f); //this is the distance relative to the player where the camera has the player in the center

    private void Start() {
        if (cam != null) {
            cam.transform.position = normalDistance;
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
    private void moveCam() {
        if (checkIfLeftBorder()) { //if the player is stationary or inside the border
            moveFollowPlayer();
        } 
    }
    
    /**
     * <summary>Follows quickly the player using smooth scrolling</summary>
     * <remarks><see cref="Mathf.Lerp(float, float, float)"/> is used here to scroll smoothly</remarks>
     */
    private void moveFollowPlayer() {
        var pBody = getPlayerBody().position;
        var camPos = cam.transform.position;
        var asd = haveLeftBorder(camPos, pBody);
        foreach (var i in asd) {
            camPos[i] = Mathf.Lerp(camPos[i], normalDistance[i] + pBody[i], 0.02f);
        } cam.transform.position = camPos;
    }

    /**
     * <summary>Checks if the player have left a border</summary>
     * <param name="camPos">The main camera's current position</param>
     * <param name="pBody">The player's current position</param>
     * <returns>A list of booleans corresponding to each cardinal sides that the player have left the border at</returns>
     */
    private List<int> haveLeftBorder(Vector3 camPos, Vector3 pBody) {
        var asd = Math.Abs(camPos.x - pBody.x) > normalDistance.x + 2f; //todo check the z, since the cam doesn't want to follow the player upwards
        var asd2 = Math.Abs(camPos.y - pBody.y) > normalDistance.y + 2f;
        var asd3 = Math.Abs(camPos.z - pBody.z) > normalDistance.z + 2f;
        var leftBorderAt = new[] {asd,asd2
           , asd3 };
        if (!leftBorderAt.Contains(true)) {
            return new List<int> {-1};
        } var ret = new List<int>();
        for (var i = 0; i < 3; i++) {
            if (leftBorderAt[i]) {
                ret.Add(i);
            }
        } return ret;
    }

    /**
     * <summary>A simple check to check if the player have left a border at all</summary>
     * <returns>true if the player is deemed to have passed the border at any cardinal directions, false otherwise</returns>
     * <remarks>This function could be integrated (I imagine) with a LINQ style call into the <see cref="moveCam"/> function</remarks>
     */
    private bool checkIfLeftBorder() {
        return !haveLeftBorder(cam.transform.position, getPlayerBody().position).Contains(-1);
    }
}
