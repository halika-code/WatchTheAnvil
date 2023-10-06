using System.Collections;
using UnityEngine;

public static class VeggieAnim{
    /**
     * <summary>The idea here is to have the carrot pop out of the ground then abruptly stop
     * <para>The carrot is supposed to shoot up then slow slightly (and naturally) before abruptly stopping, all popped out or hidden</para></summary>
     */
    public static IEnumerator animateCarrot(Rigidbody targetCBody, VegetableVisibility.VegState state) {
        var y = 10f;
        var cBodyP = targetCBody.position;
        if (state is VegetableVisibility.VegState.Hidden) {
            y *= -1;
        } while (y > 1) { //todo check if I have the right idea, then fine-tune this condition to terminate when the y will not be too slow
            y = Mathf.Lerp(cBodyP.y, cBodyP.y + y, 0.5f);
            targetCBody.position = new Vector3(cBodyP.x, y);
            yield return new WaitForSeconds(0.5f);
        } 
    }
}
