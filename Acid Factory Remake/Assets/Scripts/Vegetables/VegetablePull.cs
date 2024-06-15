using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using static VelocityManipulation;

public class VegetablePull : MonoBehaviour {
    private static readonly int padding = 2;
    
    /**
     * <summary>Attempts to launch the vegetable in a random angle with random speed</summary>
     */
    public static void pullVegetable(Collider veggie) {
        RootVeg.getRoot().removeVeg(veggie.attachedRigidbody, out _);
        findNearestStableGround(veggie.attachedRigidbody);
        var parent = Character_Controller.getParentName(veggie.transform)[1];
        veggie.gameObject.GetComponentsInChildren<MeshRenderer>()[1].material = Resources.Load<Material>(
            $"Sprites/Animatables/Veggies/Carrots/Carrot{parent}/Materials/Outside");;
    }

    private static void findNearestStableGround(Rigidbody veggie) {
        var vegPos = veggie.position;
        var pPos = Character_Controller.getPlayerBody().position;
        veggie.useGravity = true;
        veggie.velocity = new Vector3(pPos.x - (vegPos.x + 5 * getRNG()), vegPos.y + 25f, pPos.z - (vegPos.z + 5 * getRNG()));
    }

    private static float getRNG() {
        var next = (float)new Random().Next(2, 6);
        if (next % 2 != 0) {
            next *= -0.1f;
        } return next;
    }

    /**
     * <summary>Checks if the vegetable has a valid parent</summary>
     */
    public static bool validateVegetable(GameObject veg) {
        var parent = Character_Controller.getParentName(veg.transform); //breakdown of the chain in an intended state: Veggie1 -> Small -> Carrot -> Vegetable
        return parent != null && parent.Contains("Vegetables");
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    /**
     * <summary>Gets a point value based on the size of a given veggie</summary>
     * <param name="parents">The name of the veggie that should be inspected</param>
     * <returns>A value ranging from 1 to 3</returns>
     * <remarks>If an object with an unexpected name is passed down, an error message will get popped alongside a return value of 0</remarks>
     */
    public static int getProfileOfVeggie(string parents) {
        switch (parents) {
            case "Carrot" or "Vegetables": {
                goto default;
            } case "Small": {
                return 1;  //returns usually a score of 1
            } case "Medium": {
                return 2;
            } case "Large": {
                return 3;
            } default: {
                Debug.Log($"Whoopy while trying to decide the score of the object of name {parents}");
                return 0;
            }
        }
    }

    /**
     * <summary>Based on the parent-list of a given vegetable,
     * calculates a score of what the object the player have pulled is worth</summary>
     * <param name="parentList">The name of the gameObject that are parent to the vegetable</param>
     * <returns>A point value based on the worth of the vegetable</returns>
     * <remarks>It is assumed that the parentList contains the "family tree" of a vegetable</remarks>
     */
    public static int calculatePoints(List<string> parentList) {
        var vegProf = getProfileOfVeggie(parentList[1]);
        if (parentList.Contains("Beetroot")) {
            RootVeg.updateBeetPoints(1 * vegProf);
            return 0;
        } return vegProf is 3 ? vegProf + 2 : vegProf;
    }

    public static IEnumerator rotateVeg(Rigidbody vegBod) { //rotate x by -80, y by -90, z by -70, change direction by 
        var vegVel = vegBod.velocity;
        var sprite = vegBod.GetComponentInChildren<MeshRenderer>().gameObject; //todo in the StopTool add a public static component for GameObject which intializes early
        var rotation = vegBod.rotation;
        var angle = rotation.eulerAngles;
        var targetAngle = vegVel + new Vector3(-80, Math.Sign(vegVel.x) * 90, Math.Sign(vegVel.x) * -70);
        while (absRound(angle.x) < absRound(targetAngle.x) * 0.8f || absRound(angle.y) < 
               absRound(targetAngle.y) * 0.8f || absRound(angle.z) < absRound(targetAngle.z) * 0.8f) { //todo the vegVel doesn't work here, need to use the SpriteHolder's rotation
            Debug.Log("started rotating");                                          //todo note: there are no Rigidbodies attached to any of the spriteHolder
            rotation.eulerAngles = Vector3.Lerp(angle, targetAngle, 0.1f);
            yield return null;
            vegBod.rotation = rotation;
        } Debug.Log("Done rotating");
    }
}
