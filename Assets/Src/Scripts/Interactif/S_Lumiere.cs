using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Lumiere : S_AbsInteractive
{
    public GameObject lumiere;
    private bool isOn = false;
    public override void Kicked()
    {
        base.Kicked();
        isOn = !isOn;
        lumiere.SetActive(isOn);
    }

}
