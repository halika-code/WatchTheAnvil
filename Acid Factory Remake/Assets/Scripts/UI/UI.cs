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
    
    
    private void Update() {
        
    }

    /**
     * <summary>Updates the points with a set value</summary>
     * <param name="score">Points that needed to be updated</param>
     * <remarks>A bit convoluted, but looks more robust than just dictating</remarks>
     */
    public static void updatePoints(int score) {
        points.text = points.text.Replace(points.text[^1], score.ToString()[0]);
    }

    public static void updateHealthPoint(int points) {
        
    }

    /**
     * <summary>Fetches the points the player have earned so far</summary>
     */
    public static int getPoints() {
        return int.Parse(points.text.Split(" ")[1]); //example expected text "Points: 0"
    }

    /**
     * <summary>Carbon copy of the <see cref="getPoints()"/> function</summary>
     */
    public static int getHealthPoints() {
        return int.Parse(health.text.Split(" ")[1]);
    }
}
