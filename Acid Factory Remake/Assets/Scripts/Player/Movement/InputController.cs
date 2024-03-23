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
    protected static KeyCode? lastButtonPressed;
    protected static readonly KeyCode[] Buttons = { KeyCode.A, KeyCode.D, KeyCode.S, KeyCode.W };
    
    /**
     * <summary><para>Evaluates the movement vector of the player</para>
     *  Based on the keys supplied.</summary>
     * <returns>A set of velocity the player will go with IF the player is grounded</returns>
     */
    public static Vector3 checkForButtonPress() {
        var foundButton = false;
        var vel = pBody.velocity;
        for (var i = 0; i <= 3; i++) {
            if (Input.GetKey(Buttons[i]) && Move.getMove() != Move.CanMove.Cant) { //Note: casting to int practically performs a Math.Floor operation
                foundButton = true;
                vel[i < 2 ? 0 : 2] = applyRestriction(i);
                if (JumpController.getJumpingState(i, out var flyingState) && flyingState is not 2) {  
                    updateButtonPress(i);
                }
            } 
        } if (!foundButton && !isAscending) { //if the player have not pressed a button AND is grounded
            vel.Set(vel.x is 0 ? 0 : vel.x * 0.95f, vel.y, vel.z is 0 ? 0 : vel.z * 0.95f); //note: this speed gives the perfect glide on the ground
        } return vel;
    }

    /**
     * <summary>Converts a verified key-press into a restriction applicable to the direction the player is heading
     * <para>Checks the converted restriction against the one applied to the player</para>
     * If a match is found, the player is deemed to be colliding into a wall, the input will be dropped</summary>
     * <param name="i">the index pointing to the key pressed from the <see cref="Buttons"/> array</param>
     * <remarks>The index supplied is expected to correspond to the placement of the key pressed</remarks>
     * note: this function delves into intricate button manipulations, earning it's place in this script
     */
    private static float applyRestriction(int i) {
        var parity = calculateParity(i);
        float velocity;
        Enum.TryParse((i + 1).ToString(), out Move.CanMove restriction); //this finds the restriction
        if (restriction != Move.getMove()) { //normal movement
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
            if (!Buttons[i].Equals(lastButtonPressed)) {
                return incrementPlayerSpeed(pBody.velocity[i < 2 ? 0 : 2] + velocity * ((float)MoveVel / 17.1f)); //dampening
            } return velocity * (float)(MoveVel * 1.25); //dropping faster
        } return incrementPlayerSpeed(velocity * (float)MoveVel + 2f); //moving normal
    }

    /**
     * <summary>Toggles between -1 and 1 based on the value given.
     * <para>Starts at 1 when given a 1</para></summary>
     * <param name="i">The index from the pattern that will be returned</param>
     * <remarks>Could be done in a simplified way</remarks>
     */
    protected static int calculateParity(double i) {
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
        lastButtonPressed = index >= 0 && index < Buttons.Length ? Buttons[index] : null;
    }

    /**
     * <summary>Finds the index of the lastButtonPressed inside the buttons array</summary>
     * <returns>The index of the KeyCode corresponding to the lastButtonPressed</returns>
     * <remarks>This implementation uses a delegate to find the index of the lastButtonPressed within the buttons array</remarks>
     */
    protected static int getLastButtonIndex() {
        return Buttons.ToList().FindIndex(code => code.Equals(lastButtonPressed));
    }
}
