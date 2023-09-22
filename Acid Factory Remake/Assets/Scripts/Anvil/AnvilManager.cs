using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnvilManager : MonoBehaviour {
    private static Anvil currentAnvil;
    private static int waitTimer;
    public GameObject preFab;

    private void Start() {
        waitTimer = 5;
        StartCoroutine(startInitialWait());
    }

    // Update is called once per frame
    private void FixedUpdate() {
        if (waitTimer is 0) {
            currentAnvil = new Anvil(Instantiate(preFab, transform, true), 3);
            StartCoroutine(runTimer(currentAnvil));
            var aCount = getAnvilCount();
            waitTimer = 21 - (int)Math.Ceiling((double)aCount*3/2);
            StartCoroutine(startInitialWait());
        }
    }

    /**
     * <summary>Starts a timer, then attempts to murder the player</summary>
     */
    private IEnumerator runTimer(Anvil anvil) {
        yield return helpRunTimer(anvil, 3);
        StartCoroutine(trackPlayer(anvil));
        yield return helpRunTimer(anvil, 0);
        StartCoroutine(anvil.dropAnvil());
    }

    private static IEnumerator helpRunTimer(Anvil anvil, int limit) {
        while (anvil.aTimer != limit) {
            anvil.aTimer--;
            yield return new WaitForSeconds(0.8f);
            UI.updateTimer(anvil.aTimer);
        }
    }

    /**
     * <summary>Attempts to keep the player locked on target</summary>
     * <remarks>This is a carbon copy of <see cref="ShadowController.findPlatform()"/>, could have made it generic if iterators could accept out keyword</remarks>
     */
    private static IEnumerator trackPlayer(Anvil anvil) {
        if (!anvil.getTarget().activeSelf) {
            anvil.getTarget().SetActive(true);
        } while (anvil.aTimer is not 0) {
            if (!ShadowController.findColPoint(out var hit) || hit.collider.gameObject.name is "DeathPane") {
                anvil.getTarget().SetActive(false);
            } if (Character_Controller.getParentName(hit.collider.transform) is "Platforms") {
                anvil.setTargetPos(new Vector3(hit.point.x, hit.point.y+0.1f, hit.point.z));
            } yield return null;
        } anvil.isFlying = true;
    }

    /**
     * <summary>Sets a period where anvils cannot spawn</summary>
     */
    private static IEnumerator startInitialWait() {
        while (waitTimer > 0) {
            Debug.Log("Preparing next anvil in " + waitTimer + " seconds");
            yield return new WaitForSeconds(1f);
            waitTimer--;
        }
    }

    /**
     * <summary>Returns the amount of Anvils present in the stage</summary>
     * <returns>From 0 to integer limit</returns>
     * <remarks>Should return a count of EVERY anvil littered on the stage</remarks>
     */
    public static int getAnvilCount() {
        return GameObject.Find("Anvils").transform.childCount;
    }

    public static bool isFlyin() {
        return currentAnvil.isFlying;
    }

    public static void disableAnvil() {
        currentAnvil.getTarget().SetActive(false);
        currentAnvil.getAnvilBody().isKinematic = true;
        currentAnvil.isFlying = false;
    }
}
