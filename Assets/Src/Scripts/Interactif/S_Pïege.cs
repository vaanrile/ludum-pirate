using System.Collections;
using System.Collections.Generic;
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

    private Coroutine _lightCoroutine;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
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
            Debug.Log("Piege Actif");
            objetEmissive.GetComponent<Renderer>().material = matActif;
            StartCoroutine(WaitForEndOfEncens());
            particleLight.Play();
        }
    }

    public override void Kicked()
    {
        base.Kicked();
        SetActive();
    }

    public void PiegeHitMoskito()
    {
        StopCoroutine(_lightCoroutine);
        Debug.Log("Son : moustique taser");
    }



    IEnumerator WaitForEndOfEncens()
    {
        yield return new WaitForSeconds(duration);
        isActive = false;
        particleLight.Stop();
        objetEmissive.GetComponent<Renderer>().material = matInactif;
        StopCoroutine(_lightCoroutine);
    }
}
