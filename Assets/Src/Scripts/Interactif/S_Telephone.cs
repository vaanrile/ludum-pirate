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


    private float timeBetweenDring;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void Awake()
    {
        timeBetweenDring = duration / (nbDringSound-1);
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
            Debug.Log("Tel Actif");
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
        Debug.Log("Dring");
        for (int i = 0; i < nbDringSound-1; i++)
        {
            yield return new WaitForSeconds(timeBetweenDring);
            Debug.Log("Dring");
        }
        isActive = false; 
        Debug.Log("Tel Stop");
    }
}
