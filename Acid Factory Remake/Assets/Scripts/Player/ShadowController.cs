using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowController : MonoBehaviour {
    private static Rigidbody sBody;
    // Start is called before the first frame update
    private void Start() {
        sBody = gameObject.GetComponent<Rigidbody>();
        //movePlayer(new Vector3(0f, 0.2f, 0f)); //todo this need to be done into a ray-firing
    }

    private void OnCollisionEnter(Collision collision) {
        movePlayer(Vector3.zero);
    }

    private void OnCollisionExit(Collision other) {
        gameObject.SetActive(false); //todo I need to learn how the hell to fire a ray to check for platform
    }

    public static void movePlayer(Vector3 movement) {
        sBody.velocity = movement;
    }
}
