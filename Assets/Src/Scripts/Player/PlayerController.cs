
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

    [SerializeField]
    private float repulseForce = 50f;

    [SerializeField] 
    private float hitBox = 2f;
    //Hitbox

    public GameObject cameraLight;
    public GameObject cameraUnlight;

    private PlayerMotor motor;

    private Vector3 velocity;

    [SerializeField]
    private Animator tapetteAnimator;

    private Player player;

    private bool _canMoveCamera=true;

    private S_Tapette tapette;

    private AudioSource _audioSource;
    public float footstepTimerInit;
    private float _footstepCurrent;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        player = GetComponent<Player>();
        tapette = GetComponentInChildren<S_Tapette>();
        _audioSource = GetComponent<AudioSource>();
        _footstepCurrent = footstepTimerInit;
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
        if (Input.GetMouseButtonDown(0))
        {
            // AudioManager.instance?.PlayLol();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _canMoveCamera = !_canMoveCamera;
        }
        if (!_canMoveCamera)
        {
            return;   
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
            //ApplyRepulseForce();
            //Debug.DrawLine(transform.position, transform.position + transform.forward * hitBox, Color.red, 1f);
        }
        if (velocity.magnitude != 0)
        {
            _footstepCurrent -= Time.deltaTime;
            if (_footstepCurrent <= 0)
            {
                _audioSource.PlayOneShot(AudioManager.instance.footstepArray[Random.Range(0, AudioManager.instance.footstepArray.Length - 1)]);
                _footstepCurrent = footstepTimerInit;
            }
        }
    }
    public void ApplyRepulseForce()
    {
        //Ou ki son les rigidbody ? 
        tapette.kick(hitBox, repulseForce);
        _audioSource.PlayOneShot(AudioManager.instance.swatterArray[Random.Range(0, AudioManager.instance.swatterArray.Length - 1)]);
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public void setCamera(bool lightOn)
    {
        if (lightOn)
        {
            cameraLight.SetActive(true);
            cameraUnlight.SetActive(false);
        }
        else
        {
            cameraLight.SetActive(false);
            cameraUnlight.SetActive(true);
        }
    }


}
