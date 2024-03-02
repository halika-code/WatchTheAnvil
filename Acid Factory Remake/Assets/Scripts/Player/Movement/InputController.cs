using System;
using UnityEngine;
using static GravAmplifier;

/**
 * <date>05/01/2024</date>
 * <author>Gyula Attila Kovacs(gak8)</author>
 * <summary>An extra script that is meant to solely handle inputs</summary>
 */
public class InputController : Character_Controller {
    public static bool itemCoolDown; //true if the cooldown is activated
    private static KeyCode lastButtonPressed = KeyCode.D;
    private static KeyCode[] buttons = { KeyCode.A, KeyCode.D, KeyCode.S, KeyCode.W };
    private static Vector3 lastSpeed = Vector3.zero;
    
    /**
     * <summary><para>Evaluates the movement vector of the player</para>
     *  Based on the keys supplied.</summary>
    */
    public static Vector3 checkForButtonPress(Vector3 vel) {
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
     * note: this function delves into intricate button manipulations, earning it's place in this script
     */
    private static float applyRestriction(int i) {
        float velocity = calculateParity(i);
        Enum.TryParse((i + 1).ToString(), out Move.CanMove restriction); //this finds the restriction
        if (restriction != Move.getMove()) { //restriction is correct, getMove isn't
            velocity = processPlayerSpeed(velocity, i);
        } else { //if the player tries to move towards a direction that is restricted
            if (gravity.getDownwardSpeed() > 0f) {
                gravity.updateDownwardSpeed(-1f); 
            } return 0f;
        } return velocity;
    }

    /**
     * <summary>Assigns a speed to the player based on the player's current state</summary>
     * <param name="velocity">A float that houses the orientation of the vector each key-press has</param>
     * <param name="i">The index denoting which button have been pressed</param>
     * <returns>The finished velocity</returns>
     */
    private static float processPlayerSpeed(float velocity, int i) {
        if (isAscending) { //if the player is soaring
            if (buttons[i].Equals(lastButtonPressed)) { //if the player is pressing the same button, keep a steady speed
                return incrementPlayerSpeed(velocity * flyingVector[i] + 2f, (float)(MoveVel * 1.25));
            } lastButtonPressed = buttons[i]; 
            return velocity; //player switching directions, use dampening from VelocityManipulation
        } if (Math.Abs(flyingVector[i] - velocity * (float)MoveVel) > 0.05f) { //the player is grounded
            flyingVector[i] = velocity * (float)MoveVel + 2f;
        } return incrementPlayerSpeed(flyingVector[i]); 
    }

    /**
     * <summary>Increments then checks the player's speed</summary>
     */
    private static float incrementPlayerSpeed(float desiredSpeed, float limit = (float)MoveVel * 1.5f) { 
        return Math.Abs((int)desiredSpeed) > (int)limit ? Math.Sign(desiredSpeed) * limit : desiredSpeed; //casting to int equals to a Math.Floor statement
    }

    /**
     * <summary>Toggles between -1 and 1 based on the value given.
     * <para>Starts at 1 when given a 1</para></summary>
     * <param name="i">The index from the pattern that will be returned</param>
     * <remarks>Could be done in a simplified way</remarks>
     */
    private static int calculateParity(double i) {
        var ret = i % 2 is not 0 ? 1 : -1;
        return ret; //note, the integer casting is used to divide by zero and get 0 as result. The brackets are important there, as in we wanna divide then cast
    }

    /**
     * <summary>Checks if the player can jump</summary>
     * <returns>false if the player shouldn't jump, true if the player is not in the air and has pressed jump</returns>
     * <remarks>the <see cref="GravAmplifier.isAscending"/> has to be false in order to return a true</remarks>
     */
    public static bool checkForJump() {
        if (Input.GetKey(KeyCode.Space) && !isAscending) {
            toggleToJumpingState();
            return true;
        } //Debug.Log("canFly: " + isAscending + ", Pressed lump: " + Input.GetKey(KeyCode.Space));
        return false;
    }

    public static void toggleToJumpingState() {
        isAscending = true;
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
