
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(Player))]

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 3;
    [SerializeField]
    private float mouseSensitivityX = 3f;

    [SerializeField]
    private float mouseSensitivityY = 3f;

    private PlayerMotor motor;

    private Vector3 velocity;

    [SerializeField]
    private Animator tapetteAnimator;

    private Player player;




    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        player = GetComponent<Player>();
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

        velocity = (moveHorizontal + moveVertical) * speed;

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

        if (Input.GetButtonDown("Fire1"))
        {
            tapetteAnimator.SetTrigger("Tap");
            player.OnTryingToHit();
        }
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }
}
