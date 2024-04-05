using System.Collections;
using UnityEngine;

public static class VeggieAnim {

    private static readonly float[] RaiseHeights = {2.58f, 1.33f, 0.52f, 3.2f}; //big, medium, small veggie and flower
    /**
     * <summary>The idea here is to have the carrot pop out of the ground then abruptly stop
     * <para>The carrot is supposed to shoot up then slow slightly (and naturally) before abruptly stopping, all popped out or hidden</para></summary>
     */
    public static IEnumerator animateCarrot(Rigidbody targetCBody, VegetableVisibility.VegState state) {
        var asd2 = Character_Controller.getParentName(targetCBody.transform);
        var asd = getModifier(targetCBody, state);
        var y = targetCBody.position.y; //original y speed
        for (var i = 8; i > 0; i--) {
            targetCBody.transform.position = new Vector3(targetCBody.position.x, y + getModifier(targetCBody, state), targetCBody.position.z); 
            yield return new WaitForSeconds(0.008f); //todo the modifier still doesn't work, the position is too small for some reason
        } 
    }

    /**
     * <summary> todo the idea here is to have the modifier above be set to the corresponding value
     * (which is the amount the vegetable (given it's size) should move up/down)
     * todo also add a case for flower (see how that can be done)
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