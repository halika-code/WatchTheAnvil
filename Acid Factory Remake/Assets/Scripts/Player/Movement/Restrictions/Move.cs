using UnityEngine;

/**
 * <date>17/06/2023</date>
 * <author>Gyula Attila Kovacs kuki(gak8)</author>
 * <summary>A simple class that is used to regulate the movement of the player</summary>
 */
public class Move : MonoBehaviour {
    
    private static CanMove controller; 
    
    /**
     * <summary>A movement restriction list of states that will stop the player from being able to register movement</summary>
     * <remarks>Mainly used for collision detection and movement logic</remarks>
     * Note: each state is used in different scenarios,
     * Rider just can't find the gray states being named specifically (those are used indirectly in InputController)
     */
    public enum CanMove : int{ 
        /**
         * <summary>Flag denoting the player has free movement</summary>
         */
        Freely,
        /**
         * <summary>Flag denoting the player hit a wall from the right</summary>
         */
        CantLeft,
        /**
         * <summary>Flag denoting the player looks at a wall from the left</summary>
         */
        CantRight,
        /**
         * <summary>Flag denoting the player collides with a wall from behind</summary>
         */
        CantDown,
        /**
         * <summary>Flag denoting the player hugs a wall from the front</summary>
         */
        CantUp,
        /**
         * <summary>Flag denoting the player is in a cutscene / transition state</summary>
         */
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
