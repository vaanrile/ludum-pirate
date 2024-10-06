using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_Event : MonoBehaviour
{
    public PlayerController playerController;
    public void TapetteHit()
    {
        playerController.ApplyRepulseForce();
    }
}
