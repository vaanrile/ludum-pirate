using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[Serializable]
public struct AudioClips
{
    public AudioClip audioClip;
    public string audioClipName;
}
public class AudioManager : Game
{
    private AudioSource _2DAudioSource;
    public Transform sfxObjectsList;
    public List <AudioSource> sfxSourcesList;
    public List<AudioSource> musicSourcesList;
    private int _musicIndex;

    public AudioClips[] audioClipsArray; 
    void Start()
    {
        _2DAudioSource =GetComponent<AudioSource>();
        foreach (Transform obj in sfxObjectsList) 
        {
            sfxSourcesList.Add(obj.GetComponent<AudioSource>());
        }
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            FadeSound(musicSourcesList[_musicIndex], 1, 0);
            _musicIndex++;
            FadeSound(musicSourcesList[_musicIndex], 1, 1);
        }*/
    }
    private void StartSound(AudioSource audioToStart)
    {
        audioToStart.Play();
    }
    private void StopSound(AudioSource audioToStop)
    {
        audioToStop.Stop();
    }
    public void PlaySound2DByName(string soundName)
    {
        _2DAudioSource.PlayOneShot(findAudioClip(soundName));
    }
    public void PlaySoundAttached(string soundName, GameObject objetAttached) 
    {
        
    }
    public void PlaySoundAtLocation(string soundName, Vector3 location)
    {

    }
    public void FadeSound(AudioSource audioToFade, float timeToFade, float targetVolume)
    {
        DOTween.To(() => audioToFade.volume, x => audioToFade.volume = x, targetVolume, timeToFade);
    }
    private AudioClip findAudioClip(string soundName)
    {
        AudioClip audioClipToPlay = null;
        foreach(AudioClips aud in audioClipsArray)
        {
            if (aud.audioClipName == soundName)
            {
                audioClipToPlay = aud.audioClip;
            }
        }
        return audioClipToPlay;
    }
}
