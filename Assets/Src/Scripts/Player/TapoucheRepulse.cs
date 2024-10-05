using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapoucheRepulse : MonoBehaviour
{
    public float repulseForce = 50f;

    private void OnTriggerEnter(Collider other)
    {
        //OnlyRigidBody
        Rigidbody otherRigidbody = other.attachedRigidbody;

        if (otherRigidbody != null)
        {
            //Dire force
            Vector3 direction = other.transform.position - transform.position;
            direction.Normalize();

            //Force
            otherRigidbody.AddForce(direction * repulseForce, ForceMode.Impulse);
        }
    }
}
