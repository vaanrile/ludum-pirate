
using DG.Tweening.Core.Easing;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MoskitoMotor))]
[RequireComponent(typeof(Moskito))]


public class MoskitoController : MonoBehaviour
{
    [Header("__Game Design Variables")]
    [SerializeField]
    private float speed = 3;
    private float initialSpeed;

    [Header("__Dev Variables")]
    [SerializeField]
    private bool isFrenetic;
    [SerializeField]
    [Range(0,100)]
    private float randomMoveIntensity;
    [SerializeField]
    [Range(0, 1)]
    private float freneticIntensity;
    [SerializeField]
    private Vector3 randomDirection;

    private Transform target;

    private GameObject randomTargetMoskito;

    public GameObject prefabInstantiate;

    private Moskito moskito;
    private MoskitoMotor motor;

    private Vector3 direction;
    private Vector3 randomNormalizedVector;

    private Vector3 velocity;

    private void Awake()
    {
        moskito = GetComponent<Moskito>();
        motor = GetComponent<MoskitoMotor>();
        target = transform;
        randomNormalizedVector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        initialSpeed = speed;
        StartCoroutine(GenerateRandomNormalizedVector3EachXseconds(2));

        randomTargetMoskito = Instantiate(prefabInstantiate);
    }

    IEnumerator GenerateRandomNormalizedVector3EachXseconds(float duration)
    {
        yield return new WaitForSeconds(duration);
        randomNormalizedVector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        StartCoroutine(GenerateRandomNormalizedVector3EachXseconds(Random.Range(.2f,.7f)));
    }

    private void Update()
    {
        direction = Vector3.zero;
        motor.Move(Vector3.zero);
        if (GameManager.IsOn)
        {  
            motor.Move(Vector3.zero);
            motor.Rotate(Vector3.zero);
            return;
        }
        if (!moskito.GetMoskitoBox().bounds.Contains(moskito.transform.position))
        {
            moskito.transform.position = NearLocInMoskitoBox();
        }

        if (moskito.GetTouchDetector().IsAttachedToObject())
        {
            motor.Move(Vector3.zero);
            return;
        }
        //BEFORE SETTING DIRECTION
        switch (moskito.GetMoskitoStatus())
        {
            case Moskito.MoskitoStatus.GoBehind: 
                UpdateTarget(moskito.GetPlayer().GetBehindPlayerTransform()); 
                break;
            case Moskito.MoskitoStatus.GoClose:
            case Moskito.MoskitoStatus.GoSafe:
            case Moskito.MoskitoStatus.AfterAttack:
            case Moskito.MoskitoStatus.PrepareToAttack:
            case Moskito.MoskitoStatus.StayFar:
            case Moskito.MoskitoStatus.StayConfort:
            case Moskito.MoskitoStatus.StayDanger:
                UpdateTarget(moskito.GetPlayer().transform);
                break;
            case Moskito.MoskitoStatus.Encens:
                UpdateTarget(moskito.GetEncens().transform);
                break;
            case Moskito.MoskitoStatus.Telephone:

                GameManager.instance.SetRandomTargetMoskito(randomTargetMoskito);

                UpdateTarget(randomTargetMoskito.transform);
                break;
        }


        direction = (target.position - transform.position).normalized;
        if (isFrenetic)
        {
            if(Random.Range(0f,1f) > 0.99)
            {
                randomDirection = Vector3.zero;
            }
            randomDirection += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            direction = Vector3.Lerp(direction, randomDirection, randomMoveIntensity* Time.deltaTime);
        }


        //DETERMINE IF SHUOLD AVOID OR GO TO TARGET
        switch (moskito.GetMoskitoStatus())
        {
            case Moskito.MoskitoStatus.Sneak:
                direction = Vector3.zero;
                break;
            case Moskito.MoskitoStatus.GoSafe:
                direction = -direction;
                break;
            case Moskito.MoskitoStatus.AfterAttack:
                direction = -direction;
                break;
            case Moskito.MoskitoStatus.StayFar:
                if(moskito.GetMoskitoZone() == Moskito.MoskitoZone.Far)
                {
                    direction = randomNormalizedVector;
                }
                else
                {
                    direction = -direction;
                }
                break;
            case Moskito.MoskitoStatus.StayConfort:
                switch (moskito.GetMoskitoZone())
                {
                    case Moskito.MoskitoZone.Confort:
                        direction = randomNormalizedVector;
                        break;
                    case Moskito.MoskitoZone.Danger:
                        direction = -direction;
                        break;
                    case Moskito.MoskitoZone.Far: // go closer
                        break;
                }
                break;
            case Moskito.MoskitoStatus.StayDanger:
                if (moskito.GetMoskitoZone() == Moskito.MoskitoZone.Danger)
                {
                    direction = randomNormalizedVector;
                } // else we go closer
                break;
            case Moskito.MoskitoStatus.PrepareToAttack:
                direction = Vector3.zero;
                break;
        }
        
        if(direction != Vector3.zero)
        {
            transform.forward = direction.normalized;
        }
        velocity = (direction.normalized) * speed;
        //Move
        motor.Move(velocity);
    }

    public void UpdateTarget(Transform _newTarget)
    {
        target = _newTarget;
    }

    public void SetSpeed(float newSpeed)
    {
       speed = newSpeed;
    }
    public void ResetSpeed()
    {
        speed = initialSpeed;
    }

    private Vector3 RandomLocInMoskitoBox()
    {
        var bounds = moskito.GetMoskitoBox().bounds;
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z));
    }
    private Vector3 NearLocInMoskitoBox()
    {
        var bounds = moskito.GetMoskitoBox().bounds;
        var vec = transform.position;

        float offset = 2f;

        if (vec.x < bounds.min.x)
        {
            vec.x = bounds.min.x+offset;
        }
        else if (vec.x > bounds.max.x)
        {
            vec.x = bounds.max.x-offset;
        }

        if (vec.y < bounds.min.y)
        {
            vec.y = bounds.min.y+offset;
        }
        else if (vec.y > bounds.max.y)
        {
            vec.y = bounds.max.y-offset;
        }

        if (vec.z < bounds.min.z)
        {
            vec.z = bounds.min.z+offset;
        }
        else if (vec.z > bounds.max.z)
        {
            vec.z = bounds.max.z-offset;
        }

        return vec;
    }
}

