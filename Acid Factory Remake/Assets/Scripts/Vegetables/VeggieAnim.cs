using System;
using System.Collections;
using UnityEngine;

public static class VeggieAnim {
    /**
     * <summary>The idea here is to have the carrot pop out of the ground then abruptly stop
     * <para>The carrot is supposed to shoot up then slow slightly (and naturally) before abruptly stopping, all popped out or hidden</para></summary>
     */
    public static IEnumerator animateCarrot(Rigidbody targetCBody, VegetableVisibility.VegState state) {
        var y = 5f;
        var cBodyP = targetCBody.position;
        if (state is VegetableVisibility.VegState.Visible) {
            y *= -1;
        } while (Math.Abs(y) > 1) {
            y = Mathf.Lerp(cBodyP.y, cBodyP.y + y, 0.2f); //todo test this out, the veg jumps out THEN ducks down ... perhaps check the state if the veg is trying to wiggle between states
            targetCBody.position = new Vector3(cBodyP.x, y, cBodyP.z);
            yield return new WaitForSeconds(0.5f);
        } 
    }
}
