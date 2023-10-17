using System.Collections.Generic;
using UnityEngine;

public static class RootVeg {
    private static List<Rigidbody> cBodyCollective;
    private static List<VegetableVisibility.VegState> vegStateCollective;

    public static void init(bool forceReset) {
        if (cBodyCollective == null || forceReset) {
            cBodyCollective = new List<Rigidbody>();
            vegStateCollective = new List<VegetableVisibility.VegState>();
        }
    }

    /**
     * <summary>Appends to the end of the list the new variables</summary>
     * <param name="bCol">The array containing the fresh vegetables</param>
     * <param name="terminate">an extra bit denoting if the script accessing this function should destroy itself</param>
     * <remarks>If the bCol is fed as null, the function terminates immediately
     * <para>The bCol is assumed to be new and not existing already in the cBodyCollective</para></remarks>
     */
    public static void init(Rigidbody[] bCol, out bool terminate) {
        terminate = bCol.Length is 0; //idea here is if there is no vegetables under this parent script, the script should disable
        foreach (var bodyCollective in bCol) {
            cBodyCollective.Add(bodyCollective);
        } for (var i = 0; i < bCol.Length; i++) {
            vegStateCollective.Add(VegetableVisibility.VegState.Hidden);
        }
    }

    /**
     * <summary>Gets the collection of veggie bodies</summary>
     * <remarks>Loops once if the body collective isn't initialized properly</remarks>
     */
    public static List<Rigidbody> getBodyCollective() {
        return cBodyCollective;
    }

    /**
     * <summary>Gets the collection of veggie bodies</summary>
     * <remarks>Loops once if the body collective isn't initialized properly</remarks>
     */
    public static List<VegetableVisibility.VegState> getVegStates() { 
        return vegStateCollective;
    }

    public static void updateCollective(int index, VegetableVisibility.VegState state) {
        vegStateCollective[index] = state;
    }
}
