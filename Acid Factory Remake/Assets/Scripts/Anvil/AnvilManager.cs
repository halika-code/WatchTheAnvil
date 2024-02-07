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
    }

    /**
     * <summary>This is the main loop of the anvil, where the <see cref="runWait"/> calls this function</summary>
     * <remarks>Manual implementation of a <see cref="Collide.FixedUpdate"/> event-function</remarks>
     */
    private IEnumerator runAnvils() {
        anvilCounter++;
        currentAnvil = new Anvil(Instantiate(preFab, transform, true), 3);
        currentAnvil.getAnvilBody().transform.localPosition = Vector3.zero;
        yield return runTimer(currentAnvil);
        waitTimer = 20 - (int)Math.Ceiling((double)anvilCounter*3/2); //resets the wait timer
        yield return runWait(); 
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
     * <summary>Runs timer for the anvil's built in timer</summary>
     */
    public static IEnumerator helpRunTimer(Anvil anvil, int limit) {
        while (anvil.aTimer != limit) {
            while (StopWatch.checkWatch()) { //will skip over this IF the stopWatch is not in use
                yield return new WaitForFixedUpdate(); //if the StopWatch is in use, wait for a hot second
            } anvil.aTimer--;
            Debug.Log("Anvil droppin in " + anvil.aTimer + " seconds");
            UI.updateTimer(anvil.aTimer);
            yield return new WaitForSeconds(0.8f);
        }
    }

    /**
     * <summary>Attempts to keep the player locked on target</summary>
     * <remarks>This is a carbon copy of <see cref="ShadowController.followPlayer"/>, could have made it generic if iterators could accept out keyword</remarks>
     */
    private static IEnumerator trackPlayer(Anvil anvil) {
        while (anvil.aTimer is not 0) {
            if (!anvil.getTarget().activeSelf) {
                anvil.getTarget().SetActive(true);
            } if (!ShadowController.findColPoint(out var hit) || hit.collider.gameObject.name is "DeathPane") {
                anvil.getTarget().SetActive(false);
            } if (Character_Controller.getParentName(hit.collider.gameObject) is not "Vegetables") {
                anvil.setTargetPos(new Vector3(hit.point.x, hit.point.y+0.1f, hit.point.z));
            } yield return null;
        } anvil.isFlying = true;
    }

    /**
     * <summary>Sets a period where anvils cannot spawn</summary>
     */
    public IEnumerator runWait() {
        while (waitTimer > 0) {
            while (StopWatch.checkWatch()) { //will skip over this IF the stopWatch is not in use
                yield return new WaitForFixedUpdate(); //if the StopWatch is in use, wait for a hot second
            } Debug.Log("Preparing next anvil in " + waitTimer + " seconds");
            yield return new WaitForSeconds(0.5f);
            waitTimer--;
        } yield return runAnvils(); //keep in mind, execution returns here as soon as the 1st timer starts ticking
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
