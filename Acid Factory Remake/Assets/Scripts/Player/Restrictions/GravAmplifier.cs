using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Script.Tools.ToolType;
using UnityEngine;
using static Character_Controller;

public class GravAmplifier : MonoBehaviour {

    public static GravAmplifier gravity;

    public void OnEnable() {
        gravity = GameObject.Find("Player").GetComponentInChildren<GravAmplifier>();
    }

    /**
     * <summary>Handles the player's y velocity for the whole duration of the player's flight</summary>
     * <remarks>it has a decent arch</remarks>
     */
    public void flying() {
        Debug.Log("flyin");
        var hop = new Vector3(getPlayerBody().velocity.x, (float)(MoveVel / 12f), getPlayerBody().velocity.z);
        movePlayer(hop);
        falling(hop);
    }

    /**
     * <summary>Calculates the falling velocity during the downward arch before reaching terminal (desired) velocity</summary>
     */
    public void falling(Vector3 hop) {
        if (!(Toolbelt.getBelt().checkForTool("Umbrella", out var umbrella) && ((Umbrella)umbrella).checkIfOpen())) { 
            gravAmplifier(hop); //idea here is to have the gravity work specifically when the player is not jumping
        }
    }

    private async void speedDown(Vector3 hop) {
        while (hop.y > -50f) { //here the arch goes from ~50 to -30
            await Task.Delay(200); //this is needed with the time being optimal
            Debug.Log(getPlayerBody().velocity.y + ", p: " + hop.y);
            hop.y -= (float)MoveVel;
            movePlayer(hop);
        }
    }
    
    /**
     * <summary>Amplifies the gravity applied onto the player in a logarithmic arch (capped)</summary>
     * <remarks>I have no idea how this works while falling...</remarks>
     */
    private async void gravAmplifier(Vector3 hop) { //todo note: hop has a high value,
                                                    //that value supposed to go slowly down, then reach terminal velocity around -50f.
                                                    //note: what it looks like, an infinite loop is being created
        Debug.Log("move is: " + Move.getMove());
        while (Move.getMove() is not Move.CanMove.Freely) { //here the arch is kept at a downwards angle
            Debug.Log("tryin to speed down, y vel is " + hop.y);
            await Task.Run(() => speedDown(hop));
            Debug.Log("speedin down after speedDown"); //todo try perhaps removing the await above to have speedDown run it's course. 
            movePlayer(hop);
            await Task.Delay(5);
        }
    }
    
    public void slapPlayerDown() {
        var pBody = getPlayerBody();
        getPlayerBody().velocity = new Vector3(pBody.velocity.x, -5f, pBody.velocity.z);
    }
}
