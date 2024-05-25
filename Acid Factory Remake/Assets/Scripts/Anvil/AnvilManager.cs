using System;
using System.Collections;
using Script.Tools.ToolType;
using UnityEngine;

/**
 * <date>18/09/2023</date>
 * <author>Gyula Attila Kovacs (gak8)</author>
 * <summary>A collection of function that falls under the animation for the anvil
 * alongside miscellaneous logic needed for the anvil.</summary>
 */

public class AnvilManager : MonoBehaviour {
    public static Anvil currentAnvil;
    public static int waitTimer;
    private int anvilCounter;
    public GameObject preFab;

    private void Start() {
        waitTimer = 5;
        anvilCounter = 0;
        StartCoroutine(runWait());
        waitTimer = 20 - (int)Math.Ceiling((double)1*3/2); //resets the wait timer
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /**
     * <summary>This is the main loop of the anvil, where the <see cref="runWait"/> calls this function</summary>
     * <remarks>Manual implementation of a <see cref="Collide.FixedUpdate"/> event-function</remarks>
     */
    private IEnumerator runAnvils() {
        if (currentAnvil != null) {
            yield return new WaitUntil(() => !currentAnvil.isFlying); //check if we need to wait for the last anvil to properly land
        } currentAnvil = new Anvil(Instantiate(preFab, transform, worldPositionStays: true), diff: getDifficulty());
        if (!currentAnvil.isFlying) { //todo what it looks like, the waitTime does not reset properly (or gets skipped)
            anvilCounter++;
            currentAnvil.getAnvilBody().transform.localPosition = Vector3.zero;
            yield return runTimer(currentAnvil); //waits for the anvil then drops it
            yield return runWait();             //runs the "replenishment" wait
            waitTimer = 20 - (int)(anvilCounter * 1.5f); //resets the wait timer
        } 
    }

    /**
     * <summary>Based on the waitTimer, decides what difficulty the anvils should spawn with</summary>
     */
    private static int getDifficulty() {
        return waitTimer switch {
            > 10 => 1, > 5 => 2, 
            <  5 => 3,   _ => 0
        }; //this assigns a return value based on the waitTimer using the lambda ... value ... expression (or whatever)
    }

    /**
     * <summary>Starts a timer, then attempts to murder the player</summary>
     */
    private IEnumerator runTimer(Anvil anvil) {
        yield return helpRunTimer(anvil, 3);  //wait until the aTimer is 3
        StartCoroutine(trackPlayer(anvil));
        yield return helpRunTimer(anvil, 0); //spend the rest of the timer
        StartCoroutine(anvil.dropAnvil());
    }

    /**
     * <summary>Runs a Coroutine based timer tailor made for the anvils
     * <para>Terminates when limit matches with anvil.aTimer</para></summary>
     * <remarks>Could be remade with async</remarks>
     */
    public static IEnumerator helpRunTimer(Anvil anvil, int limit) {
        while (anvil.aTimer != limit) {
            while (StopWatch.checkWatch()) { //will skip over this IF the stopWatch is not in use
                yield return new WaitForFixedUpdate(); //if the StopWatch is in use, wait for a hot second
            } anvil.aTimer--;
            UI.updateTimer(anvil.aTimer);
            yield return new WaitForSeconds(0.8f);
        }
    }

    /**
     * <summary>Attempts to keep the player locked on target
     * <para>Terminates the seconds the <see cref="Anvil.aTimer"/> is set to 0</para></summary>
     * <remarks>This is a carbon copy of <see cref="ShadowController.followPlayer"/>, could have made it generic if iterators could accept out keyword</remarks>
     */
    private static IEnumerator trackPlayer(Anvil anvil) {
        while (anvil.aTimer is not 0) {
            anvil.getTarget().SetActive(ShadowController.findColPoint(out var hit) || hit.collider.gameObject.name is not "DeathPane"); //if the anvil is not too high above ground OR not above a deathpane
            if (Character_Controller.getParentName(hit.collider.gameObject) is not "Vegetables") {
                anvil.setTargetPos(new Vector3(hit.point.x, hit.point.y+0.1f, hit.point.z));
            } yield return null;
        } anvil.isFlying = true;
    }

    /**
     * <summary>Sets a period where anvils cannot spawn</summary>
     */
    public IEnumerator runWait() {
        while (waitTimer > 0) {
            while (StopWatch.checkWatch()) { //gets caught as soon as stopwatch is in use, does not reach waitTimer but exits as soon as stopwatch is turned off
                yield return new WaitForFixedUpdate();
            } yield return new WaitForSeconds(0.5f); //else: 
            waitTimer--;
        } Debug.Log("Wait finished, dropping anvil");
        yield return runAnvils(); //keep in mind, execution returns here as soon as the 1st timer starts ticking
    }

    public static bool isFlyin() {
        return currentAnvil.isFlying;
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
