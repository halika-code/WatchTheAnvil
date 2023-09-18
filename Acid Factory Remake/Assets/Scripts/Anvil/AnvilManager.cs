using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Anvil;

public class AnvilManager : MonoBehaviour {
    private Anvil currentAnvil;
    private static int waitTimer;
    
    private void OnEnable() {
        setTarget(GetComponentInChildren<Rigidbody>());
        waitTimer = 20;
    }

    private void Start() {
        currentAnvil = new Anvil();
        StartCoroutine(startInitialWait());
    }

    // Update is called once per frame
    private void Update() {
        //todo when waitTimer is done, start spawning anvils then revert waitTimer. Perhaps after like 5 times of this, difficulty could be raised
    }
    
    /**
     * <summary>Defines the timer based on the difficulty</summary>
     * <param name="diff">An integer of difficulty ranging from 0 to 3 (and beyond)</param>
     * <param name="diffNum">An embedded return value that returns the diff variable</param>
     * <remarks>the out keyword is used here as an exercise, this use is not optimal</remarks>
     */
    public static int setTimer(int diff) {
        switch (diff) {
            case 0 or 1: {
                return 20;
            } case 2: {
                return 10;
            } case <= 3: {
                return 5;
            } default: {
                Debug.Log("Whoopy while setting the timer for the Anvil with a difficulty of " + diff);
                return 1;
            }
        }
    }
    
    /**
     * <summary>Attempts to keep the player locked on target</summary>
     * <remarks>This is a carbon copy of <see cref="ShadowController.findPlatform()"/>, could have made it generic if iterators could accept out keyword</remarks>
     */
    public static IEnumerator trackPlayer() {
        if (!getTarget().activeSelf) {
            getTarget().SetActive(true);
        } while (true) {
            if (!ShadowController.findColPoint(out var hit) || hit.collider.gameObject.name is "DeathPane") {
                getTarget().SetActive(false);
            } if (Character_Controller.getParentName(hit.collider.transform) is "Platforms") {
                setTargetPos(new Vector3(hit.point.x, hit.point.y+0.1f, hit.point.z));
            } yield return null;
        }
    }

    /**
     * <summary>Acts as a self-contained timer, stops this function for a bit</summary>
     * <param name="endTime">The amount of cycles the function should loop (based on how much there is on the timer)</param>
     * <param name="delay">The amount of seconds the cycle should rest</param>
     * <param name="timer"></param>
     */
    public static IEnumerator genericTimer(int timer, int endTime, WaitForSeconds delay) {
        while (timer > endTime) {
            yield return delay;
            timer--;
        }
    }

    
    /**
     * <summary>Sets a period where anvils cannot spawn</summary>
     */
    private static IEnumerator startInitialWait() {
        yield return genericTimer(waitTimer, 0,  new WaitForSeconds(1f));
    }

}
