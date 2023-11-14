using System.Collections;
using UnityEngine;

namespace Script.Tools.ToolType {
    public class Dynamite : global::Tools {
        private bool inUse;
        private GameObject tnt;

        public void prepDynamite(GameObject obj) {
            lifeSpanTimer = 5;
            inUse = false;
            //new Anvil(Instantiate(preFab, transform, true), 3);
            tnt = obj;
            this.name = obj.name;
        }

        public override void useItem() {
            inUse = true;
            StartCoroutine(nameof(explode));
        }

        private IEnumerator explode() {
            var pBody = Character_Controller.getPlayerBody().position;
            tnt.transform.position = new Vector3(pBody.x, pBody.y - 10f, pBody.z);
            yield return new WaitForSeconds(lifeSpanTimer);
            //check for distance for rocks, if any is found, blow up
        }

        public bool checkIfInUse() {
            return inUse;
        }
    }
}