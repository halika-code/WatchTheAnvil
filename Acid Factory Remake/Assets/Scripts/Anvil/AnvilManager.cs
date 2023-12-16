using System;
using System.Collections;
using System.Threading.Tasks;
using Script.Tools.ToolType;
using UnityEngine;

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
        yield return runTimer(currentAnvil);
        waitTimer = 20 - (int)Math.Ceiling((double)anvilCounter*3/2);
        StartCoroutine(runWait()); 
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
            while (StopWatch.stopWatchInUse) { //will skip over this IF the stopWatch is not in use
                yield return new WaitForFixedUpdate(); //if the StopWatch is in use, wait for a hot second
            }
            anvil.aTimer--;
            Debug.Log("Anvil droppin in " + anvil.aTimer + " seconds");
            yield return new WaitForSeconds(0.8f);
            UI.updateTimer(anvil.aTimer);
        }
    }

    /**
     * <summary>Attempts to keep the player locked on target</summary>
     * <remarks>This is a carbon copy of <see cref="ShadowController.findPlatform()"/>, could have made it generic if iterators could accept out keyword</remarks>
     */
    private static IEnumerator trackPlayer(Anvil anvil) {
        while (anvil.aTimer is not 0) {
            if (!anvil.getTarget().activeSelf) {
                anvil.getTarget().SetActive(true);
            } if (!ShadowController.findColPoint(out var hit) || hit.collider.gameObject.name is "DeathPane") {
                anvil.getTarget().SetActive(false);
            } if (Character_Controller.getParentName(hit.collider.transform) is not "Vegetables") {
                anvil.setTargetPos(new Vector3(hit.point.x, hit.point.y+0.1f, hit.point.z));
            } yield return null;
        } anvil.isFlying = true;
    }

    /**
     * <summary>Sets a period where anvils cannot spawn</summary>
     */
    public IEnumerator runWait() {
        while (waitTimer > 0) {
            while (StopWatch.stopWatchInUse) { //will skip over this IF the stopWatch is not in use
                yield return new WaitForFixedUpdate(); //if the StopWatch is in use, wait for a hot second
            }
            Debug.Log("Preparing next anvil in " + waitTimer + " seconds");
            yield return new WaitForSeconds(0.5f);
            waitTimer--;
        } StartCoroutine(runAnvils()); //keep in mind, execution returns here as soon as the 1st timer starts ticking
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
