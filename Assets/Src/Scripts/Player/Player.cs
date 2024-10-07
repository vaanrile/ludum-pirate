using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
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
        TouchingObjects,
        Shaking,
        Hitting
    }

    public PlayerStatus GetCurrentPlayerStatus()
    {
        return status;
    }
    public void StartMove()
    {
        controller.StartMove();
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
    }

    public Transform GetBehindPlayerTransform()
    {
        return behindPlayer;
    }

    public PlayerController GetPlayerController()
    {
        return controller;
    }

    public void Cri()
    {

        Debug.Log("son : Aaaaargh ! Fuck !");
    }

}
