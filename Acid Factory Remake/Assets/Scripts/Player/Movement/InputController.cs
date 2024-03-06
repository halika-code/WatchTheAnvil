using System;
using UnityEngine;
using static GravAmplifier;
using static VelocityManipulation;

/**
 * <date>05/01/2024</date>
 * <author>Gyula Attila Kovacs(gak8)</author>
 * <summary>An extra script that is meant to solely handle inputs</summary>
 */
public class InputController : Character_Controller {
    public static bool itemCoolDown; //true if the cooldown is activated
    private static KeyCode lastButtonPressed = KeyCode.Z;
    private static KeyCode[] buttons = { KeyCode.A, KeyCode.D, KeyCode.S, KeyCode.W };
    
    /**
     * <summary><para>Evaluates the movement vector of the player</para>
     *  Based on the keys supplied.</summary>
     * <returns>A set of velocity the player will go with IF the player is grounded</returns>
     */
    public static Vector3 checkForButtonPress() {
        var vel = pBody.velocity;
        for (var i = 0; i <= 3; i++) {
            if (Input.GetKey(buttons[i]) && Move.getMove() != Move.CanMove.Cant) { //Note: casting to int practically performs a Math.Floor operation
                vel[i < 2 ? 0 : 2] = applyRestriction(i);
                if (shouldUpdateButton(i)) {
                    Debug.Log("updating Button");
                    updateButtonPress(i);
                }
            } 
        } return vel;
    }

    /**
     * The idea here is, this function should only return true if the player's speed matches with the angle calculated from the i variable
     * and the speed is 50% of the MoveVel, otherwise return false
     */
    private static bool shouldUpdateButton(int i) {
        return isAscending && lastButtonPressed != buttons[i] && Math.Abs(pBody.velocity[i < 2 ? 0 : 2]) > MoveVel * 0.8; //if the player's angle is the same as the button's AND the player's speed reaches 80% of the max speed
    }

    private static float dampenVelocity(int i) { //todo this function needs to have an async Task.Delay into it with a small number
        Debug.Log("Dampening Velocity");
        if (lastButtonPressed != buttons[i]) {
            i += 4; //added 4 instead of 3 to account to the int being 0, 0+3=3 which is in the bounds of expected values, will not trigger the reset
        } return VelocityManipulation.dampenVelocity(i);
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
            velocity = processPlayerSpeed(velocity, i < 2 ? 0 : 2);
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
            return buttons[i].Equals(lastButtonPressed) ? //if the player is pressing the same button, keep a steady speed
                incrementPlayerSpeed(velocity * (float)(MoveVel * 1.25)) : dampenVelocity(i); //player switching directions, use dampening from VelocityManipulation
        } return incrementPlayerSpeed(velocity * (float)MoveVel + 2f);  //todo check why the dampenVelocity doesn't work
    }

    /**
     * <summary>Toggles between -1 and 1 based on the value given.
     * <para>Starts at 1 when given a 1</para></summary>
     * <param name="i">The index from the pattern that will be returned</param>
     * <remarks>Could be done in a simplified way</remarks>
     */
    public static int calculateParity(double i) {
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

    /**
     * <summary>Updates the button to a valid index of the buttons array</summary>
     * <remarks>If an incorrect index is supplied, the lastButtonPressed will be set to KeyCode.Z</remarks>
     */
    public static void updateButtonPress(int index) {
        lastButtonPressed = index > 0 && index < buttons.Length ? buttons[index] : KeyCode.Z;
    }
}
