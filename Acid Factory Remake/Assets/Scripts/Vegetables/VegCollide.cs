using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegCollide : MonoBehaviour {
    
    /**
     * <summary>Re-bounces the veggie towards a reverse direction
     * <para>If the veggie lands on the ground, the veggie will freeze there</para></summary>
     */
    public void OnCollisionEnter(Collision other) {
        for (var i = 0; i < 3; i++) {
            if (Math.Abs(other.contacts[0].normal[i]) > 0.6f) { //if the veggie have collided with something
                var vegBody = gameObject.GetComponent<Rigidbody>();
                var reversedSpeed = vegBody.velocity;
                if (i != 1 || Math.Sign(other.contacts[0].normal[1]) != 1) { //todo test this
                    reversedSpeed[i] *= -0.8f;
                } else {
                    vegBody.velocity = Vector3.zero;
                    vegBody.useGravity = false;
                } gameObject.GetComponent<Rigidbody>().velocity = reversedSpeed;//reverse the velocity the angle the veggie collided with
            }
        }
    }
}
