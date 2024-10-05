using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encens : MonoBehaviour
{
    [Header("Game Design Variables")]
    [SerializeField]
    private float radius;

    [SerializeField]
    private bool isActive;

    [SerializeField]
    private float duration;
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
            StartCoroutine(WaitForEndOfEncens());
        }

    }

    IEnumerator WaitForEndOfEncens()
    {
        yield return new WaitForSeconds(duration);
        isActive = false;
    }

}
