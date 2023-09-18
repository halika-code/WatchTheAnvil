using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AnvilManager;

public class Anvil {
    private static int aTimer;
    private static Rigidbody aShadow;

    /**
     * <summary>A default constructor for an Anvil object
     * <para>that has a default difficulty of 1</para></summary>
     */
    public Anvil() {
        aTimer = setTimer(1);
    }

    /**
     * <summary>A modified constructor for an Anvil object
     * <para>that has a definable difficulty</para></summary>
     * <param name="diff">The desired difficulty ranging from 0 to 3 (and above)</param>
     * <remarks>a difficulty score of 4 and above will be handled as 3 while a value of -1 and less will set the difficulty to impossible</remarks>
     */
    public Anvil(int diff) {
        aTimer = setTimer(diff);
    }

    /**
     * <summary>Sends the Anvil barrelling down towards the player</summary>
     */
    private static void dropAnvil() {
        var tarVel = aShadow.velocity;
        aShadow.velocity = new Vector3(tarVel.x, -100f, tarVel.z);
    }

    /**
     * <summary>Starts a timer, then attempts to murder the player</summary>
     */
    private IEnumerator runTimer(MonoBehaviour runtime, WaitForSeconds delay) {
        yield return genericTimer(aTimer, 3, delay);
        runtime.StartCoroutine(trackPlayer());
        yield return genericTimer(aTimer, 0, delay);
        dropAnvil();
    }

    public static GameObject getTarget() {
        return aShadow.gameObject;
    }
    
    public static void setTarget(Rigidbody shadow) {
        aShadow = shadow;
    }
    
    public static void setTargetPos(Vector3 pos) {
        aShadow.position = pos;
    }

}
