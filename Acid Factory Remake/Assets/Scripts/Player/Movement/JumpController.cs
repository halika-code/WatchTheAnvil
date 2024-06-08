using System.Linq;
using static GravAmplifier;
using static VelocityManipulation;
using Vector3 = UnityEngine.Vector3;

public class JumpController : InputController {
    
       
    /**
     * <summary>Gives the player an initial / desired speed and a speed-cap then using that speed initializes the gravity amplification</summary>
     * <param name="speedUp">The starting Y velocity, by default is <see cref="Character_Controller.MoveVel"/> * 2.9 cast into a float</param>
     * <param name="desiredSpeedCap">The desired terminal velocity (this sets how fast the player will fall, by default is set to -70f but can also be set to 0 for unity's gravity to float the player away</param>
     */
    public static void jump(float speedUp = (float)MoveVel * 2.9f, float desiredSpeedCap = -70f) { //note: modifying the speedUp will screw with Character_Controller.isFarFromGround()
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
                break;
            } case true when lastButtonPressed != Buttons[i] && !wasOppositePressed(i): { //air-strafing
                flyingState = 2;
                break;
            } case true when lastButtonPressed != Buttons[i] && speedHighEnough(getPlayerBody().velocity[i < 2 ? 0 : 2], calculateParity(i)): { //opposite air movement with high enough speed,
                flyingState = 3;
                break; //note, when this statement is checked against, velocity dampening is expected to be making the movement
            } default: { //opposite air movement with low speed
                flyingState = 0;
                return false;
            }
        } return true;
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
     * <summary>Finds the index of the lastButtonPressed inside the buttons array</summary>
     * <returns>The index of the KeyCode corresponding to the lastButtonPressed</returns>
     * <remarks>This implementation uses a delegate to find the index of the lastButtonPressed within the buttons array</remarks>
     */
    private static int getLastButtonIndex() {
        return Buttons.ToList().FindIndex(code => code.Equals(lastButtonPressed));
    }
    
    /**
     * <summary>Checks if the opposite key have been pressed</summary>
     * <returns>True, if the player presses a button that is mapped to the opposite parity of a given axis
     * <para>False if the player presses the same button or a button assigned to a different axis</para></returns>
     * <example>Scenario: The player presses 'W' after 'A':
     * <para>An i of value of 3 is given while the lastButtonPressed is KeyCode.A. The lastButtonPressed is converted into an index using <see cref="InputController.getLastButtonIndex"/>
     * and the value of 3 is compared against a value of 0.</para>
     * For the return condition, the two numbers are checked to not be the same (1st condition: true), and to be equal when 1 * (-1)
     * is added to the i variable, to arrive at the expected opposite value: 2 (2nd condition: false)
     * </example>
     * <remarks> Example uses default button mappings
     * <para>This function will function properly as long as there is exactly 4 keys in the registered button mappings</para></remarks>
     */
    private static bool wasOppositePressed(int i) { 
        if (lastButtonPressed == null) {
            return true;
        } return getLastButtonIndex() != i &&  /*below on the left I grab the parity the i corresponds with (negative or positive in any axis)*/
                 getOpposite(i) == getLastButtonIndex(); //I can calculate the opposite value of the current index by grabbing the parity I got, multiply by -1 (flip it to the other side) and add i to it, which consistently puts it to the opposite key
    } //note at that -1 == buttons: the buttons is used to get the INDEX where the lastButtonPressed is kept at inside the array

}
