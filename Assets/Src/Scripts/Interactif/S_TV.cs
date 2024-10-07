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
    [SerializeField]private AudioSource _audioSource;

    void Update()
    {
        
    }

    public void On()
    {       
        if (!onEffect.activeSelf)
        {
            onEffect.SetActive(true);
            _screenCoroutine = StartCoroutine(ScreenAnim());
            _audioSource.PlayOneShot(AudioManager.instance.findAudioClip("Tv_Turning_On"));
        }       
    }

    private void Off()
    {
        if (onEffect.activeSelf)
        {
            _audioSource.PlayOneShot(AudioManager.instance.findAudioClip("Tv_Turning_Off"));
            StopCoroutine(_screenCoroutine);
            onEffect.SetActive(false);            
        }
       
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
