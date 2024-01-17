using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Character_Controller;

/**
 * <date>20/08/2023</date>
 * <author>Gyula Attila Kovacs</author>
 * <summary>A script that handles the movement of the camera using a pre-set border and an activation formula</summary>
 * <remarks>The camera only moves, will not rotate and
 * needs to be snapped back to the player in case the player is warped further than the border in a frame (will jitter until the player reappears into frame)</remarks>
 */

public class Cam : MonoBehaviour {
    private Camera cam;
    private readonly Vector3 normalDistance = new (5f, 50f, -80f); //this is the distance relative to the player where the camera has the player in the center

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
        foreach (var i in haveLeftBorder(camPos, pBody)) {
            camPos[i] = Mathf.Lerp(camPos[i], normalDistance[i] + pBody[i], 0.02f);
        } cam.transform.position = camPos;
    }

    /**
     * <summary>Checks if the player have left a border</summary>
     * <param name="camPos">The main camera's current position</param>
     * <param name="pBody">The player's current position</param>
     * <returns>A list of integers corresponding to each cardinal sides that the player have left the border at</returns>
     */
    private List<int> haveLeftBorder(Vector3 camPos, Vector3 pBody) {
        var leftBorderAt = calculateBorders(camPos, pBody);
        var ret = new List<int>();
        if (!leftBorderAt.Contains(true)) {
            ret.Add(-1);
            return ret;
        } for (var i = 0; i < leftBorderAt.Length; i++) {
            if (leftBorderAt[i]) {
                ret.Add(i);
            }
        } return ret;
    }

    /**
     * <summary>Calculates if the player have left the border</summary>
     * <para>Using a formula to decide if the player have left the border</para>
     * <returns>A list of bool denoting if the given direction have been passed over</returns>
     * <remarks>The formula works for all 3 axis</remarks>
     */
    private bool[] calculateBorders(Vector3 camPos, Vector3 pBody) {
        var retVal = new bool[3];
        for (var i = 0; i < 3; i++) {
            retVal[i] = Math.Abs(camPos[i] * Math.Sign(camPos[i]) - (normalDistance[i] + Math.Sign(camPos[i]) * pBody[i])) > 5;
        } return retVal;
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
