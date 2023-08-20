using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour {
    
    private static Camera cam; //todo idea: check if the player is dead center, (if true, do nothing),
                               //todo otherwise if the player is inside a border that is like 3 times the size of the player, then slowly follow the palyer
                               //todo for example follow with the speed of the player
                               //todo if the player goes out of that border, follow the player normally, lagging a bit behind until the player is back in the border
    private static Vector3 border;
    private void Start() {
        if (cam == null) {
            cam = Camera.main;
        } cam.transform.position = new Vector3(0f, 50f, -120f);
        border = new Vector3(50f, 50f, 50f);
    }

    // Update is called once per frame
    private void Update() {
        
    }

    private static void moveCam() {
        if (!Character_Controller.isMoving()) {
            moveCatchup();
        } else if () {
            
        }
    }

    private static void moveCatchup() {
        var pBody = Character_Controller.getPlayerBody();
        var camPos = cam.transform.position;
        var target = camPos + (pBody.transform.position - camPos / 2);
        if (!Character_Controller.isMoving()) {
            Vector3.Lerp(camPos, target, 0.5f);
        } 
    }

    private static void moveFollowPlayer() {
        var pBody = Character_Controller.getPlayerBody();
        cam.transform.position = updateBorder(pBody.velocity) - pBody.position; //the camera will always be 1 border size away from the player
    }

    private static Vector3 updateBorder(Vector3 pBodyVel) {
        border.x *= Math.Sign(pBodyVel.x);
        border.y *= Math.Sign(pBodyVel.y);
        border.z *= Math.Sign(pBodyVel.z);
        return border;
    }

    private static bool checkIfInBorder() {
        var camPos = cam.transform.position;
        var pBodyPos = Character_Controller.getPlayerBody().position;
        //todo use cam.worldToScreenSpace, or check worldToViewPort in order to decide which side the player is compared to the center of the camera

        return false;
    }
}
