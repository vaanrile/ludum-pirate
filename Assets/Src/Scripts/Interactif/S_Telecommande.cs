using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Telecommande : MonoBehaviour
{
    public S_TV tv;

    private void OnTriggerEnter(Collider other)
    {
        tv.On();
    }

}
