using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Script.Tools.ToolType;
using UnityEngine;

/**
 * <date>05/01/2024</date>
 * <author>Gyula Attila Kovacs(gak8)</author>
 * <summary>An extra script that is meant to solely handle inputs</summary>
 */
public class InputController : Character_Controller {
    public static bool itemCoolDown; //true if the cooldown is activated
    
    /**
     * <summary><para>Evaluates the movement vector of the player</para>
     * Based on the keys supplied by the currently active gimmick.</summary>
     * <remarks>I wish I could implement this into a switch statement</remarks>
    */
    public static Vector3 move() {
        var vel = new Vector3(0f, pBody.velocity.y, 0f);
        if (Input.GetKey(KeyCode.A) && Move.getMove() is not Move.CanMove.CantLeft) { //left
            vel.x = -(float)(MoveVel*1.5);
        } if (Input.GetKey(KeyCode.D) && Move.getMove() is not Move.CanMove.CantRight) { //right
            vel.x = (float)(MoveVel*1.5);
        } if (Input.GetKey(KeyCode.W) && Move.getMove() is not Move.CanMove.CantUp) { //up
            vel.z = (float)(MoveVel*1.5);
        } if (Input.GetKey(KeyCode.S) && Move.getMove() is not Move.CanMove.CantDown) { //down
            vel.z = -(float)(MoveVel*1.5);
        } return vel;
    }

    public static bool checkForJump() {
        if (Input.GetKey(KeyCode.Space) && !isAscending) {
            Move.updateMovement(Move.CanMove.CantJump);
            isAscending = true;
            return true;
        } return false;
    }
    
    /**
     * <summary>Checks if the player have pressed the E key</summary>
     */
    public static bool checkForActionButton() {
        return Input.GetKey(KeyCode.E);
    }

    public static bool checkForItemUse() {
        return Input.GetKey(KeyCode.F) && Toolbelt.getBelt().toolInHand /*hand is not null*/ && !itemCoolDown;
    }
}
