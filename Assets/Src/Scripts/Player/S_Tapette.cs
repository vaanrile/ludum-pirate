using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Tapette : MonoBehaviour
{
    public void kick(float hitbox, float repulseForce)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, hitbox);

        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.attachedRigidbody;
            if (rb != null && rb != GetComponent<Rigidbody>())
            {
                //dire force
                Vector3 direction = collider.transform.position - transform.position;
                direction.Normalize();

                //Force
                rb.AddForce(direction * repulseForce, ForceMode.Impulse);

                S_AbsInteractive interactive = rb.GetComponent<S_AbsInteractive>();
                if (interactive != null)
                {
                    interactive.Kicked();
                }
            }
        }
    }
}
