using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class MenuHandler : MonoBehaviour {
    private TextMeshProUGUI[] buttonArray; //menu
    public static GameObject menu;
    private TextMeshProUGUI veggieInfo;
    public static GameObject escapeMenu;
    
    /**
     * <summary>Loads in every smaller menu's components</summary>
     * <remarks>A hefty function if I ever made one</remarks>
     */
    public void Start() {
        menu = GameObject.Find("PauseMenu");
        escapeMenu = GameObject.Find("EscapeMenu");
        var asd = menu.GetComponentsInChildren<TextMeshProUGUI>();
        buttonArray = GameObject.Find("Buttons").GetComponentsInChildren<TextMeshProUGUI>();
        veggieInfo = GameObject.Find("Text-field").GetComponentsInChildren<TextMeshProUGUI>()[1];
        escapeMenu.SetActive(false);
        menu.SetActive(false);
    }

    public async void reset() {
        await Task.Delay(100); //holds the game's horse for a bit
        Character_Controller.killPlayer();
    }

    public void onContinue() {
        UI.ui.SetActive(true);
        gameObject.SetActive(false);
    }

    /**
     * <summary>Exits from a given level</summary>
     */
    public void exit() {
        if (LevelManager.getLevelLoader().levelType is "Menu") {
            escape();
        } LevelManager.loadLevel("WorldMap");
    }

    /**
     * <summary>Terminates the game</summary>
     */
    public void escape() {
        Application.Quit();
        Debug.Break();
    }

    /**
     * <summary>Updates the vegetable tally according to the player's action</summary>
     */
    public void updateVeggieTally() {
        var carrotScore = RootVeg.getPulledPoints();
        var veggieTallyText = "You haven't found any carrots yet";
        if (carrotScore[0] is 0) {
            if (carrotScore[1] is not 0) {
                veggieTallyText = "You have found " + carrotScore[1] + " pieces of beet! Now to find the carrots we need";
            }
        } else if (carrotScore[1] is 0) {
            veggieTallyText = "You have found " + carrotScore[0] + " pieces of carrots. Great!";
        } else {
            veggieTallyText = "You have found " + carrotScore[0] + " carrots and " + carrotScore[1] +" beets. Amazing!";
        } veggieInfo.text = veggieTallyText;

    }
}
