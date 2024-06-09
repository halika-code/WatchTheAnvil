using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class Extras {

    /**
     * <summary>An array to keep the state of the timers in check.
     * <para>First is the IEnumerator, second is the async timer</para></summary>
     */
    public static bool[] isTimerRunning = {false, false};
    /**
     * <summary>Runs an ienumerator based timer</summary>
     * <param name="seconds">A rough amount of seconds the function should halt for</param>
     * <remarks>For a more precise stopping, see <see cref="runTimer(float)"/></remarks>
     */
    public static IEnumerator runTimer(int seconds) {
        yield return runTimer(milliseconds: seconds);
    }
    
    /**
     * <summary>Runs an ienumerator based timer</summary>
     * <param name="milliseconds">The exact seconds and milliseconds in one variable a function should halt for</param>
     * <example>For a stop of 5.5 seconds, just supply 5.5F as the parameter</example>
     * <remarks>For a simpler function call, see <see cref="runTimer(int)"/></remarks>
     */
    public static IEnumerator runTimer(float milliseconds) {
        isTimerRunning[0] = true;
        yield return new WaitForSeconds(milliseconds);
        isTimerRunning[0] = false;
    }

    public static async void runTimer(double milliseconds) {
        isTimerRunning[1] = true;
        await Task.Delay((int)(milliseconds * 1000));
        isTimerRunning[1] = false;
    }
}