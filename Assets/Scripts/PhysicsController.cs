using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PhysicsController : MonoBehaviour {

    [Header("Locomotion Parameters")]
    [SerializeField]
    private float _mass = 75; // [kg]

    [SerializeField]    
    public float MaxRunningSpeed =(30.0f * 1000) / (60 * 60);

    [SerializeField]
    private float _acceleration = 3; // [m/s^2]

    [SerializeField]
    private float _jumpHeight = 1; // [m/s^2]

    [SerializeField]
    private float _dragOnGround = 1; // []

    [SerializeField]
    private float _dragInAir = 1; // []

    [SerializeField]
    private float _dragWhileFalling = 1; // []

    private CharacterController _characterController;

    public Vector3 Movement { get; set; }
    public Vector3 Aim { get; set; }
    private Vector3 _velocity = Vector3.zero;
    private Transform _playerTransform;
    public bool Jump { get; set; }
    public bool Jumping { get; set; }


    [SerializeField] private Transform _cameraRoot;
    [SerializeField] private float _camRotation;
    [SerializeField] private float _minCamAngle;
    [SerializeField] private float _maxCamAngle;
    [SerializeField] private float _rotationSpeed;

    // Use this for initialization
    void Start () {
        _characterController = GetComponent<CharacterController>();
        _playerTransform = transform;
	}

    private void Update()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate () {
        ApplyGround();
        ApplyGravity();

        ApplyRotation();
        ApplyMovement();
        ApplyGroundDrag();

        ApplyAirDrag();
        ApplyJump();


        LimitMaximumRunningSpeed();

        _characterController.Move(_velocity * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        if (_characterController.isGrounded)
        {
            var relativeMovement = RelativeDirection(Movement);
            _velocity += relativeMovement * _acceleration * Time.deltaTime; // F(= m.a) [m/s^2] * t [s]
        }
    }

    private Vector3 RelativeDirection(Vector3 direction)
    {
        Quaternion forwardRotation =
            Quaternion.LookRotation(Vector3.Scale(_playerTransform.forward, new Vector3(1, 0, 1)));

        Vector3 relativeMovement = forwardRotation * direction;
        return relativeMovement;
    }

    private void ApplyGround()
    {
        if (_characterController.isGrounded)
        {
            _velocity -= Vector3.Project(_velocity, Physics.gravity.normalized);
            Jumping = false;
        }
    }

    private void ApplyGravity()
    {
        if (!_characterController.isGrounded)
        {
            _velocity += Physics.gravity * Time.deltaTime; // g[m/s^2] * t[s]
        }

    }

    private void ApplyJump()
    {
        //https://en.wikipedia.org/wiki/Equations_of_motion
        //v^2 = v0^2  + 2*a(r - r0)
        //v = 0
        //v0 = ?
        //a = 9.81
        //r = 1
        //r0 = 0
        //v0 = sqrt(2 * 9.81 * 1) 
        //but => g is inverted

        if (Jump && _characterController.isGrounded)
        {
            _velocity += -Physics.gravity.normalized * Mathf.Sqrt(2 * Physics.gravity.magnitude * _jumpHeight);
            Jump = false;
            Jumping = true;
        }

    }

    private void ApplyAirDrag()
    {
        if (!_characterController.isGrounded)
        {
            _velocity.x = _velocity.x * (1 - Time.deltaTime * _dragInAir);

            if (_velocity.y < 0)
                _velocity.y = _velocity.y * (1 - Time.deltaTime * _dragWhileFalling);
        }
    }

    private void ApplyGroundDrag()
    {
        if (_characterController.isGrounded)
        {
            _velocity = _velocity * (1 - Time.deltaTime * _dragOnGround);
        }
    }

    private void LimitMaximumRunningSpeed()
    {
        Vector3 yVelocity = Vector3.Scale(_velocity, new Vector3(0, 1, 0));

        Vector3 xzVelocity = Vector3.Scale(_velocity, new Vector3(1, 0, 1));
        Vector3 clampedXzVelocity = Vector3.ClampMagnitude(xzVelocity, MaxRunningSpeed);

        _velocity = yVelocity + clampedXzVelocity;
    }

    public void ApplyRotation()
    {
        _characterController.transform.eulerAngles += new Vector3(0, Aim.x, 0) * _rotationSpeed * Time.deltaTime;
        //vertical rotation
        _camRotation += Aim.z * _rotationSpeed * Time.deltaTime; //get vertical rotation

        _camRotation = Mathf.Clamp(_camRotation, _minCamAngle, _maxCamAngle); //clamp vertical rotation

        _cameraRoot.eulerAngles = new Vector3(_camRotation, _cameraRoot.eulerAngles.y, _cameraRoot.eulerAngles.z);

    }
}
