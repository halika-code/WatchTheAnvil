using System.Collections.Generic;
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
                RootVeg.getRoot().addVeg(flower.gameObject.GetComponent<Rigidbody>());
            }
        }

        /**
         * <summary>Attempts to remove a flower from the collection</summary>
         * <remarks>Will fall through if flower is not found or is pulled</remarks>
         */
        public static void pullFlower(Flower tulip) {
            if (tulip == null) {
                Debug.Log("Couldn't find the flower in the array");
            } else if (!tulip.havePulled) {
                tulip.havePulled = true;
                RootVeg.getRoot().removeVeg(tulip.GetComponent<Rigidbody>(), out _);
                addFlower(tulip);
            } 
        }

        /**
         * <summary>Adds a single flower into the collection</summary>
         */
        private static void addFlower(Flower tulip) {
            Bouquet.Add(tulip);
        }

        /**
         * <summary>Transforms a given name to an instance of a Flower object with a matching name</summary>
         * <returns>The instance of the flower found, or a null instance</returns>
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
            Bouquet.Clear();
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