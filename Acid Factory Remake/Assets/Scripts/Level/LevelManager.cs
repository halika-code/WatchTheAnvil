using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Character_Controller;

public class LevelManager : MonoBehaviour {
    public string levelType;
    private static LevelManager lvlLoader;
    
    // Start is called before the first frame update
    private void OnEnable() {
        lvlLoader = this;
        levelType = SceneManager.GetActiveScene().name.Contains("Level") ? "Level" : "Menu";
    }

    /**
     * <summary>Calculates the max amount of points the player can possibly earn in a level</summary>
     * <remarks>Relies on <see cref="VegetablePull.getProfileOfVeggie(string)"/> and
     * <see cref="Character_Controller.getParentName(GameObject)"/> to get the calculations</remarks>
     */
    private static int countMaxPoints() {
        var maxPoints = 0;
        foreach (var veggie in RootVeg.getRoot().getBodyCollective()) {
            if (veggie.name is not "Flower") {
                maxPoints += VegetablePull.getProfileOfVeggie(getParentName(veggie.transform)[1]);
            }
        } return maxPoints;
    }

    public void advanceLevel() {
        var maxPoints = countMaxPoints();
        if (UI.getCurrentPoints() >= Math.Round(maxPoints * 0.8)) {
            Debug.Log("Running into the next level ... kinda");
            killPlayer(); //todo add functionality that loads the next level
        } else {
            gameObject.GetComponentInChildren<TextMeshPro>().text =
                "Whoops, looks like you tried leaving the level with " + UI.getCurrentPoints() + " points against " +
                maxPoints + ", try again when you have at least 80% of the vegetables collected";
        }
    }

    public static LevelManager getLevelLoader() {
        return lvlLoader;
    }
}
