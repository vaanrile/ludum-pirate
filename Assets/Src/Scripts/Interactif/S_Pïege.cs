using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class S_Pïege : S_AbsInteractive
{
    [Header("Game Design Variables")]
    [SerializeField]
    private float radius;

    [SerializeField]
    private bool isActive;

    [SerializeField]
    private float duration;

    public Material matActif;
    public Material matInactif;
    public GameObject objetEmissive;
    public ParticleSystem particleLight;

    public GameObject particleHit;

    private AudioSource _audioSource;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void Awake()
    {
        GetComponent<SphereCollider>().radius = radius;
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
            _audioSource.PlayOneShot(AudioManager.instance.findAudioClip("Trap_Turn_On"));
            objetEmissive.GetComponent<Renderer>().material = matActif;
            StartCoroutine(WaitForEndOfActive());
            GetComponent<SphereCollider>().enabled=true;
            particleLight.Play();
        }
    }

    public override void Kicked()
    {
        base.Kicked();
        SetActive();
    }

    public void PiegeHitMoskito(GameObject moskito)
    {
        Debug.Log("Son : moustique taser");
        _audioSource.PlayOneShot(AudioManager.instance.findAudioClip("Eletric_Zap"));
        StartCoroutine(WaitForEndFlashParticle(moskito));
    }

    IEnumerator WaitForEndFlashParticle(GameObject moskito)
    {
        GameObject particleCurrent = Instantiate(particleHit, moskito.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(4f);
        Destroy(particleCurrent);
    }

    IEnumerator WaitForEndOfActive()
    {
        yield return new WaitForSeconds(duration);
        isActive = false;
        GetComponent<SphereCollider>().enabled = false;
        particleLight.Stop();
        objetEmissive.GetComponent<Renderer>().material = matInactif;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Moskito")
        {
            PiegeHitMoskito(other.gameObject);
        }
    }

}
