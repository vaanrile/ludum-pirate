
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MoskitoMotor))]
[RequireComponent(typeof(Moskito))]


public class MoskitoController : MonoBehaviour
{
    [SerializeField]
    private float speed = 3;

    [SerializeField]
    private float rotationSpeed = 3f;

    private Transform target;

    [SerializeField]
    private Player player;

    private Moskito moskito;
    private MoskitoMotor motor;

    private Vector3 direction;

    private void Awake()
    {
        moskito = GetComponent<Moskito>();
        motor = GetComponent<MoskitoMotor>();
        target = transform;
    }

    private void Update()
    {

        if (GameManager.IsOn)
        {  
            motor.Move(Vector3.zero);
            motor.Rotate(Vector3.zero);
            return;
        }

        //BEFORE SETTING DIRECTION
        switch (moskito.GetMoskitoStatus())
        {
            case Moskito.MoskitoStatus.GoClose:
                UpdateTarget(player.transform);
                break;
            case Moskito.MoskitoStatus.GoBehind: 
                UpdateTarget(player.GetBehindPlayerTransform()); 
                break;
            case Moskito.MoskitoStatus.GoSafe:
                UpdateTarget(player.transform);
                break;
            case Moskito.MoskitoStatus.AfterAttack:
                UpdateTarget(player.transform);
                break;
            case Moskito.MoskitoStatus.PrepareToAttack:
                UpdateTarget(player.transform);
                break;
            case Moskito.MoskitoStatus.StayFar:
                UpdateTarget(player.transform);
                break;
        }

        
        direction = (target.position - transform.position).normalized;

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
                direction = -direction;
                break;
        }
        
        transform.forward = direction.normalized;
        float yMov = Random.Range(-0.2f, 0.2f);

        Vector3 velocity = (direction) * speed;

        //Move
        motor.Move(velocity);
    }

    public void UpdateTarget(Transform _newTarget)
    {
        target = _newTarget;
    }
}

