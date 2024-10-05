
using UnityEngine;

[RequireComponent(typeof(MoskitoMotor))]


public class MoskitoController : MonoBehaviour
{
    [SerializeField]
    private float speed = 3;

    private MoskitoMotor motor;

    [SerializeField]
    private float rotationSpeed = 3f;

    [SerializeField]
    private Transform graphics;

    [SerializeField]
    private Transform target;


    private void Start()
    {
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

        Debug.DrawLine(transform.position, transform.position + transform.forward * 100, Color.green);
        Debug.DrawLine(transform.position, (transform.position + (target.position - transform.position)).normalized * 100, Color.yellow);

        //Calculer la vélocité du mouvement de notre joueur
        float xMov = Random.Range(-3f, 3f);
        float zMov = Random.Range(-2f, 2f);
        float yMov = Random.Range(-2f, 2f);

        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;
        Vector3 moveUp = graphics.forward * yMov;


        Vector3 velocity = (moveHorizontal + moveVertical + moveUp) * speed;

        //Move
        motor.Move(velocity);

        // on calcule la rotation du moustique
        float yRot = 2f;
        Vector3 rotation = new Vector3(0, yRot, 0) * rotationSpeed;

        motor.Rotate(rotation);

        // on calcule la rotation de du graphics
        float cameraRotationX = yMov;


        motor.RotateCamera(cameraRotationX);

    }
}

