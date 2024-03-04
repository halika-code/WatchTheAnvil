using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ButtonHandler : MonoBehaviour {
    private TextMeshProUGUI[] buttonArray;
    public void Start() {
        buttonArray = GetComponentsInChildren<TextMeshProUGUI>();
    }

    public async void reset() {
        await Task.Delay(100); //holds the game's horse for a bit
        Character_Controller.killPlayer();
    }
}
