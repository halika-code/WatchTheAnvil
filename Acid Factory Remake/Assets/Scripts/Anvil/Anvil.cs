using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * <date>17/09/2023</date>
 * <author>Gyula Attila Kovacs</author>
 * <summary>A container script with logic for individual anvils</summary>
 */

public class Anvil {
    public int aTimer;
    private Rigidbody aBody;
    private Rigidbody tBody;
    /**
     * <summary>A flag inside the anvils that is set to true the frame the </summary>
     */
    public bool isFlying;

    /**
     * <summary>A default constructor for an Anvil object
     * <para>that has a default difficulty of 1</para></summary>
     */
    public Anvil(GameObject anvil) {
        aTimer = 10;
        prepAnvil(anvil);
    }

    /**
     * <summary>A modified constructor for an Anvil object
     * <para>that has a definable difficulty</para></summary>
     * <param name="diff">The desired difficulty ranging from 0 to 3 (and above)</param>
     * <param name="anvil">A reference to the new gameObject</param>
     * <remarks>a difficulty score of 4 and above will be handled as 3 while a value of -1 and less will set the difficulty to impossible</remarks>
     */
    public Anvil(GameObject anvil, int diff) {
        aTimer = diff;
        prepAnvil(anvil);
    }
    
    private void prepAnvil(GameObject anvil) {
        isFlying = false;
        aBody = anvil.GetComponent<Rigidbody>();
        aBody.position = new Vector3(0f, 50f, 0f);
        setTarget(aBody.GetComponentsInChildren<Rigidbody>());
    }

    /**
     * <summary>Sends the Anvil barrelling down towards the player</summary>
     */
    public IEnumerator dropAnvil() {
        var tarVel = aBody.velocity;
        aBody.velocity = new Vector3(tarVel.x, -100f, tarVel.z);
        do {
            yield return new WaitForFixedUpdate();
        } while (!haveLanded());
        AnvilManager.freezeAnvil();
    }

    /**
     * <summary>Attempts to freeze the anvil in place while keeping it functional</summary>
     */
    public void freezeAnvil() {
        aBody.velocity = new Vector3(aBody.velocity.x, 0f, aBody.velocity.z);
    }

    public GameObject getTarget() {
        return getShadow().gameObject;
    }

    public Rigidbody getAnvilBody() {
        return aBody;
    }

    private bool haveLanded() {
        if (isFlying) {
            return aBody.velocity.y > 0f;
        } return true;

    }
    
    /**
     * <summary>Sets the target of the anvil</summary>
     * <remarks>Finds the proper rigidbody based on the name.
     * <para>Anvil's name will be varying while target name is always "Target"</para></remarks>
     */
    private void setTarget(IEnumerable<Rigidbody> shadows) {
        foreach (var shadow in shadows) {
            if (shadow.name is "Target") {
                tBody = shadow;
            }
        }
    }
    
    public void setTargetPos(Vector3 pos) {
        aBody.position = new Vector3(pos.x, aBody.position.y, pos.z);
        tBody.position = pos;
    }

    private Rigidbody getShadow() {
        return tBody;
    }
}
