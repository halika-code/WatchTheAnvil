using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetableVisibility : MonoBehaviour {
    private static VegState controller;
    public enum VegState {
        Visible,
        Hidden
    }

    private void OnEnable() {
        controller = VegState.Hidden;
    }

    public static VegState getVisibleVeg() {
        return controller;
    }

    public static void updateState(VegState state) {
        controller = state;
    }
}
