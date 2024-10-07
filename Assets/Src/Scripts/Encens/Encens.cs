using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encens : S_AbsInteractive
{
    [Header("Game Design Variables")]
    [SerializeField]
    private float radius;

    [SerializeField]
    private bool isActive;

    [SerializeField]
    private float duration;

    public ParticleSystem actifTrait;
    public ParticleSystem actifSmoke;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,radius);
    }

    public float GetRadius()
    {
        return radius; 
    }

    public bool IsActive()
    {
        return isActive;
    }

    public void SetActive()
    {
        if (!isActive)
        {
            _audioSource.PlayOneShot(AudioManager.instance.findAudioClip("Encens_Turn_On"));
            isActive = true;
            actifTrait.Play();
            actifSmoke.Play();
            StartCoroutine(WaitForEndOfEncens());
        }

    }

    public override void Kicked()
    {
        base.Kicked();
        SetActive();
    }

    IEnumerator WaitForEndOfEncens()
    {
        yield return new WaitForSeconds(duration);
        isActive = false;
        actifTrait.Stop();
        actifSmoke.Stop();
    }

}
