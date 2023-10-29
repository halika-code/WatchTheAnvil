using System;
using System.Collections;
using UnityEngine;

public static class VeggieAnim {
    /**
     * <summary>The idea here is to have the carrot pop out of the ground then abruptly stop
     * <para>The carrot is supposed to shoot up then slow slightly (and naturally) before abruptly stopping, all popped out or hidden</para></summary>
     */
    public static IEnumerator animateCarrot(Rigidbody targetCBody, VegetableVisibility.VegState state) {
        var y = targetCBody.position.y;
        var modifier = 5f;
        if (state is VegetableVisibility.VegState.Visible) {
            modifier *= -1;
        } for (var i = 1; i < 7; i++) {
            //Debug.Log("The y value of " + targetCBody.name + " is " + y); 
            targetCBody.position = new Vector3(targetCBody.position.x, 
                Mathf.Lerp(y, y + modifier, 0.08f * i), targetCBody.position.z); 
            yield return null;
        } 
    }
}