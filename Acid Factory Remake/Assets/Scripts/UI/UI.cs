using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour {
    public static TMP_Text points;
    public static TMP_Text health;

    private void Start() {
        var canv = GameObject.Find("Canvas").GetComponentsInChildren<TextMeshProUGUI>();
        points = canv[0];
        health = canv[1];
    }

    /**
     * <summary>Updates the points with a set value</summary>
     * <param name="score">Points that needed to be updated</param>
     * <remarks>A bit convoluted, but looks more robust than just dictating</remarks>
     */
    public static void updatePoints(int score) {
        points.text = updateText(points, score);
    }

    public static void updateHealthPoint(int point) {
        health.text = updateText(health, point);
    }

    private static string updateText(TMP_Text textBox, int score) {
        var updatedScore = new char[findNumbers(textBox.text)];
        for (var i = textBox.text.Length - 1; i < textBox.text.Length - updatedScore.Length; i--) { //here I try to gather every number from the textbox
            updatedScore[textBox.text.Length - i] = textBox.text[i];
        } updatedScore = (int.Parse(updatedScore) + score).ToString().ToCharArray(); //todo test this! converts the prepared numbers into int, adds to the score then converts to string then to charArray

        return textBox.text.Split(" ")[0] + updatedScore;
    }

    private static int findNumbers(string name) {
        var counter = 0;
        while (counter < name.Length) {
            if (int.TryParse(name[^counter].ToString(), out counter)) { //as far as I can tell, this is a built-in counter ... cool
                return counter;
            }
        } return 0;
    }

    /**
     * <summary>Fetches the points the player have earned so far</summary>
     */
    private static int getPoints(TMP_Text textBox) {
        return int.Parse(textBox.text.Split(" ")[1]); //example expected text "Points: 0"
    }

    /**
     * <summary>Carbon copy of the <see cref="getPoints()"/> function</summary>
     */
    public static int getHealthPoints() {
        return int.Parse(health.text[^1].ToString());
    }
}
