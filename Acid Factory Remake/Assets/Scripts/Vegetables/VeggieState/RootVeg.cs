using System;
using System.Collections.Generic;
using System.Linq;
using Script.Tools.ToolType;
using UnityEngine;
using static VegetableVisibility;

public class RootVeg : MonoBehaviour {
    private Dictionary<Rigidbody, VegState> collective;
    private static RootVeg root;
    private static int beetPoints = 0;

    public void OnEnable() {
        root = this; //if null, assign as this
        collective = new Dictionary<Rigidbody, VegState>();
    }

    /**
     * <summary>Wipes the list from the previous level</summary>
     */
    public void OnDisable() {
        collective.Clear();
        root = null;
    }

    /**
     * <summary>Appends to the end of the list the new variables</summary>
     * <param name="bCol">The array containing the fresh vegetables</param>
     * <param name="terminate">an extra bit denoting if the script accessing this function should destroy itself</param>
     * <remarks>If the bCol is fed as null, the function terminates immediately
     * <para>The bCol is assumed to be new and not existing already in the cBodyCollective</para></remarks>
     */
    public void init(Rigidbody[] bCol, out bool terminate) {
        terminate = bCol.Length is 0; //idea here is if there is no vegetables under this parent script, the script should disable
        foreach (var bodyCollective in bCol) {
            collective.Add(bodyCollective, VegState.Hidden);
        } if (LevelManager.getLevelLoader().levelType is "Level") {
            GameObject.Find("Flowers").GetComponent<FlowerController>().prepFlowers(); 
        }
    }

    public bool doesContainVeggie(Rigidbody veg) {
        return collective.ContainsKey(veg);
    }

    /**
     * <summary>Returns the collection the RigidBodies are kept that are vegetables</summary>
     */
    public List<Rigidbody> getBodyCollective() {
        return collective.Keys.ToList();
    }

    /**
     * <summary>Returns the collection of the states registered for each RigidBodies kept in <see cref="cBodyCollective"/></summary>
     */
    public List<VegState> getVegStates() { 
        return collective.Values.ToList();
    }

    public void updateCollective(int index, VegState state) {
        collective[collective.Keys.ToList()[index]] = state;
    }

    /**
     * <summary>Adds a single vegetable into the list to be animated upon the player's proximity </summary>
     */
    public void addVeg(Rigidbody veg) {
        collective.Add(veg, VegState.Hidden);
    }

    /**
     * <summary>Removes a given vegetable from an entry</summary>
     */
    public void removeVeg(Rigidbody veg, out bool didItSucceed) {
        if (doesContainVeggie(veg)) {
            collective.Remove(veg);
        } didItSucceed = doesContainVeggie(veg);
    }

    /**
     * <summary>Updates the beet's count</summary>
     */
    public static void updateBeetPoints(int freshPoints) {
        beetPoints += freshPoints;
    }

    /**
     * <summary>Returns an array consisting of the calculated carrot points from UI and the kept beetroot points</summary>
     */
    public static int[] getPulledPoints() {
        return new [] {UI.getVeggiePoints() , beetPoints};
    }

    public static RootVeg getRoot() {
        return root;
    }
}
