
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Script.Tools.ToolType {
    public class FlowerController : MonoBehaviour {
        private static readonly List<Flower> FlowerArray = new();
        public static readonly List<Flower> Bouquet = new();
        
        public void prepFlowers() {
            foreach (var flower in gameObject.GetComponentsInChildren<Rigidbody>()) {
                FlowerArray.Add(flower.AddComponent<Flower>());
                FlowerArray[^1].prepFlower(flower.gameObject);
                RootVeg.addVeg(flower.gameObject.GetComponent<Rigidbody>());
            }
        }

        private bool checkIfFlowersInHand() {
            return Toolbelt.getBelt().toolInHand.name.Contains("Flower");
        }

        /**
         * <summary>Adds a single flower into the collection</summary>
         */
        public static void addFlower(Flower tulip) {
            var flower = findFlower(tulip.name);
            if (flower == null) {
                Debug.Log("Couldn't find the flower in the array");
                return;
            } flower.havePulled = true;
            tulip.gameObject.transform.parent = Character_Controller.getPlayerHand(); //todo remove the instance of the flower from the vegStateCollective array in RootVeg
            Bouquet.Add(flower);
        }

        /**
         * <summary>Transforms a given name to an instance of a Flower object with a matching name</summary>
         */
        public static Flower findFlower(string tulip) {
            foreach (var flower in FlowerArray) {
                if (flower.name.Equals(tulip)) {
                    return flower;
                }
            } return null;
        }

        /**
         * <summary>Checks if a valid flower based on the flower-name exists in the list kept here
         * </summary>
         * <returns>true if a flower with the exact name is found, false otherwise</returns>
         */
        public static bool checkIfFlowerExists(string flowerName) {
            return findFlower(flowerName) != null;
        }

        /**
         * <summary>Checks if the flower been pulled</summary>
         * <returns>true if the plan is found to be pulled, false if it wasn't or if the flower wasn't found</returns>
         * 
         */
        public static bool haveFlowerBeenPulled(string name) {
            var flower = findFlower(name);
            return flower != null && flower.havePulled;
        }

        /**
         * <summary>Decides how many lives the player should receive for his/her flower bouquet</summary>
         */
        public static void useItem() {
            var hp = UI.getHealthPoints();
            switch (Bouquet.Count) {
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
                    Debug.Log("Whoopy in trying to get hearts for " + Bouquet.Count + " flowers");
                    break;
                }
            } UI.updateHealthPoint(hp);
        }
    }
}