using System;
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
        if (UI.getVeggiePoints() >= Math.Round(maxPoints * 0.8)) {
            Debug.Log("Running into the next level ... kinda");
            killPlayer(); //todo add functionality that loads the next level
        } else {
            gameObject.GetComponentInChildren<TextMeshPro>().text =
                "Whoops, looks like you tried leaving with " + UI.getVeggiePoints() + " points against " +
                maxPoints + ", need more than that";
        }
    }

    /**
     * <summary>Attempts to load a level by a given name
     * <para>If the level name is invalid, an error will be thrown</para></summary>
     */
    public static void loadLevel(string levelName) {
        if (SceneManager.GetSceneByName(levelName) != default) {
            SceneManager.LoadScene(levelName);
        }
        else {
            Debug.Log("Whoopy, I tried to load a level named " + levelName + ". This level does not exist in the build menu");
        }
    }

    public static LevelManager getLevelLoader() {
        return lvlLoader;
    }
}
