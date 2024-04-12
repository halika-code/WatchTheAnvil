using UnityEngine;

public abstract class Tools : MonoBehaviour {
    protected int lifeSpanTimer;
    public new string name;

    public abstract void useItem();
}