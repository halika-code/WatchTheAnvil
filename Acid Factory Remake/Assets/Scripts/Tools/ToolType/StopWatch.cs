using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static AnvilManager;

namespace Script.Tools.ToolType {
    public class StopWatch : global::Tools {
        
        private Coroutine stopWatchFuncContainer; //used as a more reliable way to stop a coroutine
        public bool stopWatchInUse;
        public TextMeshPro text;
        public void prepStopWatch() {
            lifeSpanTimer = 20;
            stopWatchInUse = false;
            this.name = "StopWatch";
            text = gameObject.GetComponentInChildren<TextMeshPro>();
        }

        public void OnCollisionEnter(Collision other) {
            if (Character_Controller.getParentName(other.gameObject) is "Platforms") {
                gameObject.GetComponent<Collider>().isTrigger = true;
                gameObject.GetComponent<Rigidbody>().useGravity = false;
            }
        }

        public override void useItem() {
            if (stopWatchInUse) {
                StopCoroutine(stopWatchFuncContainer);
                stopWatchInUse = false;
                updateStopWatchDisplay();
            } else if (lifeSpanTimer > 0){
                stopWatchFuncContainer = StartCoroutine(runStopWatch());
            } else { //I expect a stop-watch is in the player's hand
                destroyWatch();
            }
        }
            
        /**
         * <summary>Find which state the anvil is in, saves that state and attempts to continue execution where it was left off</summary>
         */
        private IEnumerator runStopWatch() { 
            stopWatchInUse = true;
            while (lifeSpanTimer is not 0) { //actual wait
                updateStopWatchDisplay();
                yield return new WaitForSeconds(0.7f);
                lifeSpanTimer--;
            } destroyWatch();
        }

        /**
         * <returns>Returns True if the stop-watch is in use, false otherwise</returns>
         * <remarks>This function does a search for the stop-watch beforehand</remarks>
         */
        public static bool checkWatch() {
            return Toolbelt.getBelt().checkForTool("StopWatch", out var watch) && ((StopWatch)watch).stopWatchInUse;
        }

        /**
         * <summary>Updates the small text-box located above the stop-watch to display to the player how long the stop-watch can be used</summary>
         * <remarks>The textmeshpro component will have an empty string when the stop-watch is not in use</remarks>
         */
        private void updateStopWatchDisplay() {
            text.text = stopWatchInUse ? "Anvils frozen for " + lifeSpanTimer + " seconds" : ""; 
        }

        private static void destroyWatch() {
            Destroy(Toolbelt.getBelt().toolInHand.gameObject); //if the while loop falls through, destroy the item
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