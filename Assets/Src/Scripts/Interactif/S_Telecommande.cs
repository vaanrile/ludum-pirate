using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Telecommande : MonoBehaviour
{
    public S_TV tv;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        tv.On();
        _audioSource.PlayOneShot(AudioManager.instance.findAudioClip("Remote_Click"));

    }

}
