using System;
using System.Collections;
using UnityEngine;
using static Character_Controller;

public class StopTools : MonoBehaviour {
    public void OnTriggerEnter(Collider other) { //todo idae: use the velocity along with this distance check (angled towards a wall) to check if the veggie have collided with anything ... wall-like
        if (getParentName(other.gameObject) is not "Player") { //if the object is not the player
            if (isFarFromGround(gameObject)) { //if the ground underneath is far
                handleWallInteraction();
            } else { //if the object collided with is underneath it. This is the intended final branch this script should take
                var rObj = gameObject.GetComponent<Rigidbody>();
                rObj.useGravity = false;
                StartCoroutine(dampenLanding());
            }
        }
    }

    /**
     * <summary>Reverses the speed of the object</summary>
     * <remarks>This is outside the Collide script only because it concerns items outside the player's interactions (when called)
     * <para>Cannot be moved since it relies its speed fetch and storing on the gameObject being the triggering object</para></remarks>
     */
    private void handleWallInteraction() {
        var body = gameObject.GetComponent<Rigidbody>();
        Vector3 position = default; //a variable I use to store the shooting angle I send in to isFarFromGround
        for (var i = 0; i < 3; i++) {
            if (body.velocity[i] is 0) { //if the speed we are checking is 0, skip a cycle
                continue;
            } position[i] = VelocityManipulation.absRound(body.velocity.x) < 0.5f ? 0f : Math.Sign(body.velocity.x);
            if (!isFarFromGround(gameObject, position.x, position.y, position.z)) {
                var speed = body.velocity; //hate this implementation
                speed[i] *= i is 1 ? 1.1f : -0.8f; //gives a small speed penalty or a small boost (if colliding with a ceiling)
                body.velocity = speed;
                break;
            } position = default;
        }
    }

    /**
     * <summary>Slows the landing of the object in question</summary>
     */
    private IEnumerator dampenLanding() {
        var speed = gameObject.GetComponent<Rigidbody>();
        while (!speed.velocity.) { //todo have a check that checks if the speed is close enough to 0. Could be a separate function
            speed.velocity *= 0.8f;
            yield return new WaitForSeconds(0.1f);
        } speed.velocity = Vector3.zero;
    }
}
