using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Locomotion
    private CharacterController _controller;
    private Vector3 _currentVelocity;
    private const float walkSpeed = 1f;
    private const float runSpeed = 3f;

    //Animation vars
    private bool _hasAnimator;
    private Animator _anim;
    private int _xVel;
    private int _yVel;
    private float _animLerpSpeed = 9f;

    //look around u 
    private float xRotation;
    [SerializeField, Range (0, 200)] private float mouseSensitivity;
    private Transform cam;
    private float upperLimit = -75;
    private float lowerLimit = 70f;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;

        _hasAnimator = TryGetComponent (out _anim);

        //get values from the _mainChar animator controller (found on the Animator Window)
        _xVel = Animator.StringToHash("x_Velocity");
        _yVel = Animator.StringToHash("y_Velocity");

        //For Temp
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        LookAround();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (!_hasAnimator) return;

        var H = Input.GetAxis("Horizontal");
        var V = Input.GetAxis("Vertical");

        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
       if (H == 0 && V == 0) targetSpeed = 0f;

        _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, H * targetSpeed, Time.deltaTime * _animLerpSpeed);
        _currentVelocity.z = Mathf.Lerp(_currentVelocity.z, V * targetSpeed, Time.deltaTime * _animLerpSpeed);

        _controller.Move(transform.TransformDirection(_currentVelocity) * Time.deltaTime);

        _anim.SetFloat(_xVel, _currentVelocity.x);
        _anim.SetFloat(_yVel, _currentVelocity.z);
    }


    private void LookAround()
    {
        if (!_hasAnimator) return;
        if (!PlayerInteract.canLook) return;
        var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.smoothDeltaTime;
        var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.smoothDeltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, upperLimit, lowerLimit);

        cam.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up, mouseX);
    }
}
