using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_TV : S_AbsInteractive
{
    public GameObject onEffect;
    void Update()
    {
        
    }

    public void On()
    {
        onEffect.SetActive(true);
    }

    private void Off()
    {
        onEffect.SetActive(false);
    }

    public override void Kicked()
    {
        Off();
    }
}
