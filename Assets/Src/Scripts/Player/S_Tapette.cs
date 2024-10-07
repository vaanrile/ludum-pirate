using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Tapette : MonoBehaviour
{

    public GameObject particleHit;
    public GameObject particleKill;


    public void kick(float hitbox, float repulseForce)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, hitbox);

        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.attachedRigidbody;
            if (rb != null && rb != GetComponent<Rigidbody>() && collider.gameObject.tag != "Tapette" && collider.gameObject.tag != "Player")
            {  
                if(collider.gameObject.tag == "Moskito")
                {
                    collider.gameObject.GetComponent<Moskito>().Kiecked();
                    StartCoroutine(WaitForEndKillParticle());
                }
                else
                {
                    //dire force
                    Vector3 direction = collider.transform.position - transform.position;
                    direction.Normalize();

                    //Force
                    rb.AddForce(direction * repulseForce, ForceMode.Impulse);
                    StartCoroutine(WaitForEndFlashParticle());

                    S_AbsInteractive interactive = rb.GetComponent<S_AbsInteractive>();
                    if (interactive != null)
                    {
                        interactive.Kicked();
                    }
                }
            }
        }
    }

    IEnumerator WaitForEndFlashParticle()
    {
        GameObject particleCurrent = Instantiate(particleHit, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(4f);
        Destroy(particleCurrent);
    }
    IEnumerator WaitForEndKillParticle()
    {
        GameObject particleCurrent = Instantiate(particleKill, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(4f);
        Destroy(particleCurrent);
    }

}
