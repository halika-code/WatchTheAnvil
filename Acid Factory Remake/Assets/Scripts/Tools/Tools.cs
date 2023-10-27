using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools {
    public string name;
    public int lifeSpanTimer;
    
    public Tools() {
        name = "Helmet";
        lifeSpanTimer = -1;
    }

    /**
     * <summary>Defines the item based on it's name</summary>
     * <para>Recognised names include: Flower, Helmet, Vest, Slippers, Dynamite and StopWatch</para>
     */
    public Tools(string name) {
        this.name = name;
        switch (name) {
            case "Flower" or "Helmet" or "Vest" or "Slippers": {
                lifeSpanTimer = -1;
                break;
            } case "Dynamite": {
                lifeSpanTimer = 5;
                break;
            } case "StopWatch": {
                lifeSpanTimer = 10;
                break;
            } default: {
                Debug.Log("Whoopy while defining a tool named " + name);
                break;
            }
        }
    }
}
