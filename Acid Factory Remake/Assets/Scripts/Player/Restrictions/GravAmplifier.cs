using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Script.Tools.ToolType;
using UnityEngine;
using static Character_Controller;

/**
 * <date>11/01/2024</date>
 * <author>Gyula Attila Kovacs</author>
 * <summary>Amplifies the gravity only when the player is deemed to be falling</summary>
 */

public class GravAmplifier : MonoBehaviour {

    public static GravAmplifier gravity;

    public void OnEnable() {
        gravity = GameObject.Find("Player").GetComponentInChildren<GravAmplifier>();
    }

    /**
     * <summary>Calculates the falling velocity during the downward arch before reaching terminal (desired) velocity</summary>
     */
    public void falling(Vector3 hop) {
        if (!(Toolbelt.getBelt().checkForTool("Umbrella", out var umbrella) && ((Umbrella)umbrella).checkIfOpen())) { 
            StartCoroutine(gravAmplifier(hop)); //idea here is to have the gravity work specifically when the player is not jumping
        }
    }

    private IEnumerator speedDown(Vector3 hop) {
        while (hop.y > -50f) { //here the arch goes from ~50 to -30
            movePlayer(hop);
            yield return new WaitForSeconds(0.1f); //tune this to strengthen the gravity,
            hop.y -= (float)MoveVel; 
        } Move.updateMovement(Move.CanMove.Freely);
    }
    
    /**
     * <summary>Amplifies the gravity applied onto the player in a logarithmic arch (capped)</summary>
     * <remarks>I have no idea how this works while falling...</remarks>
     */
    private IEnumerator gravAmplifier(Vector3 hop) {
        while (Move.getMove() is not Move.CanMove.Freely) { //here the arch is kept at a downwards angle
            yield return speedDown(hop);
        } 
    }
    
    public void slapPlayerDown() {
        var pBody = getPlayerBody();
        getPlayerBody().velocity = new Vector3(pBody.velocity.x, -5f, pBody.velocity.z);
    }
}
