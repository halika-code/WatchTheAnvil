using System;
using System.Linq;
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
    private static KeyCode? lastButtonPressed;
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
                if (getJumpingState(i, out var flyingState) && flyingState is not 2) {  
                    Debug.Log("updating Button");       //todo add a waiting function to updateButtonPress when the flyingState is not 1
                    updateButtonPress(i);
                }
            } 
        } return vel;
    }

    /**
     * <summary>Decides if the lastPressedButton needs to be updated or not</summary>
     * <param name="i">The index the button is pressed in the <see cref="buttons"/>array</param>
     * <param name="flyingState">An additional returned variable that houses information about the player's button press.
     * <para>Sets to 0 by default</para>
     * Sets to 1 when the player moves after a stationary jump
     * <para>Sets to 2 when the player presses a direction mapped to a different axis</para>
     * Sets to 3 when the player presses a direction in the opposite direction but the same axis</param>
     * <returns>True, if the player makes a stationary jump then moves,
     * <para>If the player tries to strafe in the air or</para>
     * If the player have been moving in the opposite direction for long enough</returns>
     * Note: this doesn't specifically needs an out var flyingState BUT I feel like it might gonna be useful later
     */
    private static bool getJumpingState(int i, out int flyingState) {
        switch (isAscending) {
            default: {
                flyingState = 0;
                return false;
            } case true when lastButtonPressed == null: {  //stationary jump then movement
                flyingState = 1;
                return true;
            } case true when lastButtonPressed != buttons[i] && !wasOppositePressed(i): { //air-strafing
                flyingState = 2;
                return true;
            } case true when lastButtonPressed != buttons[i] && pBody.velocity[i < 2 ? 0 : 2] > MoveVel * 0.8: { //opposite air movement,
                flyingState = 3;
                return true; //note, when this statement is checked against, velocity dampening is expected to be making the movement
            }
        } 
    } 
    
    /**
     * <summary>Checks if the opposite key have been pressed</summary>
     * <returns>True, if the player presses a button that is mapped to the opposite parity of a given axis
     * <para>False if the player presses the same button or a button assigned to a different axis</para></returns>
     * <example>Scenario: The player presses 'A' after 'W':
     * <para>An i of value of 3 is given while the lastButtonPressed is KeyCode.A. The lastButtonPressed is converted into an index using <see cref="getLastButtonIndex"/>
     * and the value of 3 is compared against a value of 0.</para>
     * For the return condition, the two numbers are checked to not be the same (1st condition: true), and to be equal when 1 * (-1)
     * is added to the i variable, to arrive at the expected opposite value: 2 (2nd condition: false)
     * </example>
     * <remarks> Example uses default button mappings</remarks>
     */
    private static bool wasOppositePressed(int i) { //this here is a delegate creation with a variable named code of type KeyCode,
        if (lastButtonPressed == null) {
            return true;
        } return getLastButtonIndex() != i && i + calculateParity(i) * -1 == getLastButtonIndex(); 
    } //note at that -1 == buttons: the buttons is used to get the INDEX where the lastButtonPressed is kept at inside the array

    /**
     * <summary>Converts a verified key-press into a restriction applicable to the direction the player is heading
     * <para>Checks the converted restriction against the one applied to the player</para>
     * If a match is found, the player is deemed to be colliding into a wall, the input will be dropped</summary>
     * <param name="i">the index pointing to the key pressed from the <see cref="buttons"/> array</param>
     * <remarks>The index supplied is expected to correspond to the placement of the key pressed</remarks>
     * note: this function delves into intricate button manipulations, earning it's place in this script
     */
    private static float applyRestriction(int i) {
        var parity = calculateParity(i);
        float velocity;
        Enum.TryParse((i + 1).ToString(), out Move.CanMove restriction); //this finds the restriction
        if (restriction != Move.getMove()) { //restriction is correct, getMove isn't
            velocity = processPlayerSpeed(parity, i);
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
            if (!buttons[i].Equals(lastButtonPressed)) {
                Extras.runTimer(0.5f);
                Debug.Log("running timer"); //todo test if this actually stops as intended
                return pBody.velocity[i < 2 ? 0 : 2] + (velocity * (float)(MoveVel * 1.25) / 5); //dampening
            } return velocity * (float)(MoveVel * 1.25); //dropping faster
        } return incrementPlayerSpeed(velocity * (float)MoveVel + 2f); //moving normal
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
        lastButtonPressed = index >= 0 && index < buttons.Length ? buttons[index] : null;
    }

    /**
     * <summary>Finds the index of the lastButtonPressed inside the buttons array</summary>
     * <returns>The index of the KeyCode corresponding to the lastButtonPressed</returns>
     * <remarks>This implementation uses a delegate to find the index of the lastButtonPressed within the buttons array</remarks>
     */
    public static int getLastButtonIndex() {
        return buttons.ToList().FindIndex(code => code.Equals(lastButtonPressed));
    }
}
