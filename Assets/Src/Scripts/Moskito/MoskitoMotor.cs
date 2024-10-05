using UnityEngine;
[RequireComponent(typeof(Rigidbody))]

public class MoskitoMotor : MonoBehaviour
{

    private Vector3 velocity;
    private Vector3 rotation;

    private Rigidbody rb;

    [SerializeField]
    private float rotationLimit = 90f;
    private float currentRotationX = 0f; 
    private float rotationX = 0f;

    [SerializeField]
    private Transform graphicsTransform;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    private void FixedUpdate()
    {

        PerformMovement();
        PerformRotation();
    }

    private void PerformMovement()
    {

        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

    }

    public void RotateCamera(float _currentRotationX)
    {
        rotationX = _currentRotationX;
    }

    private void PerformRotation()
    {
        //On calcule la rotation de la camera
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        currentRotationX -= rotationX;
        currentRotationX = Mathf.Clamp(currentRotationX, -rotationLimit, rotationLimit);
        //on applique la rotation de la caméra
        graphicsTransform.localEulerAngles = new Vector3(currentRotationX, 0f, 0f);
    }

}
