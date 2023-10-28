using System.Collections;
using UnityEngine;
using static AnvilManager;

namespace Script.Tools.ToolType {
    public class StopWatch : global::Tools{
        
        public bool stopWatchInUse;
        public StopWatch() {
            lifeSpanTimer = 20;
            stopWatchInUse = false;
        }
        
            
        /**
         * <summary>Find which state the anvil is in, saves that state and attempts to continue execution where it was left off</summary>
         * <param name="shouldContinue">The flag that flips intended function of this script, unfreezing the anvil and terminating early</param>
         */
        public IEnumerator runStopWatch(bool shouldContinue) {
            var wait1Sec = new WaitForSeconds(1);
            var targetScript = getTargetScript();
            if (!shouldContinue) {
                StartCoroutine(targetScript);
                stopWatchInUse = false;
                yield break;
            } StopCoroutine(targetScript);
            stopWatchInUse = true;
            while (lifeSpanTimer is not 0) { //actual wait
                yield return wait1Sec;
                lifeSpanTimer--;
            } StartCoroutine(targetScript);
            stopWatchInUse = false;
        }

        /**
         * <summary>A sister-script to <see cref="runStopWatch"/>, finds the script that is currently running concerning the anvil</summary>
         * <returns>The instance of that script in IEnumerator form</returns>
         */
        private IEnumerator getTargetScript() {
            if (currentAnvil.aTimer is not 0) { //if the anvil is falling
                currentAnvil.freezeAnvil();
                return currentAnvil.dropAnvil();
            } return waitTimer is 0 ? //if the anvil have spawned
                helpRunTimer(currentAnvil, currentAnvil.aTimer is 3 ? 3 : 0) : /*deciding if after the freeze, the anvil should target or not */
                startInitialWait(); //if the anvil is just waiting to spawn
        }
    }
}