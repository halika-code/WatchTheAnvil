using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tools : MonoBehaviour {
    protected int lifeSpanTimer;
    protected new string name;

    public static void useItem(Object obj) {
        //todo add logic that switches between the children scripts (Flower, Equipment, StopWatch)
    }
}