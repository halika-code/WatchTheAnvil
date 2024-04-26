using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static GravAmplifier;
using static VelocityManipulation;
using Task = System.Threading.Tasks.Task;

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
     * <summary>Checks for every interaction the player could take</summary>
     */
    public static Vector3 checkForButtonPress() {
        var isMenuOpen = MenuHandler.isMenuOpen(out var whichMenu); //checking if any of the menu is open. The idea here is there should not be any overlapping menu popping up
        if (!Extras.isTimerRunning) {
            if (checkForExit(!isMenuOpen || !whichMenu) || checkForPauseMenu(!isMenuOpen || whichMenu)) { //if pause / escape is pressed, pots
                Extras.runTimer(0.1d); //UI interactions should be restricted to a reduced speed to not give the player a stroke
                return Vector3.zero;
            }
        } return checkForPlayerInteraction();
    }

    /**
     * <summary><para>Evaluates the movement vector of the player</para>
     *  Based on the keys supplied.</summary>
     * <returns>A set of velocity the player will go with IF the player is grounded</returns>
     */
    private static Vector3 checkForPlayerInteraction() {
        var vel = pBody.velocity;
        for (var i = 0; i <= 3; i++) {
            if (Input.GetKey(Buttons[i]) && Move.getMove() != Move.CanMove.Cant) { //Note: casting to int practically performs a Math.Floor operation
                if (wasOppositePressed(vel, i)) { 
                    resetDampening(i < 2 ? 0 : 1); //if the player have not pressed the current button, hard reset dampening
                } vel[truncateIndex(i)] = applyRestriction(i);
                if (JumpController.getJumpingState(i, out var flyingState) && flyingState is not 2) {  
                    updateButtonPress(i);
                } continue; //if an input is pressed, skip to the next cycle, down below we can expect the button processed "above" will not be modified
            } if (absRound(vel[truncateIndex(i)]) > 1d) { //built-in dampening when the player have not pressed a given button
                vel[truncateIndex(i)] *= 0.99f;
            } else {
                resetDampening(i < 2 ? 0 : 1);
            }
        } return vel;
    }

    private static bool checkForExit(bool shouldToggle) {
        if (Input.GetKeyUp(KeyCode.Escape) && shouldToggle) { //toggles the escape menu
            MenuHandler.escapeMenu.SetActive(!MenuHandler.escapeMenu.activeSelf);
            return true;
        } return false;
    }

    private static bool checkForPauseMenu(bool shouldToggle) {
        if (Input.GetKeyUp(KeyCode.P) && shouldToggle) { //toggles the pause menu
            MenuHandler.menu.SetActive(!MenuHandler.menu.activeSelf);
            return true;
        } return false;
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
                return incrementPlayerSpeed(pBody.velocity[truncateIndex(i)] + velocity * ((float)MoveVel / 17.1f)); //flying dampening
            } return velocity * (float)(MoveVel * 1.25); //dropping faster
        } return processInitialDampening(i, velocity);
    } //moving normal

    /**
     * <summary>Toggles between -1 and 1 based on the value given.
     * <para>Starts at 1 when given a 1</para></summary>
     * <param name="i">The index from the pattern that will be returned</param>
     * <remarks>Could be done in a simplified way</remarks>
     */
    protected static int calculateParity(double i) {
        return i % 2 is not 0 ? 1 : -1; //note, the integer casting is used to divide by zero and get 0 as result. The brackets are important there, as in we wanna divide then cast
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
        } return false;
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

    /**
     * <summary>Checks if the player has pressed the correct button AND can use an item</summary>
     */
    public static bool checkForItemUse() {
        return Input.GetKey(KeyCode.F) && Toolbelt.getBelt().toolInHand /*hand is not null*/ && !itemCoolDown;
    }

    /**
     * <summary>Updates the button to a valid index of the buttons array</summary>
     * <remarks>If an incorrect index is supplied, the lastButtonPressed will be set to null</remarks>
     * <example>Sets in Collide to null, sets in InputController to a relevant index</example>
     */
    public static void updateButtonPress(int index) {
        lastButtonPressed = index >= 0 && index < Buttons.Length ? Buttons[index] : null;
    }
    
    /**
     * <summary>Checks the current index extracted from the player's speed against the future index given</summary>
     * <returns>true if the current index is found to be the opposite of the future index
     * <para>False otherwise</para></returns>
     * <param name="vel">The previous speed the player had</param>
     * <param name="i">The index of the button currently pressed</param>
     * <remarks>Doesn't matter which button the player presses, this function will find the</remarks>
     */
    private static bool wasOppositePressed(Vector3 vel, int i) { //the i is the current button pressed, 
        return getIndexFromVelocity(vel, i) is -1 || getIndexFromVelocity(vel, i) == getOpposite(i); 
    }

    /**
     * <summary>Calculates the index of the button that is opposite to the button assigned to i</summary>
     * <remarks>under index of buttons I mean <see cref="Buttons"/></remarks>
     */
    protected static int getOpposite(int i) {
       return i + calculateParity(i) * -1;
    }

    /**
     * <summary>Fetches the index used in the main <see cref="checkForPlayerInteraction"/>'s for loop
     * by using the speed of the player and a given index</summary>
     * <param name="vel">The speed the player has</param>
     * <param name="index">The index of the speed within the vel parameter that should be processed</param>
     */
    private static int getIndexFromVelocity(Vector3 vel, int index) {
        if (absRound(vel[truncateIndex(index)]) > 1f) { //if the index in question contains an actual speed and not just float imprecision data (usually 1.26*10^-6)
            return (Math.Sign(vel[truncateIndex(index)]) is -1 ? 0 : 1) + truncateIndex(index); //the + index is used
        } return -1;
    }

    /**
     * <summary>Truncates the index to either 0 or 2</summary>
     */
    private static int truncateIndex(int i) {
        return i < 2 ? 0 : 2;
    }
}
