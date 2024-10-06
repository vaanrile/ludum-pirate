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
    public Light lightPiege;

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
            _lightCoroutine = StartCoroutine(LightAnim());
            StartCoroutine(WaitForEndOfEncens());
            lightPiege.gameObject.SetActive(true);
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
        StartCoroutine(FlashLight());
        Debug.Log("Son : moustique taser");
    }

    IEnumerator FlashLight()
    {
        lightPiege.intensity = 1f;
        yield return new WaitForSeconds(0.15f);
        _lightCoroutine = StartCoroutine(LightAnim());
    }

    IEnumerator LightAnim()
    {
        while (true)
        {
            lightPiege.intensity = 0.5f;
            yield return new WaitForSeconds(0.1f);
            lightPiege.intensity = 0.4f;
            yield return new WaitForSeconds(0.1f);
            lightPiege.intensity = 0.6f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator WaitForEndOfEncens()
    {
        yield return new WaitForSeconds(duration);
        isActive = false;
        Debug.Log("Piege Stop");
        objetEmissive.GetComponent<Renderer>().material = matInactif;
        lightPiege.gameObject.SetActive(true);
        StopCoroutine(_lightCoroutine);
    }
}
