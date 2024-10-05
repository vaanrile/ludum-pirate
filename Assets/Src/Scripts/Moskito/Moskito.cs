using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moskito : MonoBehaviour
{
    [SerializeField] Material moskitoConfort;
    [SerializeField] Material moskitoDanger;
    [SerializeField] Material moskitoFar;

    [SerializeField] MeshRenderer meshRenderer;

    [SerializeField] MoskitoZone moskitoZone = MoskitoZone.Far;
    [SerializeField] MoskitoStatus moskitoStatus = MoskitoStatus.GoSafe;
    [SerializeField] Player player;

    [Header("GameDesignVariables")]
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

    private float timeSincePreparingAttack;
    private float timeSinceItAttacked;
    private float timeSinceBeingFar;
    private float timeBeforeGoingBackMin;
    private float timeBeforeGoingBackMax;




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
        GoOutsidePhoneLight

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

    private void Update()
    {
       UpdateMoskitoZone();
       UpdateMoskitoStatus();
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
        if (moskitoStatus == MoskitoStatus.AfterAttack && timeSinceItAttacked < timeAfterAttack) {
            timeSinceItAttacked += Time.deltaTime;
            return;
        }
        else
        {
            timeSinceItAttacked = 0;
        }
        switch (moskitoZone)
        {
            case MoskitoZone.Confort:
                switch (player.GetCurrentPlayerStatus())
                {
                    case Player.PlayerStatus.Idle:
                        SetMoskitoStatus(MoskitoStatus.GoClose);
                        break;
                    case Player.PlayerStatus.WalkingOppositeMoskito:
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
                        PrepareToAttack();
                        break;
                    case Player.PlayerStatus.WalkingOppositeMoskito:
                        PrepareToAttack();
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
                SetMoskitoStatus(MoskitoStatus.GoClose);
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
        moskitoStatus = _newStatus;

    }

    private void PrepareToAttack()
    {
        Debug.Log($"Hello {timeSincePreparingAttack}");
        timeSincePreparingAttack += Time.deltaTime;

        if (timeSincePreparingAttack > timePreparingAttack)
        {
            timeSincePreparingAttack = 0;
            SetMoskitoStatus(MoskitoStatus.AfterAttack);
            // HERE DEAL POSSIBLE DAMAGE => THE MOSKITO A PIQU� !
        }
        else
        {
            SetMoskitoStatus(MoskitoStatus.PrepareToAttack);
        }
    }

    private void OnDrawGizmos()
    {
        UpdateMoskitoZone();
    }
}
