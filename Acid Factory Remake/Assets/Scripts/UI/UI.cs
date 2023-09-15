using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
     * <summary>Updates the score of the player with a set value</summary>
     * <param name="score">Points that needed to be updated</param>
     */
    public static void updatePoints(int score) {
        points.text = updateText(points, score);
    }

    /**
     * <summary>Updates the health of the player</summary>
     * <param name="point">The amount to be deducted / added</param>
     */
    public static void updateHealthPoint(int point) {
        health.text = updateText(health, point);
    }

    /**
     * <summary>A generic function to update a text-box with a number embedded into it
     * <para>A score will be added to the numbers embedded</para></summary>
     * <param name="textBox">The TMP_Text to be modified</param>
     * <param name="score">The score to be added to the textBox</param>
     * <returns>The modified text of the text-box</returns>
     */
    private static string updateText(TMP_Text textBox, int score) {
        return textBox.text.Split(" ")[0] + " " + (findNumbers(textBox.text) + score).ToString(); //todo after 12 it reverts back to 4 ...
    }

    /**
     * <summary>Fetches the number embedded in the text-box</summary>
     * <param name="name">The name of the object a number is embedded into</param>
     * <returns>The number found</returns>
     * <remarks>This is setup to handle text-boxes with no numbers in them</remarks>
     */
    private static int findNumbers(string name) {
        var number = new List<char>();
        for (var i = name.Length - 1; i > 0; i--) {
            if (!int.TryParse(name[i].ToString(), out var digit)) {
                break;
            } number.Add(digit.ToString()[0]);
        } number.Reverse();
        return int.Parse(number.ToArray()); //either leaving this or using number.toStringArray
    }

    /**
     * <summary>Carbon copy of the <see cref="getPoints()"/> function</summary>
     */
    public static int getHealthPoints() {
        return int.Parse(health.text[^1].ToString());
    }

    /**
     * <summary>Sets the health of the player to a predetermined amount</summary>
     * <param name="points">The amount of health the player should have</param>
     */
    public static void setHealth(string points) {
        health.text = "Health: " + points;
    }
}
