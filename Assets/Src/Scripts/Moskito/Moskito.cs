using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using static Player;
using UnityEngine.Device;

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
    [SerializeField] PlayerOrientation playerOrientation = PlayerOrientation.FacingMoskito;
    [SerializeField] MoskitoZone patrollingZone = MoskitoZone.None;
    [SerializeField] MoskitoZone lastPatrollingZone = MoskitoZone.None;
    [Header("__OTHER GameObjects")]
    [SerializeField] Player player;
    [SerializeField] Encens encens;
    [SerializeField] S_Radiateur radiateur;
    [SerializeField] S_Telephone tel;
    MoskitoTouchDetector moskitoTouchDetector;
    MoskitoController moskitoController;

    private Rigidbody rb;

    [Header("__GameDesignVariables")]
    [SerializeField]
    private float distanceZoneDanger = 15f;

    [SerializeField]
    private float distanceZoneConfort = 30f;

    [SerializeField]
    [Tooltip("-1 will always be BackToMoskito, 1 will always be FaceToMoskito, 0 is 180°")]
    [Range(-1, 1)]
    private float faceMoskitoAngle = 0f;

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

    private float timePreparingAttack = 2f;
    [SerializeField]
    private float timePreparingAttackMin = 4f;
    [SerializeField]
    private float timePreparingAttackMax = 6f;

    private float timeSincePatrolling;
    private float timeBeforeStopPatrolling;
    [SerializeField]
    private float timePatrollingMin = 4f;
    [SerializeField]
    private float timePatrollingMax = 6f;

    BoxCollider moskitoBox;

    private ParticleSystem bzzbzz;

    private bool isInvisible = false;

    [SerializeField]
    private bool forceStatus;

    [SerializeField]
    private bool closeToEncens;

    [SerializeField]
    private bool closeToTelephone;


    private Coroutine forceStatusCoroutine;

    private AudioSource _audioSource;
    [SerializeField]
    private float fadeAudioDuration;

    private bool _canMove;
    private bool _isAlive = true;

    public enum MoskitoZone
    {
        Confort,
        Danger,
        Far,
        Encens,
        Telephone,
        None
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
        Telephone,
        LightMod,
        GoOutsidePhoneLight,
        StayFar,
        StayConfort,
        StayDanger

    }

    public enum PlayerDirection
    {
        WalkingBackward,
        WalkingToward,
        Idle
    }

    public enum PlayerOrientation
    {
        FacingMoskito,
        BackToMoskito
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moskitoTouchDetector = GetComponent<MoskitoTouchDetector>();
        _audioSource = GetComponent<AudioSource>();
        Invoke("Aaah", 0.3f);
        moskitoController = GetComponent<MoskitoController>();
        bzzbzz = GetComponent<ParticleSystem>();
    }
    private void Aaah()
    {
        //_audioSource.PlayOneShot(AudioManager.instance.findAudioClip("Moskito_Init"),2);
    }

    public void StartMove()
    {
        _canMove = true;
        _audioSource.Play();
    }

    private void Update()
    {
        if (!_canMove) 
        {
            return;            
        }
        UpdateEncensZone();
        UpdateTelZone();
        UpdateRadiateurZone();
        UpdateMoskitoZone();
        UpdatePlayerDirection();
        UpdatePlayerOrientation();
        UpdateMoskitoStatus();

    }

    private void UpdatePlayerOrientation()
    {
        if (Vector3.Dot(player.transform.forward, (player.transform.position - transform.position).normalized) < faceMoskitoAngle)
        {
            playerOrientation = PlayerOrientation.FacingMoskito;
        }
        else
        {
            playerOrientation = PlayerOrientation.BackToMoskito;
        }
    }

    private void UpdateEncensZone()
    {
        if ((transform.position - encens.transform.position).magnitude < encens.GetRadius())
        {
            closeToEncens = true;
        }
        else
        {
            closeToEncens = false;
        }
    }
    private void UpdateTelZone()
    {
        if ((transform.position - tel.transform.position).magnitude < tel.GetRadius())
        {
            closeToTelephone = true;
        }
        else
        {
            closeToTelephone = false;
        }
    }

    private void UpdateRadiateurZone()
    {
        if ((transform.position - radiateur.transform.position).magnitude < radiateur.GetRadius())
        {
            moskitoTouchDetector.SetattachToObjectProbability(radiateur.attachToObjectProbabilityRadiateur);
        }
        else
        {
            moskitoTouchDetector.ResetattachToObjectProbability();
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

        if (player.GetCurrentPlayerStatus() == PlayerStatus.Hitting)
        {
            SetMoskitoStatus(MoskitoStatus.GoSafe);
            lastPatrollingZone = MoskitoZone.None;
            moskitoTouchDetector.StopAttachingObject();
            return;
        }

        if (moskitoTouchDetector.IsAttachedToObject())
        {
           
            return;
        }
        else
        {

        }
        
        if (closeToEncens && encens.IsActive())
        {
            SetMoskitoStatus(MoskitoStatus.Encens);
            patrollingZone = MoskitoZone.Encens;
            return;
        }

        if (closeToTelephone && tel.IsActive())
        {
            SetMoskitoStatus(MoskitoStatus.Telephone);
            patrollingZone = MoskitoZone.Telephone;
            return;
        }

        if (moskitoStatus == MoskitoStatus.AfterAttack && timeSinceItAttacked < timeAfterAttack) {
            timeSinceItAttacked += Time.deltaTime;
            return;
        }

        if (player.GetCurrentPlayerStatus() == PlayerStatus.Hitting)
        {
            SetMoskitoStatus(MoskitoStatus.GoSafe);
            lastPatrollingZone = MoskitoZone.None;
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
                if (patrollingZone == MoskitoZone.None && lastPatrollingZone != MoskitoZone.Confort)
                {
                    PatrolInZone(MoskitoZone.Confort);
                }
                else {
                    if (patrollingZone != MoskitoZone.None)
                    {
                        PatrolInZone(patrollingZone);
                        return;
                    }
                    if(player.GetCurrentPlayerStatus() == PlayerStatus.Idle)
                    {
                        SetPatrolMinAndMaxTime(3, 5);
                        SetMoskitoStatus(MoskitoStatus.GoClose);
                    }
                    else
                    {
                        if(playerOrientation == PlayerOrientation.BackToMoskito)
                        {
                            SetMoskitoStatus(MoskitoStatus.GoClose);
                            SetPatrolMinAndMaxTime(4, 6);
                        }
                        else
                        {
                            SetPatrolMinAndMaxTime(4, 6);
                            SetMoskitoStatus(MoskitoStatus.GoSafe);
                            lastPatrollingZone = MoskitoZone.Danger; // du coup il peut recommencer à patroller dans la zone COnfort
                        }
                    }
                }
                break;
            case MoskitoZone.Danger:
                if (patrollingZone == MoskitoZone.None && lastPatrollingZone != MoskitoZone.Danger)
                {
                    PatrolInZone(MoskitoZone.Danger);
                }
                else
                {
                    if(patrollingZone != MoskitoZone.None)
                    {
                        SetPatrolMinAndMaxTime(2, 3);
                        PatrolInZone(patrollingZone);
                        return;
                    }
                    if(player.GetCurrentPlayerStatus() == Player.PlayerStatus.Idle)
                    {
                        SetPreparingAttackMinAndMaxTime(2,4);
                        PrepareToAttack();
                    }
                    else
                    {
                        if(playerOrientation == PlayerOrientation.FacingMoskito)
                        {
                            SetMoskitoStatus(MoskitoStatus.GoSafe);
                        }
                        else
                        {
                            SetPreparingAttackMinAndMaxTime(3, 5);
                            PrepareToAttack();
                        }
                    }
                }
                break;
            case MoskitoZone.Far:
                if (patrollingZone == MoskitoZone.None && lastPatrollingZone != MoskitoZone.Far)
                {
                    SetPatrolMinAndMaxTime(2, 3);
                    PatrolInZone(MoskitoZone.Far);
                }
                else
                {
                    if (patrollingZone != MoskitoZone.None)
                    {
                        SetPatrolMinAndMaxTime(2, 3);
                        PatrolInZone(patrollingZone);

                        return;
                    }
                    SetMoskitoStatus(MoskitoStatus.GoClose);
                }
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

        if(moskitoStatus != MoskitoStatus.StayConfort && moskitoStatus != MoskitoStatus.StayDanger && moskitoStatus != MoskitoStatus.StayFar)
        {
            patrollingZone = MoskitoZone.None;
        }
        moskitoStatus = _newStatus;
        SetAudioBasedOnStatus();
    }
    private void SetAudioBasedOnStatus()
    {
        switch (moskitoStatus)
        {
            case MoskitoStatus.GoClose:
                FadeInAudio();
                break;
            case MoskitoStatus.PrepareToAttack:
                FadeInAudio();
                break;
            case MoskitoStatus.AfterAttack:
                FadeOutAudio();
                break;
            case MoskitoStatus.Sneak:
                FadeOutAudio();
                break;
            case MoskitoStatus.GoSafe:
                FadeOutAudio();
                break;
            case MoskitoStatus.GoBehind:
                FadeOutAudio();
                break;
            case MoskitoStatus.Encens:
                FadeInAudio();
                break;
            case MoskitoStatus.LightMod:
                FadeOutAudio();
                break;
            case MoskitoStatus.GoOutsidePhoneLight:
                FadeInAudio();
                break;
            case MoskitoStatus.StayFar:
                FadeOutAudio();
                break;
            case MoskitoStatus.StayConfort:
                FadeInAudio();
                break;
            case MoskitoStatus.StayDanger:
                FadeInAudio();
                break;
            case MoskitoStatus.Telephone:
                FadeInAudio();
                break;
            default:
                break;
        }
    }
    private void FadeInAudio()
    {
        if(!isInvisible)
        {
            bzzbzz.Play();
        }
        AudioManager.instance.FadeSound(_audioSource, fadeAudioDuration, 1);
    }
    private void FadeOutAudio()
    {
        bzzbzz.Stop();
        AudioManager.instance.FadeSound(_audioSource, fadeAudioDuration, 0);
    }

    private void PrepareToAttack()
    {
        if(timeSincePreparingAttack == 0)
        {
            timePreparingAttack = UnityEngine.Random.Range(timePreparingAttackMin, timePreparingAttackMax);
        }
        timeSincePreparingAttack += Time.deltaTime;

        if (timeSincePreparingAttack > timePreparingAttack)
        {
            SetMoskitoStatus(MoskitoStatus.AfterAttack);
            GameManager.instance.PlayerPique();
        }
        else
        {
            SetMoskitoStatus(MoskitoStatus.PrepareToAttack);
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

    private void PatrolInZone(MoskitoZone zone)
    {
        if(zone != patrollingZone)
        {
            if(zone == lastPatrollingZone)
            {
                return;
            }
            else
            {
                lastPatrollingZone = MoskitoZone.None;
            }
            timeSincePatrolling = 0;
            patrollingZone = zone;
        }
        switch (patrollingZone) { 
            case MoskitoZone.Far:
                if (moskitoStatus != MoskitoStatus.StayFar)
                {
                    SetMoskitoStatus(MoskitoStatus.StayFar);
                    timeBeforeStopPatrolling = UnityEngine.Random.Range(timePatrollingMin, timePatrollingMax);
                    return;
                }
                timeSincePatrolling += Time.deltaTime;
                if (timeSincePatrolling > timeBeforeStopPatrolling)
                {
                    SetMoskitoStatus(MoskitoStatus.GoClose);
                    lastPatrollingZone = patrollingZone;
                    patrollingZone = MoskitoZone.None;
                }
                break;
            case MoskitoZone.Confort:
                if (moskitoStatus != MoskitoStatus.StayConfort )
                {
                    SetMoskitoStatus(MoskitoStatus.StayConfort);
                    timeBeforeStopPatrolling = UnityEngine.Random.Range(timePatrollingMin, timePatrollingMax);
                    return;
                }
                timeSincePatrolling += Time.deltaTime;
                if (timeSincePatrolling > timeBeforeStopPatrolling)
                {
                    SetMoskitoStatus(MoskitoStatus.GoClose);
                    lastPatrollingZone = patrollingZone;
                    patrollingZone = MoskitoZone.None;
                }
                break;
            case MoskitoZone.Danger:
                if (moskitoStatus != MoskitoStatus.StayDanger)
                {
                    SetMoskitoStatus(MoskitoStatus.StayDanger);
                    timeBeforeStopPatrolling = UnityEngine.Random.Range(timePatrollingMin, timePatrollingMax);
                    return;
                }
                timeSincePatrolling += Time.deltaTime;
                if (timeSincePatrolling > timeBeforeStopPatrolling)
                {
                    SetMoskitoStatus(MoskitoStatus.PrepareToAttack);
                    lastPatrollingZone = patrollingZone;
                    patrollingZone = MoskitoZone.None;
                }
                break;
        }   
    }

    private void SetPatrolMinAndMaxTime(float durationMin,float durationMax)
    {
        timePatrollingMin = durationMin;
        timePatrollingMax = durationMax;
    }
    private void SetPreparingAttackMinAndMaxTime(float durationMin, float durationMax)
    {
        timePreparingAttackMin = durationMin;
        timePreparingAttackMax = durationMax;
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
    public MoskitoZone GetMoskitoZone()
    {
        return moskitoZone;
    }

    public MoskitoZone GetPatrollingZone()
    {
        return patrollingZone;
    }

    public Player GetPlayer()
    {
        return player;
    }

    public MoskitoTouchDetector GetTouchDetector() {
        return moskitoTouchDetector;
    }

    public void SetPlayer(Player _player)
    {
        player = _player;
    }

    public void SetEncens(Encens _encens)
    {
        encens = _encens;
    }
    public void SetRadiateur(S_Radiateur rad)
    {
        radiateur = rad;
    }
    public void SetTel(S_Telephone t)
    {
        tel = t;
    }

    public void SetMoskitoBox(BoxCollider _moskitoBox)
    {
        moskitoBox = _moskitoBox;
    }

    public void setParticle(bool isOn)
    {
        isInvisible = isOn;
    }

    public BoxCollider GetMoskitoBox()
    {
        return moskitoBox;
    }

    public void Kiecked()
    {
        _canMove = false;
        if (_isAlive)
        {
            _isAlive = false;
            Destroy(_audioSource);
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.volume = 1;
            _audioSource.PlayOneShot(AudioManager.instance.findAudioClip("Moskito_Dying"),2);
            //Debug.Log("METTRE SON MOUSTIQUE MORT");
            StartCoroutine(WaitToDie());
        }
            
    }
    IEnumerator WaitToDie()
    {
        yield return new WaitForSeconds(5f);
        GameManager.instance.MoskitoKill(this);
        Destroy(gameObject);
    }

    public void Stunned(float newSpeed, float duration)
    {
        moskitoController.SetSpeed(newSpeed);
        StartCoroutine(StunnedTime(duration));
    }

    IEnumerator StunnedTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        moskitoController.ResetSpeed();
    }

    public void playerCriFuite()
    {

    }
}
