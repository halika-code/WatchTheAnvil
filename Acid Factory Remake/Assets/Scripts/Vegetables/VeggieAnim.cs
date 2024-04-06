using System.Collections;
using UnityEngine;

public static class VeggieAnim {

    private static readonly float[] RaiseHeights = {2.58f*2, 1.33f*2, 0.52f*2, 3.2f}; //big, medium, small veggie and flower
    /**
     * <summary>The idea here is to have the carrot pop out of the ground then abruptly stop
     * <para>The carrot is supposed to shoot up then slow slightly (and naturally) before abruptly stopping, all popped out or hidden</para></summary>
     */
    public static IEnumerator animateCarrot(Rigidbody targetCBody, VegetableVisibility.VegState state) {
        var y = targetCBody.position.y; //original y speed
        for (var i = 6; i > 0; i--) { //todo wanna make it smooth
            targetCBody.position = new Vector3(targetCBody.position.x, y + (getModifier(targetCBody, state) / i), targetCBody.position.z);
            yield return new WaitForSeconds(0.008f);
        } 
    }

    /**
     * <summary>
     * </summary>
     */
    private static float getModifier(Component targetBody, VegetableVisibility.VegState state) {
        switch (Character_Controller.getParentName(targetBody.transform)[0]) {
            case "Large": {
                return state is VegetableVisibility.VegState.Visible ? RaiseHeights[0] * -1 : RaiseHeights[0];
            } case "Medium": {
                return state is VegetableVisibility.VegState.Visible ? RaiseHeights[1] * -1 : RaiseHeights[1];
            } case "Small": {
                return state is VegetableVisibility.VegState.Visible ? RaiseHeights[2] * -1 : RaiseHeights[2];
            } case "Flowers": {
                return state is VegetableVisibility.VegState.Visible ? RaiseHeights[3] * -1 : RaiseHeights[3];
            } default: {
                goto case "Small";
            }
        }
    }
}