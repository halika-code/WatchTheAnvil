using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {
    public enum CanMove {
        Freely,
        Left,
        Right,
        Jump,
        No
    }

    private static CanMove controller; 
    // Start is called before the first frame update
    private void OnEnable() { 
        controller = CanMove.Freely;
    }

    public static void updateMovement(CanMove restriction) {
        controller = restriction;
    }

    /**
     * Returns the currently registered enum for movement
     */
    public static CanMove getMove() {
        return controller;
    }
}
