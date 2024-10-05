
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
    }

    private void Update()
    {

        if (GameManager.IsOn)
        {  
            motor.Move(Vector3.zero);
            motor.Rotate(Vector3.zero);
            return;
        }

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
            case Moskito.MoskitoStatus.PrepareToAttack:
                UpdateTarget(player.transform);
                break;
            case Moskito.MoskitoStatus.Attack:
                UpdateTarget(player.transform);
                break;
        }

        
        direction = (target.position - transform.position).normalized;


        switch (moskito.GetMoskitoStatus())
        {
            case Moskito.MoskitoStatus.Sneak:
                direction = Vector3.zero;
                break;
            case Moskito.MoskitoStatus.GoSafe:
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

