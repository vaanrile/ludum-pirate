using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Lumiere : S_AbsInteractive
{
    public GameObject lumiere;
    private bool isOn = false;
    public PlayerController pc;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    public override void Kicked()
    {
        base.Kicked();
        isOn = !isOn;
        lumiere.SetActive(isOn);
        pc.setCamera(isOn);
        _audioSource.PlayOneShot(AudioManager.instance.findAudioClip("Light_Click"));
    }

}
