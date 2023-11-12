
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Script.Tools.ToolType {
    public class FlowerController : MonoBehaviour {
        private static List<Flower> flowerArray = new();
        public static List<Flower> bouquet = new();
        
        public void prepFlowers() {
            var flowerCollection = GameObject.Find("Flowers").GetComponentsInChildren<BoxCollider>();
            foreach (var flower in flowerCollection) {
                flowerArray.Add(gameObject.AddComponent<Flower>());
                RootVeg.addVeg(flower.gameObject.GetComponent<Rigidbody>());
            }
        }

        private bool checkIfFlowersInHand() {
            return Toolbelt.getBelt().toolInHand.name.Contains("Flower");
        }

        /**
         * <summary>Adds a single flower into the collection</summary>
         */
        public static void addFlower(string name) {
            var tulip = findFlower(name);
            if (tulip == null) {
                Debug.Log("Couldn't find the flower in the array");
                return;
            } flowerArray.Remove(tulip);
            bouquet.Add(tulip);
        }

        private static Flower findFlower(string tulip) {
            foreach (var flower in flowerArray) {
                if (flower.name.Equals(tulip)) {
                    return flower;
                }
            } return null;
        }

        /**
         * <summary>Decides how many lives the player should receive for his/her flower bouquet</summary>
         */
        public static void useItem() {
            var hp = UI.getHealthPoints();
            switch (bouquet.Count) {
                case 0: {
                    break;
                } case 1: {
                    hp++;
                    break;
                } case 2: {
                    hp += 2;
                    break;
                } case >2: {
                    hp += 5;
                    break;
                } default: {
                    Debug.Log("Whoopy in trying to get hearts for " + bouquet.Count + " flowers");
                    break;
                }
            } UI.updateHealthPoint(hp);
        }
    }
}