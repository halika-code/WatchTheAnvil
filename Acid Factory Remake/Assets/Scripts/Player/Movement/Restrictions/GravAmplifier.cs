using System;
using System.Collections;
using UnityEngine;
using static Character_Controller;

/**
 * <date>11/01/2024</date>
 * <author>Gyula Attila Kovacs</author>
 * <summary>Amplifies the gravity only when the player is deemed to be falling</summary>
 */

public class GravAmplifier : MonoBehaviour {
    public static GravAmplifier gravity;
    private float desiredSpeedCap = -70f;
    /**
     * <summary>Waits for 0.1 second</summary>
     * <remarks>Used to not have to create a new WaitForSeconds during gravity amplification</remarks>
     */
    private static readonly WaitForSeconds Wait = new(0.1f);
    /**
     * <summary>A variable that keeps the calculated speed the player will inherit from the amplified gravity.
     * Only the y component can be modified outside the script</summary>
     * <remarks>Use <see cref="updateDownwardSpeed"/> to modify its speed while the coroutine is running</remarks>
     */
    private static Vector3 hop;
    /**
     * <summary>A boolean flag that keeps track if the player is air-borne</summary>
     * <remarks>Set to true the frame when the game registers a space bar press, set to false when the player hits a PLATFORM</remarks>
     */
    public static bool isAscending; //note: is set to true in the checkForJump function.

    public void OnEnable() {
        gravity = GameObject.Find("Player").GetComponentInChildren<GravAmplifier>();
        isAscending = false;
        hop = new Vector3();
    }

    /**
     * <summary>Starts <see cref="gravAmplifier"/> as a coroutine</summary>
     * note: function needed here for JumpController to reach gravAmplifier
     */
    public void falling(Vector3 velocity, float desiredSpeed = -70f) {
        hop = velocity;
        StartCoroutine(gravAmplifier(desiredSpeed)); //idea here is to have the gravity work specifically when the player is not jumping
    }
    
    /**
     * <summary>Amplifies the gravity applied onto the player in a logarithmic arch (capped) by slowly increasing the velocity applied on the player</summary>
     * <param name="hop">The starting speed that will be given to the player</param>
     * <param name="desiredSpeed">The cap the player should not exceed while falling. By default it is set to 70f</param>
     * <remarks>Coroutine falls out when the <see cref="Character_Controller.isAscending"/> flag is cleared (false)</remarks>
     */
    private IEnumerator gravAmplifier(float desiredSpeed = -70f) {
        desiredSpeedCap = desiredSpeed;
        while (isAscending) { //here the arch is kept at a downwards angle
            movePlayer(hop.y);
            if (Math.Abs(hop.y - desiredSpeedCap) > 0.1f) {
                if (hop.y >= desiredSpeedCap) { //normal execution
                    hop.y -= (float)MoveVel / 1.2f;
                } else {
                    hop.y = desiredSpeedCap;
                }
            } yield return Wait; //waits a bit to apply the speed reduction, producing an arch
        } 
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /**
     * <summary>Updates the currently applied speed-cap (thus toggling the terminal velocity)</summary>
     * <param name="state">False for strong (-70f), true for low (-10f)</param>
     */
    public void updateSpeedCap(bool state) {
        if (isAscending) { //if the player is soaring
            desiredSpeedCap = state ? -10f : -70f;
        } else {
            Debug.Log("Hmmm, that did nothing ... nice umbrella though");
        }
    }

    /**
     * <summary>Updates the y speed only for the currently running gravity amplifier coroutine</summary>
     */
    public void updateDownwardSpeed(float ySpeed) {
        hop.y = ySpeed;
    }

    public float getDownwardSpeed() {
        return hop.y;
    }
}
