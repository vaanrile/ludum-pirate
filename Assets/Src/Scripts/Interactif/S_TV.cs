using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_TV : S_AbsInteractive
{
    public GameObject onEffect;
    public Light lightTV;
    private Coroutine _screenCoroutine;
    public Transform tvScreen;
    public float tvAnimDelay;
    void Update()
    {
        
    }

    public void On()
    {
        onEffect.SetActive(true);
        _screenCoroutine = StartCoroutine(ScreenAnim());
    }

    private void Off()
    {
        StopCoroutine(_screenCoroutine);
        onEffect.SetActive(false);
    }

    public override void Kicked()
    {
        base.Kicked();
        Off();
    }

    IEnumerator ScreenAnim()
    {
        while (true)
        {
            tvScreen.localScale = new Vector3(-1, 1, 1);
            yield return new WaitForSeconds(tvAnimDelay);
            lightTV.intensity = 0.9f;
            tvScreen.localScale = new Vector3(-1, -1, 1);
            yield return new WaitForSeconds(tvAnimDelay);
            lightTV.intensity = 1f;
            tvScreen.localScale = new Vector3(1, -1, 1);
            yield return new WaitForSeconds(tvAnimDelay);
            lightTV.intensity = 0.9f;
            tvScreen.localScale = new Vector3(1, 1, 1);
            yield return new WaitForSeconds(tvAnimDelay);
            lightTV.intensity = 1f;
        }
        
    }
}
