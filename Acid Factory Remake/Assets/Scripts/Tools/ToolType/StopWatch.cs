using System.Collections;
using UnityEngine;
using static AnvilManager;

namespace Script.Tools.ToolType {
    public class StopWatch : global::Tools {
        
        private Coroutine stopWatchFuncContainer; //used as a more reliable way to stop a coroutine
        public static bool stopWatchInUse;
        public void prepStopWatch() {
            lifeSpanTimer = 20;
            stopWatchInUse = false;
            this.name = "StopWatch";
        }

        public override void useItem() {
            if (stopWatchInUse) {
                StopCoroutine(stopWatchFuncContainer);
                stopWatchInUse = false;
            } else {
                stopWatchFuncContainer = StartCoroutine(runStopWatch());
            }
        }
            
        /**
         * <summary>Find which state the anvil is in, saves that state and attempts to continue execution where it was left off</summary>
         */
        private IEnumerator runStopWatch() { 
            stopWatchInUse = true;
            while (lifeSpanTimer is not 0) { //actual wait
                yield return new WaitForSeconds(0.5f);
                Debug.Log("StopWatch has " + lifeSpanTimer + " seconds left");
                lifeSpanTimer--;
            } stopWatchInUse = false;
        }

        #region GetCurrentlyRunningIEnumeratorScript
        /**
         * <summary>A sister-script to <see cref="runStopWatch"/>, finds the script that is currently running concerning the anvil</summary>
         * <returns>The instance of that script in IEnumerator form</returns>
         */
        private IEnumerator getCurrentlyRunningAnvilScript() {
            if (currentAnvil.aTimer is 0) { //if the anvil is falling
                currentAnvil.freezeAnvil();
                return currentAnvil.dropAnvil();
            } return waitTimer is 0 ? //if the anvil have spawned
                helpRunTimer(currentAnvil, currentAnvil.aTimer is 3 ? 3 : 0) : /*deciding if after the freeze, the anvil should target or not */
                GameObject.Find("Anvils").GetComponent<AnvilManager>().runWait(); //if the anvil is just waiting to spawn, I expect this to be the less-costly option
        }
        #endregion 
        //unused, just not ready to delete this brainstorming result
        
    }
}