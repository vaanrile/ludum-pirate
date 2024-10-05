using UnityEngine;
[RequireComponent(typeof(Rigidbody))]

public class PlayerMotor : MonoBehaviour
{

    private Vector3 velocity;
    private Vector3 rotation;

    [SerializeField]
    private Transform playerHead;

    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;

    private Rigidbody rb;

    [SerializeField]
    private float cameraRotationLimit = 85f;

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
    public void RotateCamera(float _cameraRotationX)
    {
        cameraRotationX = _cameraRotationX;
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

    private void PerformRotation()
    {
        //On calcule la rotation de la camera
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        currentCameraRotationX -= cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
        //on applique la rotation de la cam�ra
        playerHead.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

}
