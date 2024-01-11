using UnityEngine;

/**
 * <date>17/06/2023</date>
 * <author>Gyula Attila Kovacs (gak8)</author>
 * <summary>A simple class that is used to regulate the movement of the player</summary>
 */
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
