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
     * <summary>Starts <see cref="gravAmplifier"/> as a coroutine</summary>
     */
    public void falling(Vector3 hop, float desiredSpeed) {
        StartCoroutine(gravAmplifier(hop, desiredSpeed)); //idea here is to have the gravity work specifically when the player is not jumping
    }
    
    /**
     * <summary>Amplifies the gravity applied onto the player in a logarithmic arch (capped) by slowly increasing the velocity applied on the player</summary>
     * <param name="hop">The starting speed that will be given to the player</param>
     * <param name="desiredSpeed">The cap the player should not exceed while falling</param>
     * <remarks>Coroutine falls out when the <see cref="Character_Controller.isAscending"/> flag is cleared (false)</remarks>
     */
    private IEnumerator gravAmplifier(Vector3 hop, float desiredSpeed) {
        while (Move.getMove() is Move.CanMove.CantJump || isAscending) { //here the arch is kept at a downwards angle
            movePlayer(hop);
            yield return null;
            if (hop.y > desiredSpeed) { 
                hop.y -= (float)MoveVel;
                yield return new WaitForSeconds(0.1f); //waits a bit to apply the speed reduction, producing an arch
            }
        } Move.updateMovement(Move.CanMove.Freely);
    }
}
