using System.Numerics;
using static GravAmplifier;
using static VelocityManipulation;
using Vector3 = UnityEngine.Vector3;

public class JumpController : InputController {
    
       
    /**
     * <summary>Gives the player a starting velocity then based on the desired speed cap, will apply increased gravity until the cap is reached / passed</summary>
     * <param name="speedUp">The starting Y velocity, by default is <see cref="Character_Controller.MoveVel"/> * 3.2 cast into a float</param>
     * <param name="desiredSpeedCap">The desired terminal velocity, by default is set to -70f</param>
     */
    public static void jump(float speedUp = (float)MoveVel * 3.2f, float desiredSpeedCap = -70f) {
        var pVel = pBody.velocity;
        gravity.falling(new Vector3(pVel.x, speedUp, pVel.z), desiredSpeedCap);
        incrementXSpeedDown(wait: 3f); //called from here to start the async function
    }


    /**
     * <summary>Decides if the lastPressedButton needs to be updated or not</summary>
     * <param name="i">The index the button is pressed in the <see cref="InputController.Buttons"/>array</param>
     * <param name="flyingState">An additional returned variable that houses information about the player's button press.
     * <para>Sets to 0 when an opposite key is pressed with a low speed</para>
     * Sets to 1 when the player moves after a stationary jump
     * <para>Sets to 2 when the player presses a direction mapped to a different axis</para>
     * Sets to 3 when the player presses a direction in the opposite direction but the same axis</param>
     * <returns>True, if the player makes a stationary jump then moves,
     * <para>If the player tries to strafe in the air or</para>
     * If the player have been moving in the opposite direction for long enough</returns>
     * Note: this doesn't specifically needs an out var flyingState BUT I feel like it might gonna be useful later
     */
    public static bool getJumpingState(int i, out int flyingState) {
        switch (isAscending) {
            case true when lastButtonPressed == null: {  //stationary jump then movement
                flyingState = 1;
                return true;
            } case true when lastButtonPressed != Buttons[i] && !wasOppositePressed(i): { //air-strafing
                flyingState = 2;
                return true;
            } case true when lastButtonPressed != Buttons[i] && speedHighEnough(getPlayerBody().velocity[i < 2 ? 0 : 2], calculateParity(i)): { //opposite air movement with high enough speed,
                flyingState = 3;
                return true; //note, when this statement is checked against, velocity dampening is expected to be making the movement
            } default: { //opposite air movement with low speed
                flyingState = 0;
                return false;
            }
        } 
    }

    /**
     * <summary>A simple function that handles negative values in the movement calculations</summary>
     */
    private static bool speedHighEnough(float pSpeed, int parity) {
        if (parity is 1) {
            return pSpeed > MoveVel * 0.8f;
        } return pSpeed < -MoveVel * 0.8f;
    }
    
    /**
     * <summary>Checks if the opposite key have been pressed</summary>
     * <returns>True, if the player presses a button that is mapped to the opposite parity of a given axis
     * <para>False if the player presses the same button or a button assigned to a different axis</para></returns>
     * <example>Scenario: The player presses 'A' after 'W':
     * <para>An i of value of 3 is given while the lastButtonPressed is KeyCode.A. The lastButtonPressed is converted into an index using <see cref="InputController.getLastButtonIndex"/>
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
    
}