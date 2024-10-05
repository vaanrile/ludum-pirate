using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class S_AbsInteractive : MonoBehaviour
{
    public virtual void Kicked()
    {
        //transform.DOShakePosition(0.3f, 10, 5);
        transform.DOShakeRotation(0.2f, 40, 10,90,true,ShakeRandomnessMode.Harmonic);
    }
}
