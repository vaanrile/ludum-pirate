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
public class AudioManager : MonoBehaviour
{
    private AudioSource _2DAudioSource;
    public Transform sfxObjectsList;
    public List <AudioSource> sfxSourcesList;
    public List<AudioSource> musicSourcesList;
    private int _musicIndex;

    public static AudioManager instance;


    public AudioClips[] audioClipsArray;
    public AudioClip[] footstepArray;
    public AudioClip[] swatterArray;
    public AudioClip[] hitArray;

    private Tween _currentTween;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There is already an instance of AudioManager in the scene, only one can be instanciated.");
        }
    }

    void Start()
    {
        _2DAudioSource = GetComponent<AudioSource>();
        foreach (Transform obj in sfxObjectsList)
        {
            sfxSourcesList.Add(obj.GetComponent<AudioSource>());
        }
        
    }

    private void DelayStart()
    {
        PlaySound2DByName("Wake_Up");
    }
    public void StartSound()
    {
        PlaySound2DByName("Moskito_Init");
        Invoke("DelayStart", 0.5f);
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            FadeSound(musicSourcesList[_musicIndex], 1, 0);
            if (_musicIndex<musicSourcesList.Count-1) 
            {
                _musicIndex++;
            }
            else
            {
                _musicIndex = 0;
            }           
            FadeSound(musicSourcesList[_musicIndex], 1, 1);
        }*/

        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            PlaySound2DByName("lol");
        }*/
    }

    public void PlayLol()
    {
        PlaySound2DByName("lol");
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
        if (_currentTween != null)
        {
            audioToFade.DOKill();
        }        
        _currentTween = DOTween.To(() => audioToFade.volume, x => audioToFade.volume = x, targetVolume, timeToFade);
    }
    public AudioClip findAudioClip(string soundName)
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
