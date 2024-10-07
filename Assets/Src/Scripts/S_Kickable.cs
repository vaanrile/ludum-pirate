using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Kickable : S_AbsInteractive
{
    public int mode = 0;
    [SerializeField] private bool _onlyTriggeredByTapette;
    [SerializeField] private AudioSource _audioSource;
    public string audioName;

    private void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.spatialBlend = 1;        
    }

    public override void Kicked()
    {
        if (mode == 0)
        {
            _audioSource.PlayOneShot(AudioManager.instance.hitArray[Random.Range(0, AudioManager.instance.hitArray.Length-1)]);
        }
        else if (mode == 1)
        {
            _audioSource.PlayOneShot(AudioManager.instance.findAudioClip(audioName));
        }
        else
        {
            _audioSource.PlayOneShot(AudioManager.instance.hitArray[Random.Range(0, AudioManager.instance.hitArray.Length - 1)]);
            _audioSource.PlayOneShot(AudioManager.instance.findAudioClip(audioName));
        }
        _audioSource.pitch = Random.Range(1.5f,3);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (_onlyTriggeredByTapette) 
        {
            return;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Kicked();
        }
    }
}
