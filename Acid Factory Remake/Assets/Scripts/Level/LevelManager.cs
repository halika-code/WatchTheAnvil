using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    private static int maxPoints;
    
    // Start is called before the first frame update
    private void Start() {
        maxPoints = 0;
        countMaxPoints();
    }

    /**
     * <summary>Calculates the max amount of points the player can possibly earn in a level</summary>
     * <remarks>Relies on <see cref="VegetablePull.getProfileOfVeggie"/> and
     * <see cref="Character_Controller.getParentName(GameObject)"/> to get the calculations</remarks>
     */
    private void countMaxPoints() {
        foreach (var veggie in RootVeg.getBodyCollective()) {
            if (veggie.name is not "Flower") {
                maxPoints += VegetablePull.getProfileOfVeggie(Character_Controller.getParentName(veggie.transform));
            }
        }
    }

    public static void advanceLevel() {
        if (UI.getCurrentPoints() >= Math.Round(maxPoints * 0.8)) {
            Debug.Log("Running into the next level ... kinda");
            Character_Controller.killPlayer(); //todo add functionality that loads the next leve
        }
        else {
            Debug.Log("Player wanted to leave level with " + UI.getCurrentPoints() + " points against " + maxPoints);
        }
    }
}
