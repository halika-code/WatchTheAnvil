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
     *  Based on the keys supplied.</summary>
     * <remarks>I wish I could implement this into a switch statement</remarks>
    */
    public static Vector3 move(Vector3 vel) {
        for (var i = 0; i <= 3; i++) {
            if (Input.GetKey(buttons[i]) && Move.getMove() != Move.CanMove.Cant) { //Note: casting to int practically performs a Math.Floor operation
                vel[i < 2 ? 0 : 2] = applyRestriction(i);
            } 
        } return vel;
    }

    /**
     * <summary>Converts a verified key-press into a restriction applicable to the direction the player is heading
     * <para>Checks the converted restriction against the one applied to the player</para>
     * If a match is found, the player is deemed to be colliding into a wall, the input will be dropped</summary>
     * <param name="i">the index pointing to the key pressed from the <see cref="buttons"/> array</param>
     * <remarks>The index supplied is expected to correspond to the placement of the key pressed</remarks>
     */
    private static float applyRestriction(int i) {
        float velocity = calculateParity(i);
        Enum.TryParse((i + getRestrictionAxis((int)velocity)).ToString(), out Move.CanMove restriction); //this finds the restriction todo check if it is in the correct value or runs out
        //var asd = Move.getMove(); 
        if (restriction != Move.getMove()) {
            if (!buttons[i].Equals(lastButtonPressed) || !isAscending) {
                velocity *= (float)(MoveVel * 1.3);
                lastButtonPressed = buttons[i];
            } else {
                velocity *= (float)(MoveVel * 0.8f);
            } 
        } else {
            return 0f;
        } return velocity;
    }

    /**
     * <summary>Toggles between -1 and 1 based on the value given.
     * <para>Starts at 1 when given a 1</para></summary>
     * <param name="i">The index from the pattern that will be returned</param>
     * <remarks>Could be done in a simplified way</remarks>
     */
    private static int calculateParity(double i) {
        var ret = Math.Sign(-Math.Pow(-2, (int)i));
        return ret; //note, the integer casting is used to divide by zero and get 0 as result. The brackets are important there, as in we wanna divide then cast
    }

    /**
     * <param name="i">The number in question</param>
     * <returns>Based on the parity of the <see cref="i"/> supplied, 1 if positive, 2 if negative</returns>
     * <remarks>Can deal with 0, will return a 2</remarks>
     */
    private static int getRestrictionAxis(int i) {
        return Math.Sign(i) is 1 ? 1 : 2;
    }

    public static bool checkForJump() {
        if (Input.GetKey(KeyCode.Space) && !isAscending) {
            Move.updateMovement(Move.CanMove.CantJump);
            isAscending = true;
        } return isAscending && Input.GetKey(KeyCode.Space);
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
