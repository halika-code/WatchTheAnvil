
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
            } FlowerArray.Remove(tulip);
            Bouquet.Add(tulip);
            
            Debug.Log("Flower Added");
        }

        private static Flower findFlower(string tulip) {
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