using System.Collections;
using TMPro;
using UnityEngine;
using static AnvilManager;

namespace Script.Tools.ToolType {
    
    /**
     * <date>28/10/2023</date>
     * <author>Gyula Attila Kovacs</author>
     * <summary>A somewhat complex script that stops the anvils from dropping</summary>
     */
    public class StopWatch : global::Tools {

        public bool stopWatchInUse; // a flag if the async should suspend
        private delegate IEnumerator RunWatch(); //used as a more reliable way to stop a coroutine

        private RunWatch container;
        public TextMeshPro text;
        
        public void prepStopWatch() {
            stopWatchInUse = false;
            lifeSpanTimer = 20;
            this.name = "StopWatch";
            text = gameObject.GetComponentInChildren<TextMeshPro>();
            container += runStopWatch;
        }

        public void OnCollisionEnter(Collision other) {
            if (Character_Controller.getParentName(other.gameObject) is "Platforms") {
                gameObject.GetComponent<Collider>().isTrigger = true;
                gameObject.GetComponent<Rigidbody>().useGravity = false;
            }
        }

        public override void useItem() {
            if (stopWatchInUse) { //the stop-watch still have juice in it but the player wants to stop it
                updateStopWatchDisplay();
                stopWatchInUse = false; 
            } else if (lifeSpanTimer > 0){
                stopWatchInUse = true;
                if (waitTimer < 2 && waitTimer is not 0 || waitTimer is 0 && currentAnvil.aTimer < 2) { //if the anvil is in waiting time OR the anvil is just dropping
                    text.text = "Too late, watch out!";
                    stopWatchInUse = false;
                } else {
                    StartCoroutine(container.Invoke());
                } return; //avoids setting the stopwatch back to false
            } if (lifeSpanTimer == 0) { //I expect a stop-watch is in the player's hand
                destroyWatch();
            }
        }
            
        /**
         * <summary>Find which state the anvil is in, saves that state and attempts to continue execution where it was left off</summary>
         */
        private IEnumerator runStopWatch() { 
            while (lifeSpanTimer is not 0 && stopWatchInUse) { //actual wait
                updateStopWatchDisplay();
                yield return new WaitForSeconds(0.8f);
                lifeSpanTimer--;
            } if (stopWatchInUse) {
                destroyWatch();
            }
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
            text.text = stopWatchInUse ? "Anvils frozen for " + lifeSpanTimer + " seconds" : " "; 
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
                helpRunTimer(currentAnvil.aTimer is 3 ? 3 : 0) : /*deciding if after the freeze, the anvil should target or not */
                GameObject.Find("Anvils").GetComponent<AnvilManager>().runWait(); //if the anvil is just waiting to spawn, I expect this to be the less-costly option
        }
        #endregion 
        //unused, just not ready to delete this brainstorming result
        
    }
}