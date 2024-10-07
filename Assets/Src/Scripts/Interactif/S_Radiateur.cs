using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Radiateur : S_AbsInteractive
{
    [Header("Game Design Variables")]
    [SerializeField]
    private float radius;

    [SerializeField]
    private bool isActive;

    [SerializeField]
    private float duration;

    public GameObject screenInactif;
    public GameObject screenActif;
    public ParticleSystem hotTrails;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
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
            _audioSource.PlayOneShot(AudioManager.instance.findAudioClip("Radiator_Turn_On"));
            screenInactif.SetActive(false);
            screenActif.SetActive(true);
            hotTrails.Play();
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
        screenInactif.SetActive(true);
        screenActif.SetActive(false);
        hotTrails.Stop();
    }
}
