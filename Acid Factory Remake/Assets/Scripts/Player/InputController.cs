using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
    private static KeyCode lastButtonPressed = KeyCode.D;
    private static KeyCode[] buttons = { KeyCode.A, KeyCode.D, KeyCode.S, KeyCode.W };
    
    /**
     * <summary><para>Evaluates the movement vector of the player</para>
     *  Based on the keys supplied by the currently active gimmick.</summary>
     * <remarks>I wish I could implement this into a switch statement</remarks>
    */
    public static Vector3 move(Vector3 vel) {
        for (var i = 0; i <= 3; i++) {
            if (Input.GetKey(buttons[i]) && Move.getMove() != Move.CanMove.Cant) { //Note: casting to int practically performs a Math.Floor operation
                float velocity = calculateParity(i);
                if (buttons[i].Equals(lastButtonPressed) || !isAscending) {
                    velocity *= (float)(MoveVel * 1.3);
                    lastButtonPressed = buttons[i];
                } else {  //todo have an additional check breaking from the function IF the player is ascending (this formula jiggles the player left-right)
                    velocity *= (float)(MoveVel * 0.8f);
                } vel[i < 2 ? 0 : 2] = velocity;
            } 
        } return vel;
    }

    /**
     * <summary>Toggles between -1 and 1 based on the value given.
     * <para>Starts at 1 when given a 1</para></summary>
     * <param name="i">The index from the pattern that will be returned</param>
     * <remarks>Could be done in a more simple-minded way</remarks>
     */
    private static int calculateParity(double i) {
        return Math.Sign(-Math.Pow(-2, (int)i)); //note, the integer casting is used to divide by zero and get 0 as result. The brackets are important there, as in we wanna divide then cast
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
