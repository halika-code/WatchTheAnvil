using System;
using System.Collections.Generic;
using UnityEngine;

public class RootVeg : MonoBehaviour {
    protected static List<Rigidbody> cBodyCollective;
    protected static List<VegetableVisibility.VegState> vegStateCollective;

    private void Start() {
        cBodyCollective = new List<Rigidbody>();
        vegStateCollective = new List<VegetableVisibility.VegState>();
    }

    /**
     * <summary>Initializes all the crucial variables</summary>
     * <param name="bCol">The array containing the fresh vegetables</param>
     * <remarks>If the bCol is fed as null, the function terminates immediately
     * <para>The bCol is assumed to be new and not existing already in the cBodyCollective</para></remarks>
     */
    protected static void init(Rigidbody[] bCol) {
        if (bCol.Length is 0) { //idea here is if there is no vegetables under this parent script, the script should disable
            return;
        } foreach (var bodyCollective in bCol) {
            cBodyCollective.Add(bodyCollective);
        } for (var i = 0; i < bCol.Length; i++) {
            vegStateCollective.Add(VegetableVisibility.VegState.Hidden);
        }
    }
}
