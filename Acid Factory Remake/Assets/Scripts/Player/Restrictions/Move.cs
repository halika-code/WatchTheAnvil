using UnityEngine;

/**
 * <date>17/06/2023</date>
 * <author>Gyula Attila Kovacs kuki(gak8)</author>
 * <summary>A simple class that is used to regulate the movement of the player</summary>
 */
public class Move : MonoBehaviour {
    
    private static CanMove controller; 
    
    /**
     * <summary>The main restriction list</summary>
     * <remarks>The corresponding to a given state can be received IF a variable with a given state is cast into an integer</remarks>
     */
    public enum CanMove : int{ 
        Freely,
        CantLeft,
        CantRight,
        CantDown,
        CantUp,
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
