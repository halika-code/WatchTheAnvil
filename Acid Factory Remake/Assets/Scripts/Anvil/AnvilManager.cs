using System;
using System.Collections;
using Script.Tools.ToolType;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

/**
 * <date>18/09/2023</date>
 * <author>Gyula Attila Kovacs (gak8)</author>
 * <summary>A collection of function that falls under the animation for the anvil
 * alongside miscellaneous logic needed for the anvil.</summary>
 */

public class AnvilManager : MonoBehaviour {
    private static Transform aManager;
    public static Anvil currentAnvil;
    public static int waitTimer;
    public GameObject preFab;
    private Random rand;

    private void Start() {
        waitTimer = 5;
        rand = new Random();
        aManager = gameObject.transform;
        StartCoroutine(runWait());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /**
     * <summary>This is the main loop of the anvil, where the <see cref="runWait"/> calls this function</summary>
     * <remarks>Manual implementation of a <see cref="Collide.FixedUpdate"/> event-function</remarks>
     */
    private IEnumerator runAnvils() {
        if (currentAnvil != null) {
            yield return new WaitUntil(() => !currentAnvil.isFlying); //check if we need to wait for the last anvil to properly land
        } currentAnvil = new Anvil(Instantiate(preFab, transform, worldPositionStays: true), counterBasedDiff());
        if (!currentAnvil.isFlying) {
            currentAnvil.getAnvilBody().transform.localPosition = Vector3.zero;
            yield return runTimer(); //waits for the anvil then drops it
            yield return runWait();             //runs the "replenishment" wait
        } 
    }
    
    /**
     * <summary>Sets a period where anvils cannot spawn</summary>
     */
    public IEnumerator runWait() {
        while (waitTimer > 0) {
            while (StopWatch.checkWatch()) { //gets caught as soon as stopwatch is in use, does not reach waitTimer but exits as soon as stopwatch is turned off
                yield return new WaitForFixedUpdate();
            } yield return new WaitForSeconds(0.5f); 
            waitTimer--;
        } waitTimer = rand.Next(12, 21) - (int)(getAnvilCount() * 1.5f); //resets the wait timer
        yield return runAnvils(); //keep in mind, execution returns here as soon as the 1st timer starts ticking
    }

    /**
     * <summary>Based on how many anvils there are in the field,
     * adjusts the difficulty of the drop-frequency</summary>
     */
    private static int counterBasedDiff() {
        return getAnvilCount() switch {
            <= 4 => 5, < 7 => 4,
            < 11 => 3, > 14 => 2,
            _ => 1
        };
    }

    /**
     * <summary>Starts a timer, then attempts to murder the player</summary>
     */
    private IEnumerator runTimer() {
        yield return helpRunTimer(3);  //wait until the aTimer is 3
        StartCoroutine(trackPlayer());
        yield return helpRunTimer(0); //spend the rest of the timer
        StartCoroutine(currentAnvil.dropAnvil());
    }

    /**
     * <summary>Runs a Coroutine based timer tailor made for the anvils
     * <para>Terminates when limit matches with anvil.aTimer</para></summary>
     * <remarks>Could be remade with async</remarks>
     */
    public static IEnumerator helpRunTimer(int limit) {
        while (currentAnvil.aTimer != limit) {
            while (StopWatch.checkWatch()) { //will skip over this IF the stopWatch is not in use
                yield return new WaitForFixedUpdate(); //if the StopWatch is in use, wait for a hot second
            } currentAnvil.aTimer--;   
            UI.updateTimer(currentAnvil.aTimer);
            yield return new WaitForSeconds(0.8f);
        }
    }

    /**
     * <summary>Attempts to keep the player locked on target
     * <para>Terminates the seconds the <see cref="Anvil.aTimer"/> is set to 0</para></summary>
     * <remarks>This is a carbon copy of <see cref="ShadowController.followPlayer"/>, could have made it generic if iterators could accept out keyword</remarks>
     */
    private static IEnumerator trackPlayer() {
        while (currentAnvil.aTimer is not 0) {
            if (!StopWatch.checkWatch()) {
                currentAnvil.getTarget().SetActive(ShadowController.findColPoint(out var hit) && hit.collider.gameObject.name is not "DeathPane"); //if the anvil is not too high above ground OR not above a deathpane
                currentAnvil.setTargetPos(new Vector3(hit.point.x, hit.point.y+0.1f, hit.point.z));
            } yield return null;
        } currentAnvil.isFlying = true;
    }

    public static bool isFlyin() {
        return currentAnvil.isFlying;
    }

    private static int getAnvilCount() {
        return aManager.childCount;
    }

    /**
     * <summary>Destroys an instance of the anvil</summary>
     */
    public static void disableAnvil() {
        currentAnvil.isFlying = false;
        Destroy(currentAnvil.getAnvilBody().gameObject);
    }
    
    /**
     * <summary>Stops the Anvil from trying to fall through the floor</summary>
     */
    public static void freezeAnvil() {
        if (isFlyin()) {
            Destroy(currentAnvil.getTarget());
            currentAnvil.getAnvilBody().isKinematic = true;
            currentAnvil.isFlying = false;
        }
    }
}
