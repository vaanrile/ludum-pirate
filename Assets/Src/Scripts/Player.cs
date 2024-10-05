
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]


public class PlayerControler : MonoBehaviour
{
    [SerializeField]
    private float speed = 3;
    [SerializeField]
    private float mouseSensitivityX = 3f;

    [SerializeField]
    private float mouseSensitivityY = 3f;

    private PlayerMotor motor;


    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    private void Update()
    {

        if (GameManager.IsOn)
        {

            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }

            motor.Move(Vector3.zero);
            motor.Rotate(Vector3.zero);
            motor.RotateCamera(0f);
            return;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        //Calculer la vélocité du mouvement de notre joueur
        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");

        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;

        Vector3 velocity = (moveHorizontal + moveVertical) * speed;

        //jouer animation thruster
        motor.Move(velocity);

        // on calcule la rotation du player
        float yRot = Input.GetAxisRaw("Mouse X");
        Vector3 rotation = new Vector3(0, yRot, 0) * mouseSensitivityX;

        motor.Rotate(rotation);

        // on calcule la rotation de la cam
        float xRot = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = xRot * mouseSensitivityY;

        motor.RotateCamera(cameraRotationX);

        if (Input.GetMouseButtonDown(0))
        {
            AudioManager.instance.PlayLol();
        }
    }
}
