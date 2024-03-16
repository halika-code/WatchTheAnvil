using System.Threading.Tasks;

public class Extras {
    /**
     * <summary>Runs an async based timer</summary>
     * <param name="milliseconds">The exact seconds and milliseconds in one variable a function should halt for</param>
     * <example>For a stop of 5.5 seconds, just supply 5.5F as the parameter</example>
     * <remarks>For a simpler function call, see <see cref="runTimer(int)"/></remarks>
     */
    public static async void runTimer(float milliseconds) {
        await Task.Delay((int)(milliseconds * 1000));
    }

    /**
     * <summary>Runs an async based timer</summary>
     * <param name="seconds">A rough amount of seconds the function should halt for</param>
     * <remarks>For a more precise stopping, see <see cref="runTimer(float)"/></remarks>
     */
    public static async void runTimer(int seconds) {
        await Task.Delay(seconds * 1000);
    }
}