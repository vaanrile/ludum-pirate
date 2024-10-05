using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private Moskito moskito;

    [SerializeField]
    private PlayerController controller;

    [SerializeField]
    private PlayerStatus status = PlayerStatus.Idle;
    private bool blockStatus;

    [SerializeField]
    private Rigidbody rb;

    private Coroutine blockStatusCoroutine;

    [Header("Game Design Variables")]
    [SerializeField]
    private float touchObjectDuration = 2f;
    [SerializeField]
    private float hitDuration = 1f;

    [SerializeField]
    private Transform behindPlayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<PlayerController>();
    }

    public enum PlayerStatus
    {
        Idle,
        WalkingOppositeMoskito,
        WalkingTowardsMoskito,
        TouchingObjects,
        Shaking,
        Hitting
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,moskito.GetDangerZoneRadius());
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, moskito.GetConfortZoneRadius());
    }

    public PlayerStatus GetCurrentPlayerStatus()
    {
        return status;
    }

    private void Update()
    {
        SetCurrentPlayerStatus();
    }

    private void OnTouchObject()
    {
        if(blockStatusCoroutine != null)
        {
            StopCoroutine(blockStatusCoroutine);
        }
        blockStatusCoroutine = StartCoroutine(BlockStatusForXSeconds(touchObjectDuration, status = PlayerStatus.TouchingObjects));
    }
    public void OnTryingToHit()
    {
        if (blockStatus)
        {
            return;
        }
        if (blockStatusCoroutine != null)
        {
            StopCoroutine(blockStatusCoroutine);
        }
        blockStatusCoroutine = StartCoroutine(BlockStatusForXSeconds(hitDuration, status = PlayerStatus.Hitting));
    }


    private IEnumerator BlockStatusForXSeconds(float seconds, PlayerStatus newStatus)
    {

        blockStatus = true;
        status = newStatus;
        yield return new WaitForSeconds(seconds);
        blockStatus = false;
    }

    private void SetCurrentPlayerStatus()
    {
        if (blockStatus)
        {
            return;
        }
        if (controller.GetVelocity().magnitude < 0.01f) { 
            status = PlayerStatus.Idle;
            return;
        }

        if (Vector3.Dot(controller.GetVelocity(), transform.position - moskito.transform.position) < 0)
        {
            status = PlayerStatus.WalkingTowardsMoskito;
        }
        else
        {
            status = PlayerStatus.WalkingOppositeMoskito;
        }
    }

    public Transform GetBehindPlayerTransform()
    {
        return behindPlayer;
    }



}
