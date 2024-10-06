using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class Moskito : MonoBehaviour
{
    [Header("__MATERIALS")]
    [SerializeField] Material moskitoConfort;
    [SerializeField] Material moskitoDanger;
    [SerializeField] Material moskitoFar;
    [SerializeField] MeshRenderer meshRenderer;

    [Header("__Enum")]
    [SerializeField] MoskitoZone moskitoZone = MoskitoZone.Far;
    [SerializeField] MoskitoStatus moskitoStatus = MoskitoStatus.GoSafe;
    [SerializeField] PlayerDirection playerDirection = PlayerDirection.WalkingToward;
    [Header("__OTHER GameObjects")]
    [SerializeField] Player player;
    [SerializeField] Encens encens;

    private Rigidbody rb;

    [Header("__GameDesignVariables")]
    [SerializeField]
    private float distanceZoneDanger = 15f;

    [SerializeField]
    private float distanceZoneConfort = 30f;
    
    [SerializeField]
    private float timePreparingAttack = 2f;
    [SerializeField]
    private float timeAfterAttack = 2f;

    [SerializeField]
    private float timeBeforeGoingBackWhenFarMin = 5f;

    [SerializeField]
    private float timeBeforeGoingBackWhenFarMax = 15f;

    [SerializeField]
    private float timeSneakingMin = 5f;

    [SerializeField]
    private float timeSneakingMax = 15f;


    [Header("__DEV VARIABLES")]
    private float timeSincePreparingAttack;
    private float timeSinceItAttacked;
    private float timeSinceBeingFar;
    private float timeBeforeGoingBack;
    private float timeSinceSneaking;
    private float timeBeforeStopSneaking;



    [SerializeField]
    private bool forceStatus;

    [SerializeField]
    private bool closeToEncens;

    private Coroutine forceStatusCoroutine;

    public enum MoskitoZone
    {
        Confort,
        Danger,
        Far
    }

    public enum MoskitoStatus
    {
        GoClose,
        PrepareToAttack,
        AfterAttack,
        Sneak,
        GoSafe,
        GoBehind,
        Encens,
        LightMod,
        GoOutsidePhoneLight,
        StayFar

    }

    public enum PlayerDirection
    {
        WalkingBackward,
        WalkingToward,
        Idle
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        UpdateEncensZone();
        UpdateMoskitoZone();
        UpdateMoskitoStatus();
        UpdatePlayerDirection();
    }

    private void UpdateEncensZone()
    {
        if ((transform.position - encens.transform.position).magnitude < encens.GetRadius())
        {
            closeToEncens = true;
        }
        else { 
            closeToEncens = false;
        }
    }

    public void ChangeMoskitoZone(MoskitoZone _moskitoZone)
    {
        if(moskitoZone != _moskitoZone)
        {
            moskitoZone = _moskitoZone;

            switch (moskitoZone)
            {
                case MoskitoZone.Confort:
                    meshRenderer.material = moskitoConfort;
                    break;
                case MoskitoZone.Danger:
                    meshRenderer.material = moskitoDanger; 
                    break;
                case MoskitoZone.Far:
                    meshRenderer.material = moskitoFar;
                    break;
                default:
                    break;
            }
        }
    }


    public float GetConfortZoneRadius()
    {
        return distanceZoneConfort;
    }

    public float GetDangerZoneRadius()
    {
        return distanceZoneDanger;
    }

    private void UpdatePlayerDirection()
    {
        if (Vector3.Dot(player.GetPlayerController().GetVelocity(), transform.position - player.transform.position) < 0)
        {
            playerDirection = PlayerDirection.WalkingBackward;
        }
        else
        {
            playerDirection = PlayerDirection.WalkingToward;
        }
    }

    private void UpdateMoskitoZone()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > distanceZoneConfort)
        {
            ChangeMoskitoZone(Moskito.MoskitoZone.Far);
            return;
        }

        if(Vector3.Distance(transform.position, player.transform.position) > distanceZoneDanger)
        {
            ChangeMoskitoZone(MoskitoZone.Confort);
        }
        else
        {
            ChangeMoskitoZone(Moskito.MoskitoZone.Danger);
        }
    }

    public MoskitoStatus GetMoskitoStatus()
    {
        return moskitoStatus;
    }

    private void UpdateMoskitoStatus()
    {
        if (forceStatus) {
            return;
        }

        if (closeToEncens && encens.IsActive())
        {
            SetMoskitoStatus(MoskitoStatus.Encens);
            return;
        }

        if (moskitoStatus == MoskitoStatus.AfterAttack && timeSinceItAttacked < timeAfterAttack) {
            timeSinceItAttacked += Time.deltaTime;
            return;
        }
        if (moskitoStatus == MoskitoStatus.Sneak && timeSinceSneaking < timeBeforeStopSneaking)
        {
            timeSinceSneaking += Time.deltaTime;
            if(timeSinceSneaking > timeBeforeStopSneaking)
            {
                ForceStatusForXSeconds(MoskitoStatus.GoSafe,3);
            }
            return;
        }

        switch (moskitoZone)
        {
            case MoskitoZone.Confort:
                switch (player.GetCurrentPlayerStatus())
                {
                    case Player.PlayerStatus.Idle:
                        SetMoskitoStatus(MoskitoStatus.GoClose);
                        break;
                    case Player.PlayerStatus.TouchingObjects:
                        SetMoskitoStatus(MoskitoStatus.GoBehind);
                        break;
                    case Player.PlayerStatus.Shaking:
                        SetMoskitoStatus(MoskitoStatus.GoBehind);
                        break;
                    case Player.PlayerStatus.Hitting:
                        SetMoskitoStatus(MoskitoStatus.GoSafe);
                        break;
                    default:
                        SetMoskitoStatus(MoskitoStatus.GoBehind);
                        break;
                }
                break;
            case MoskitoZone.Danger:
                switch (player.GetCurrentPlayerStatus())
                {
                    case Player.PlayerStatus.Idle:
                        if(rb.velocity.sqrMagnitude >= player.GetPlayerController().GetVelocity().sqrMagnitude)
                        {
                            PrepareToAttack();
                        }
                        else
                        {
                            switch (playerDirection)
                            {
                                case PlayerDirection.WalkingToward:
                                    PrepareToSneak();
                                    break;
                                case PlayerDirection.WalkingBackward:
                                    PrepareToAttack();
                                    break;
                            }
                        }
                        
                        break;
                    case Player.PlayerStatus.TouchingObjects:
                        SetMoskitoStatus(MoskitoStatus.GoSafe);
                        break;
                    case Player.PlayerStatus.Shaking:
                        SetMoskitoStatus(MoskitoStatus.GoBehind);
                        break;
                    case Player.PlayerStatus.Hitting:
                        SetMoskitoStatus(MoskitoStatus.GoSafe);
                        break;
                    default:
                        PrepareToAttack();
                        break;
                }
                break;
            case MoskitoZone.Far:
                PrepareToComeBack();
                break;
            default:
                break;
        }
    }

    private void SetMoskitoStatus(MoskitoStatus _newStatus)
    {

        if (moskitoStatus == _newStatus)
        {
            return;
        }
        if (moskitoStatus != MoskitoStatus.AfterAttack) {
            timeSinceItAttacked = 0;
        }

        if(moskitoStatus != MoskitoStatus.PrepareToAttack)
        {
            timeSincePreparingAttack = 0;
        }
        if (moskitoStatus != MoskitoStatus.StayFar)
        {
            timeSinceBeingFar = 0;
        }
        if (moskitoStatus != MoskitoStatus.Sneak)
        {
            timeSinceSneaking = 0;
        }
        moskitoStatus = _newStatus;
    }

    private void PrepareToAttack()
    {
        timeSincePreparingAttack += Time.deltaTime;

        if (timeSincePreparingAttack > timePreparingAttack)
        {
            SetMoskitoStatus(MoskitoStatus.AfterAttack);
            // HERE DEAL POSSIBLE DAMAGE => THE MOSKITO A PIQUé !
                // ...
        }
        else
        {
            SetMoskitoStatus(MoskitoStatus.PrepareToAttack);
        }
    }

    private void PrepareToComeBack()
    {
        if(moskitoStatus == MoskitoStatus.GoClose)
        {
            return;
        }
        
        if(moskitoStatus != MoskitoStatus.StayFar)
        {
            SetMoskitoStatus(MoskitoStatus.StayFar);
            timeBeforeGoingBack = UnityEngine.Random.Range(timeBeforeGoingBackWhenFarMin, timeBeforeGoingBackWhenFarMax);
            return;
        }
        timeSinceBeingFar += Time.deltaTime;
        if(timeSinceBeingFar > timeBeforeGoingBack)
        {
            SetMoskitoStatus(MoskitoStatus.GoClose);
        }
    }

    private void PrepareToSneak()
    {
        if (moskitoStatus != MoskitoStatus.Sneak)
        {
            SetMoskitoStatus(MoskitoStatus.Sneak);
            timeBeforeStopSneaking = UnityEngine.Random.Range(timeSneakingMin, timeSneakingMax);
            return;
        }
    }

    private void ForceStatusForXSeconds(MoskitoStatus status,float duration)
    {
        if(forceStatusCoroutine != null)
        {
            StopCoroutine(forceStatusCoroutine);
        }
        forceStatusCoroutine = StartCoroutine(ForceStatusCoroutine(status,duration));
        
    }

    private IEnumerator ForceStatusCoroutine(MoskitoStatus status, float duration)
    {
        moskitoStatus = status;
        forceStatus = true;
        yield return new WaitForSeconds(duration);
        forceStatus = false;
    }


    private void OnDrawGizmos()
    {
        UpdateMoskitoZone();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, GetDangerZoneRadius());
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, GetConfortZoneRadius());
    }

    public Encens GetEncens()
    {
        return encens;
    }
}
