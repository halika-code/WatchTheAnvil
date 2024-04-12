using UnityEngine;

public class StopTools : MonoBehaviour {
    public void OnTriggerEnter(Collider other) {
        if (Character_Controller.getParentName(other.gameObject) is not "Player") {
            var rObj = gameObject.GetComponent<Rigidbody>();
            rObj.useGravity = false;
            rObj.velocity = Vector3.zero;
        }
    }
}
