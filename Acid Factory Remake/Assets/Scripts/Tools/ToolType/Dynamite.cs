using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Script.Tools.ToolType {
    public class Dynamite : global::Tools {
        private bool inUse;
        private TextMeshPro text;

        public void prepDynamite() {
            lifeSpanTimer = 5;
            inUse = false;
            name = gameObject.name;
            //new Anvil(Instantiate(preFab, transform, true), 3);
            text = gameObject.GetComponentInChildren<TextMeshPro>();
        }

        public override void useItem() {
            if (!inUse) {
                inUse = true;
                if (ShadowController.findColPoint(Character_Controller.getPlayerHand().gameObject, out var hitPos)) { //todo test this
                    dropTNT(hitPos.point);
                } else {
                    Debug.Log("Whoopy, couldn't find a platform to place the dynamite at");
                }
            }
        }

        /**
         * <summary>Drops the dynamite onto the ground then explodes after the fuse gets spent</summary>
         */
        private async void dropTNT(Vector3 pos) {
            transform.position = pos;
            gameObject.transform.parent = null;
            while (lifeSpanTimer is not 0) {
                drawText();
                await Task.Delay(1000);
                lifeSpanTimer--;
            } explode();
        }

        /**
         * <summary>Sister-function to <see cref="dropTNT"/>
         * <para>Explodes any explodable rocks found nearby, then destroys itself</para></summary>
         * <remarks>The player can also be damaged if found to be in the blast radius</remarks>
         */
        private void explode() {
            foreach (var hit in Physics.SphereCastAll(gameObject.transform.position, 5f,Vector3.forward, 5f)) {
                if (Character_Controller.getParentName(hit.transform.gameObject).Contains("ExplodableRocks")) {
                    Destroy(hit.transform.gameObject);
                } if (Character_Controller.getParentName(hit.transform) is "Player") {
                    if (!Toolbelt.getBelt().checkForTool("Vest", out _)) {
                        Character_Controller.hurtPlayer();
                    }
                }
            } Destroy(gameObject);
        }

        /**
         * <summary>Displays the <see cref="Dynamite.lifeSpanTimer"/> to the dynamite's child text-box above it</summary>
         */
        private void drawText() {
            text.text = "Explodes in " + lifeSpanTimer;
        }

        public TextMeshPro getText() {
            return text;
        }

        public bool checkIfInUse() {
            return inUse;
        }
    }
}