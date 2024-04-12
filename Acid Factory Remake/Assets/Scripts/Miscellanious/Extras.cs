using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class Extras {

    public static bool isTimerRunning = false;
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
        isTimerRunning = true;
        yield return new WaitForSeconds(milliseconds);
        isTimerRunning = false;
    }

    public static async void runTimer(double milliseconds) {
        isTimerRunning = true;
        await Task.Delay((int)(milliseconds * 1000));
        isTimerRunning = false;
    }
}