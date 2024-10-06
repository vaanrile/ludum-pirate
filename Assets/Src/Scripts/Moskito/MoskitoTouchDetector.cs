using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Moskito))]
public class MoskitoTouchDetector : MonoBehaviour
{

    [SerializeField]
    [Tooltip("0 will never attach to objects, 1 will always attach to objects")]
    [Range(0, 1)]
    private float attachToObjectProbability = 1f;

    [SerializeField]
    private bool isAttachedToObject;

    public Action OnObjectAttached;

    List<GameObject> alreadyAttachedObjects = new List<GameObject>();

    Moskito moskito;
    Coroutine coWaitForAttachedObject;

    [SerializeField]
    private float timeAttachedMin = 2, timeAttachedMax = 3;



    private void Awake()
    {
        moskito = GetComponent<Moskito>();
        StartCoroutine(FlushAllAttachedObjectEveryXseconds(30));
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (alreadyAttachedObjects.Contains(collision.gameObject))
        {
            return;
        }
        if (collision.gameObject == moskito.GetPlayer().gameObject)
        {
            return;
        }
        alreadyAttachedObjects.Add(collision.gameObject);
        if (UnityEngine.Random.Range(0,1) < attachToObjectProbability)
        {
            OnObjectAttached?.Invoke();
            if (coWaitForAttachedObject != null)
            {
                StopCoroutine(coWaitForAttachedObject);
            }
            coWaitForAttachedObject = StartCoroutine(WaitForStopAttachingObject(UnityEngine.Random.Range(timeAttachedMin,timeAttachedMax)));
        }
    }

    public bool IsAttachedToObject()
    {
        return isAttachedToObject;
    }

    private IEnumerator WaitForStopAttachingObject(float duration)
    {
        isAttachedToObject = true;
        yield return new WaitForSeconds(duration);
        isAttachedToObject = false;
    }

    public void StopAttachingObject()
    {
        if (coWaitForAttachedObject != null)
        {
            StopCoroutine(coWaitForAttachedObject);
        }
        isAttachedToObject = false;
    }

    private IEnumerator FlushAllAttachedObjectEveryXseconds(float duration)
    {
        yield return new WaitForSeconds(duration);
        alreadyAttachedObjects.Clear();
        FlushAllAttachedObjectEveryXseconds(30);
    }



}
