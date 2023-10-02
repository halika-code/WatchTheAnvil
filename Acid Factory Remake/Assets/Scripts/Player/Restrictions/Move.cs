using UnityEngine;

public class Move : MonoBehaviour {
    
    private static CanMove controller; 
    public enum CanMove {
        Freely,
        CantLeft,
        CantRight,
        CantJump,
        Cant
    }
    
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
