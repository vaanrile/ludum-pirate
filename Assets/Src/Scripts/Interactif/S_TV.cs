using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_TV : MonoBehaviour
{
    public GameObject onEffect;
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Tapette")
        {
            Off();
        }
    }

    public void On()
    {
        onEffect.SetActive(true);
    }

    private void Off()
    {
        onEffect.SetActive(false);
    }
}
