using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Telephone : S_AbsInteractive
{
    [Header("Game Design Variables")]
    [SerializeField]
    private float radius;

    [SerializeField]
    private bool isActive;

    [SerializeField]
    private float duration;

    [SerializeField]
    private int nbDringSound;

    public ParticleSystem particle;

    private float timeBetweenDring;

    private AudioSource _audioSource;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void Awake()
    {
        timeBetweenDring = duration / (nbDringSound-1);
        _audioSource = GetComponent<AudioSource>();
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
            isActive = true;
            StartCoroutine(WaitForEndOfEncens());
            particle.Play();
        }
    }

    public override void Kicked()
    {
        base.Kicked();
        SetActive();
    }


    private void dring()
    {
        transform.DOShakeRotation(1f, 10, 10, 10, true, ShakeRandomnessMode.Harmonic);
    }

    IEnumerator WaitForEndOfEncens()
    {
        dring();
        for (int i = 0; i < nbDringSound-1; i++)
        {
            _audioSource.PlayOneShot(AudioManager.instance.findAudioClip("Phone_Ring"));
            yield return new WaitForSeconds(timeBetweenDring);
            dring();
        }
        isActive = false;
        Debug.Log("Son : Fin dring");
        particle.Stop();
    }
}
